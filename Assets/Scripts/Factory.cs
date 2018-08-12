using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Mono-behavoir solely for building and initialising GameObjects.
/// </summary>
public class Factory : MonoBehaviour
{
    public GameObject SquarePrefab;
    public GameObject DeathParticlesPrefab;
    public GameObject PickupPrefab;
    public GameObject UIPrefab;
    public GameObject CameraPrefab;

    private void Start()
    {
        //TESTING STUB
        SpawnSquare(
            SquareType.Green,
            new Vector3(0, 3, 0),
            new Vector3(0, 1, 0),
            Quaternion.identity);

        var inv = new Inventory();
        var build = new Build();

        build.Squares[Vector3.zero] = SquareType.White;
        build.Squares[Vector3.up] = SquareType.Green;
        build.Squares[Vector3.up * 2] = SquareType.White;
        build.Squares[Vector3.left] = SquareType.Green;
        build.Squares[Vector3.right] = SquareType.Green;

        SpawnPlayer(
            build,
            inv,
            Vector3.zero);
    }

    private Square SpawnSquare(SquareType color,
                               Vector3 position,
                               Vector3 velocity,
                               Quaternion rotation)
    {
        var squareObject = Instantiate(SquarePrefab, position, rotation);
        var squareScript = AttachSquare(color, squareObject);
        squareScript.Factory = this;
        squareObject.GetComponent<Rigidbody2D>().velocity = velocity;
        return squareScript;
    }

    /// <summary>
    /// Attachs the square component of given type to the game object.
    /// </summary>
    private Square AttachSquare(SquareType color, GameObject square)
    {
        switch (color)
        {
            case SquareType.Green:
                return square.AddComponent<GreenSquare>();
            case SquareType.White:
                return square.AddComponent<WhiteSquare>();
            default:
                throw new UnityException(message: "Square type not found.");
        }
    }

    /// <summary>
    /// Builds a new player object with all attached squares and attaches a UI
    /// and Camera object also.
    /// 
    /// Fixed joints are put in on squares that are next to one another.
    /// </summary>
    private Player SpawnPlayer(Build build, Inventory inventory, Vector3 position)
    {
        var player = new Player
        {
            Inventory = inventory
        };

        // instantiate initial square.
        foreach (var square in build.Squares)
        {
            player.Squares[square.Key] = this.SpawnSquare(square.Value,
                                                        position + square.Key * Game.SquareSize,
                                                        Vector3.zero,
                                                        Quaternion.identity);
            player.Squares[square.Key].Player = player;
        }


        //Attach only neighbouring squares with fixed joints.
        var directions = new List<Vector3>
            {
                Vector3.up,
                Vector3.down,
                Vector3.left,
                Vector3.right
            };

        foreach (var square in build.Squares)
        {
            foreach (var direction in directions)
            {
                if (build.Squares.ContainsKey(square.Key + direction))
                {
                    this.FixSquares(
                        player.Squares[square.Key],
                        player.Squares[square.Key + direction]);
                }
            }
        }

        Game.Players.Add(player);

        player.UI = CreateUI();
        SpawnCamera(player);

        return player;
    }

    private UIController CreateUI()
    {
        var uiObject = Instantiate(UIPrefab);

        return uiObject.GetComponent<UIController>();
    }

    /// <summary>
    /// Puts a fixed joint from <paramref name="squareA"/> to
    ///  <paramref name="squareB"/>
    /// </summary>
    private void FixSquares(Square squareA, Square squareB)
    {
        var joint = squareA.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = squareB.gameObject.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Breaks any fixed joint between <paramref name="squareA"/>
    /// and <paramref name="squareB"/>. To be called on square destruction.
    /// </summary>
    private void BreakSquare(Square squareA, Square squareB)
    {
        //needs to be implemented.
    }

    /// <summary>
    /// Spawns death particles of a given color in a given location. To be
    /// called on square destruction.
    /// </summary>
    private void SpawnDeathParticles(Vector3 position, SquareType color)
    {
        var particleObject = 
            Instantiate(DeathParticlesPrefab,
                        position,
                        DeathParticlesPrefab.transform.rotation);

        var main = particleObject.GetComponent<ParticleSystem>().main;
        var finalColor = Game.GetColor(color);
        finalColor.a = Game.DeathParticleTransparency;
        main.startColor = finalColor;
    }

    private GameObject SpawnCamera(Player target)
    {
        var cam = Instantiate(CameraPrefab);
        cam.GetComponent<CameraFollowTarget>().Target = target;
        return cam;
    }

    /// <summary>
    /// Spawns the pickup in the given position with given color and velocity
    /// and returns the pickup.
    /// </summary>
    private Pickup SpawnPickup(Vector3 position,
                               Vector3 velocity,
                               SquareType color)
    {
        var pickupObject = Instantiate(PickupPrefab,
                                       position,
                                       Quaternion.identity);
        var pickupScript = pickupObject.AddComponent<Pickup>();
        pickupScript.Type = color;
        pickupObject.GetComponent<SpriteRenderer>().color = Game.GetColor(color);
        pickupObject.GetComponent<Rigidbody2D>().velocity = velocity;
        return pickupScript;
    }

    public void DestroySquare(Square square)
    {
        this.SpawnDeathParticles(square.transform.position, square.Color);
        this.SpawnPickup(square.transform.position,
                         square.GetComponent<Rigidbody2D>().velocity,
                         square.Color);
        Destroy(square.gameObject);
    }

}

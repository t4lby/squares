using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mono-behavoir solely for building and initialising GameObjects.
/// </summary>
public class Factory : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject SquarePrefab;
    public GameObject DeathParticlesPrefab;
    public GameObject PickupPrefab;
    public GameObject UIPrefab;
    public GameObject CameraPrefab;
    public GameObject BoostParticlesPrefab;

    private void Start()
    {
        
        if (Game.Players.Count == 0)
        {
            var inv = new Inventory();
            inv.Squares[SquareType.Purple] = 50;
            inv.Squares[SquareType.Blue] = 20;
            inv.Squares[SquareType.Green] = 10;
            var build = new Build();
            build.Squares[Vector3.zero] = SquareType.White;
            Game.Players.Add(
                SpawnPlayer(
                    build,
                    inv,
                    Vector3.zero));
        }
        else
        {
            Game.Players[0] = SpawnPlayer(
                Game.Players[0].Build,
                Game.Players[0].Inventory,
                Vector3.zero);
        }
    }

    //test stub
    private float nextSpawn;
    private float spawnDiff = 1;
    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            //SpawnRandomSquareInCircle(Game.Players[0].transform.position, 3, 10, true);
            nextSpawn = Time.time + spawnDiff;
        }
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
            case SquareType.Blue:
                return square.AddComponent<BlueSquare>();
            case SquareType.Purple:
                var purple = square.AddComponent<PurpleSquare>();
                purple.Particles = SpawnBoostParticles(purple.gameObject.transform,
                                                       SquareType.Purple);
                return purple;
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
        var player = Instantiate(PlayerPrefab).AddComponent<Player>();

        player.Inventory = inventory;

        foreach (var buildPair in build.Squares)
        {
            var rotation = build.Rotations.ContainsKey(buildPair.Key) ?
                                build.Rotations[buildPair.Key] :
                                Quaternion.identity;
            player.Squares[buildPair.Key] = this.SpawnSquare(buildPair.Value,
                                                        position + buildPair.Key * Game.SquareSize,
                                                        Vector3.zero,
                                                        rotation);
            player.Squares[buildPair.Key].Player = player;
            player.Squares[buildPair.Key].Regenerates = true;
            player.Squares[buildPair.Key].PositionInPlayer = buildPair.Key;
            if (build.Mappings.ContainsKey(buildPair.Key))
            {
                player.Squares[buildPair.Key].Mapped = true;
                player.Squares[buildPair.Key].Mapping =
                    build.Mappings[buildPair.Key];
            }
        }

        foreach (var square in player.Squares)
        {
            FixSquare(square.Value, player);
        }
        player.UI = CreateUI();
        player.UI.UpdateSquareCountUI(inventory.Squares);
        SpawnCamera(player);
        player.Build = build;
        player.DropSmallestComponents();
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

    private void FixSquare(Square square, Player player)
    {
        var joint = square.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = player.gameObject.GetComponent<Rigidbody2D>();
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
        cam.GetComponent<CameraFollowTarget>().Target = target.transform;
        return cam;
    }

    private GameObject SpawnBoostParticles(Transform parent, SquareType color)
    {
        var particles = Instantiate(BoostParticlesPrefab,
                                    parent);
        var main = particles.GetComponent<ParticleSystem>().main;
        main.startColor = Game.GetColor(color);
        return particles;
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

        //remove joints
        foreach (var joint in square.GetComponents<HingeJoint2D>())
        {
            foreach (var otherJoint in joint.connectedBody.GetComponents<HingeJoint2D>())
            {
                if (otherJoint.connectedBody == square.GetComponent<Rigidbody2D>())
                {
                    Destroy(otherJoint);
                }
            }
        }

        if (square.Player != null)
        {
            square.Player.Build.Squares.Remove(square.PositionInPlayer);
            square.Player.Build.Rotations.Remove(square.PositionInPlayer);
            square.Player.Build.Mappings.Remove(square.PositionInPlayer);
            square.Player.Squares.Remove(square.PositionInPlayer);
            square.Player.DropSmallestComponents();
        }
        Destroy(square.gameObject);
    }

    private void SpawnRandomSquareInCircle(Vector3 centre, float minRadius, float maxRadius, bool randomRotation)
    {
        var rotation = Quaternion.identity;
        if (randomRotation)
        {
            rotation.eulerAngles = new Vector3(0, 0, Random.Range(0, 90));
        }

        SquareType sType = Game.ActiveSquareTypes[Random.Range(0, Game.ActiveSquareTypes.Count)];

        var angle = Random.Range(0, 360);
        var position = centre + Random.Range(minRadius, maxRadius) * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        SpawnSquare(sType, position, Vector3.zero, rotation);
    }
}

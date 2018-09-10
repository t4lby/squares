using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public GameObject BoostParticlesPrefab;
    public GameObject FireParticlesPrefab;
    public GameObject BulletPrefab;
    public GameObject BuilderPrefab;

    public List<Square> SpawnedSquares;

    private void Start()
    {
        if (Game.Players.Count == 0)
        {
            var inv = new Inventory();
            /*inv.Squares[SquareType.Purple] = 50;
            inv.Squares[SquareType.Blue] = 20;
            inv.Squares[SquareType.Yellow] = 10;
            inv.Squares[SquareType.Red] = 10;*/
            var build = new Build();
            /*build.Squares[Vector3.zero] = SquareType.White;
            build.Squares[Vector3.up] = SquareType.Blue;
            build.Mappings[Vector3.up] = KeyCode.I;
            build.Squares[Vector3.left] = SquareType.Blue;
            build.Mappings[Vector3.left] = KeyCode.L;
            build.Squares[Vector3.right] = SquareType.Blue;
            build.Mappings[Vector3.right] = KeyCode.J;
            build.Squares[Vector3.right + Vector3.down] = SquareType.Purple;
            build.Squares[Vector3.left + Vector3.down] = SquareType.Purple;
            build.Squares[Vector3.up * 2] = SquareType.Red;*/
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
    private float spawnDiff = 3;

    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            SpawnRandomSquareInCircle(Game.Players[0].Position, 3, 10, true);
            nextSpawn = Time.time + spawnDiff;
        }

        //Manually updates position for all players.
        foreach (var player in Game.Players)
        {
            player.Position = player.GetPosition();
        }
    }

    public Square SpawnSquare(SquareType color,
                               Vector3 position,
                               Vector3 velocity,
                               Quaternion rotation)
    {
        var squareObject = Instantiate(SquarePrefab, position, rotation);
        var squareScript = AttachSquare(color, squareObject);
        squareScript.Factory = this;
        squareObject.GetComponent<Rigidbody>().velocity = velocity;
        SpawnedSquares.Add(squareScript);
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
            case SquareType.Yellow:
                return square.AddComponent<YellowSquare>();
            case SquareType.Purple:
                var purple = square.AddComponent<PurpleSquare>();
                purple.Particles = SpawnBoostParticles(purple.gameObject.transform,
                                                       SquareType.Purple);
                return purple;
            case SquareType.Red:
                var red = square.AddComponent<RedSquare>();
                red.Particles = SpawnFireParticles(red.gameObject.transform);
                return red;
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
            foreach (var direction in Game.Directions)
            {
                if (player.Squares.ContainsKey(square.Key + direction))
                {
                    FixSquares(square.Value,
                               player.Squares[square.Key + direction]);
                }
            }
        }
        player.Build = build;
        player.DropSmallestComponents();
        player.UI = CreateUI();
        player.UI.UpdateSquareCountUI(inventory.Squares);
        var builder = Instantiate(BuilderPrefab).GetComponent<RealtimeBuilder>();
        player.Builder = builder;
        builder.Player = player;
        builder.Factory = this;
        builder.UI = player.UI;
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
    public void FixSquares(Square squareA, Square squareB)
    {
        var joint = squareA.gameObject.AddComponent<FixedJoint>();
        joint.enablePreprocessing = false;
        joint.enableCollision = false;
        joint.connectedBody = squareB.gameObject.GetComponent<Rigidbody>();
        if (!squareA.ConnectedTo.Contains(squareB))
        {
            squareA.ConnectedTo.Add(squareB);
        }
        if (!squareB.ConnectedTo.Contains(squareA))
        {
            squareB.ConnectedTo.Add(squareA);
        }
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

    private GameObject SpawnBoostParticles(Transform parent, SquareType color)
    {
        var particles = Instantiate(BoostParticlesPrefab,
                                    parent);
        var main = particles.GetComponent<ParticleSystem>().main;
        main.startColor = Game.GetColor(color);
        return particles;
    }

    private GameObject SpawnFireParticles(Transform parent)
    {
        var particles = Instantiate(FireParticlesPrefab, parent);
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
        pickupScript.Color = color;
        var main = pickupObject.GetComponent<ParticleSystem>().main;
        main.startColor = Game.GetColor(color);

        return pickupScript;
    }

    public void DestroySquare(Square square)
    {
        this.SpawnDeathParticles(square.transform.position, square.Color);
        this.SpawnPickup(square.transform.position,
                         square.GetComponent<Rigidbody>().velocity,
                         square.Color);

        //remove joints
        foreach (var connectedSquare in square.ConnectedTo)
        {
            foreach (var otherJoint in connectedSquare.GetComponents<FixedJoint>())
            {
                if (otherJoint.connectedBody == square.GetComponent<Rigidbody>())
                {
                    Destroy(otherJoint);
                }
            }
            connectedSquare.ConnectedTo.Remove(square);
        }

        if (square.Player != null)
        {
            square.Player.Build.Squares.Remove(square.PositionInPlayer);
            square.Player.Build.Rotations.Remove(square.PositionInPlayer);
            square.Player.Build.Mappings.Remove(square.PositionInPlayer);
            square.Player.Squares.Remove(square.PositionInPlayer);
            square.Player.DropSmallestComponents();
        }

        SpawnedSquares.Remove(square);
        Destroy(square.gameObject);
    }

    public Bullet SpawnBullet(Vector3 position, Vector3 force, SquareType color)
    {
        var bulletObject = Instantiate(BulletPrefab, position, Quaternion.identity);
        bulletObject.GetComponent<Rigidbody>().AddForce(force);
        bulletObject.GetComponent<SpriteRenderer>().color = Game.GetColor(color);
        return bulletObject.AddComponent<Bullet>();
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

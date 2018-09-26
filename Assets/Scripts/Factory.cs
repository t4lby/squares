using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LevelBuilding;

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

    //test stub
    private float nextSpawn;
    private float spawnDiff = 3;

    private void Update()
    {
        if (Time.time > nextSpawn)
        {
        //    SpawnRandomSquareInCircle(Game.Players[0].Position, 3, 10, true);
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
            case SquareType.Black:
                return square.AddComponent<BlackSquare>();
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
    public Player SpawnPlayer(Build build, Inventory inventory, Vector3 position)
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
            var square = this.SpawnSquare(buildPair.Value,
                                          position + buildPair.Key * Game.SquareSize,
                                          Vector3.zero,
                                          rotation);
            player.Squares.Add(square);
            square.Player = player;
            square.Regenerates = true;
            square.PositionInPlayer = buildPair.Key;
            if (build.Mappings.ContainsKey(buildPair.Key))
            {
                square.Mapped = true;
                square.Mapping =
                    build.Mappings[buildPair.Key];
            }
        }

        foreach (var square in player.Squares)
        {
            foreach (var direction in Game.Directions)
            {
                var lookup = player.Squares.Where(sq => sq.PositionInPlayer == square.PositionInPlayer + direction);
                if (lookup.Count() == 1)
                {
                    FixSquares(square,
                               lookup.Single());
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
        if ((squareA.transform.position - squareB.transform.position).magnitude > Game.SquareSize + 0.1f ||
            (squareA.transform.position - squareB.transform.position).magnitude < Game.SquareSize - 0.1f)
        {
            //squares are not in a fixable position. return now.
            return;
        }
        var joint = squareA.gameObject.AddComponent<FixedJoint>();
        joint.enablePreprocessing = false;
        joint.enableCollision = false;
        joint.connectedBody = squareB.gameObject.GetComponent<Rigidbody>();
        joint.connectedAnchor = Tools.BestDirection(joint.connectedAnchor) * Game.SquareSize;
        squareA.ConnectedTo.Add(squareB);
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

        if (square.Player != null)
        {
            Tools.SetTriggerSurrounding(square, false);
        }
        //remove joints qq: move to own method.
        var toUnConnect = new List<Square>();
        foreach (var connectedSquare in square.ConnectedTo)
        {
            var toDestroy = new List<FixedJoint>();
            foreach (var otherJoint in connectedSquare.GetComponents<FixedJoint>())
            {
                if (otherJoint.connectedBody == square.GetComponent<Rigidbody>())
                {
                    toDestroy.Add(otherJoint);
                }
            }
            for (int i = toDestroy.Count - 1; i >= 0; i--)
            {
                Destroy(toDestroy[i]);
            }
            toUnConnect.Add(connectedSquare);
        }
        foreach (var connectedSquare in toUnConnect)
        {
            square.ConnectedTo.Remove(connectedSquare);
            connectedSquare.ConnectedTo.Remove(square);
        }

        if (square.Player != null)
        {
            square.Player.Build.Squares.Remove(square.PositionInPlayer);
            square.Player.Build.Rotations.Remove(square.PositionInPlayer);
            square.Player.Build.Mappings.Remove(square.PositionInPlayer);
            square.Player.Squares.Remove(square);
            square.Player.DropSmallestComponents();
        }
        foreach(var p in Game.Players)
        {
            p.Builder.JointTargets.Remove(square);
        }
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

    public List<Square> SpawnLevel(CellInfo[,] level)
    {
        var height = level.GetLength(0);
        var width = level.GetLength(1);
        var spawned = new Square[height, width];
        var output = new List<Square>();
        //spawnsquares
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (level[i,j].Color != SquareType.Blank)
                {
                    var rotation = new Quaternion
                    {
                        eulerAngles = new Vector3(0, 0, level[i, j].Rotation)
                    };
                    var square = this.SpawnSquare(level[i,j].Color,
                                                new Vector3(j, -i, 0) * Game.SquareSize,
                                                Vector3.zero,
                                                  rotation);
                    if (level[i,j].Fixed)
                    {
                        square.GetComponent<Rigidbody>().constraints =
                                  RigidbodyConstraints.FreezeAll;
                    }
                    square.Identifier = level[i, j].Identifier;
                    spawned[i,j] = square;
                    output.Add(square);
                }
            }
        }
        //join them
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (level[i,j].Color != SquareType.Blank)
                {
                    foreach (var c in level[i,j].JoinDirections)
                    {
                        var dir = Tools.CharToDirection(c);
                        if (i + dir[0] >= 0 && i + dir[0] < height &&
                            j + dir[1] >= 0 && j + dir[1] < width &&
                            spawned[i+dir[0], j+dir[1]] != null)
                        {
                            FixSquares(spawned[i, j],
                                       spawned[i + dir[0], j + dir[1]]);
                        }
                    }
                }
            }
        }
        return output;
    }
}

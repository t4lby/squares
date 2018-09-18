using UnityEngine;
using System.Collections;
using LevelBuilding;
using System.Collections.Generic;

public abstract class Level : MonoBehaviour
{
    public Factory Factory;

    protected List<Square> _LevelSquares;

    protected abstract Vector3 SpawnLocation { get; }
    protected abstract int LevelNumber { get; }

    protected void Start()
    {
        _LevelSquares = Factory.SpawnLevel(LevelReader.ReadLevel(@"Assets/Levels/Level"
                                              + LevelNumber.ToString()
                                              + ".csv"));
        var inv = new Inventory();
        inv.Squares[SquareType.Purple] = 50;
        inv.Squares[SquareType.Blue] = 20;
        inv.Squares[SquareType.Yellow] = 10;
        inv.Squares[SquareType.Red] = 10;
        var build = new Build();
        build.Squares[Vector3.zero] = SquareType.White;
        Game.Players.Add(
            Factory.SpawnPlayer(
                build,
                inv,
                SpawnLocation));
    }
}

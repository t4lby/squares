using System;
using UnityEngine;
using System.Linq;

public class Level1 : Level
{
    protected override Vector3 SpawnLocation
    {
        get
        {
            return new Vector3(3, -3, 0);
        }
    }

    protected override int LevelNumber
    {
        get
        {
            return 1;
        }
    }

    new private void Start()
    {
        base.Start();
        foreach (var square in _LevelSquares.Where(s => s.Identifier == "fire"))
        {
            square.Triggered = true;
        }
    }
}


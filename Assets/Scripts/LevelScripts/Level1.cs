using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Level1 : Level
{
    public float FireToggleTime;
    public float FireTravelDiff;
    private Dictionary<Square, float> _FireSquareToggleTimes;

    protected override Vector3 SpawnLocation
    {
        get
        {
            return new Vector3(3, -3, 0);
        }
    }

    new private void Start()
    {
        base.Start();
        _FireSquareToggleTimes = new Dictionary<Square, float>();
        var nextToggleTime = Time.time;
        foreach (var square in LevelSquares.Where(s => s.Identifier == "fire"))
        {
            _FireSquareToggleTimes[square] = nextToggleTime;
            nextToggleTime += FireTravelDiff;
        }
        nextToggleTime = Time.time;
        foreach (var square in LevelSquares.Where(s => s.Identifier == "fireTop"))
        {
            _FireSquareToggleTimes[square] = nextToggleTime;
            nextToggleTime += FireTravelDiff;
        }
    }

    new private void Update()
    {
        base.Update();
        foreach (var square in LevelSquares.Where(s => s.Identifier == "fire" || s.Identifier == "fireTop"))
        {
            if (Time.time > _FireSquareToggleTimes[square])
            {
                square.Triggered = !square.Triggered;
                _FireSquareToggleTimes[square] += FireToggleTime;
            }
        }
    }
}


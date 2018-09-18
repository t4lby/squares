using System;
using UnityEngine;
using System.Linq;

public class Level1 : Level
{
    public float FireOnTime;
    public float FireOffTime;
    private bool _FireOn;
    private float _NextFireChange;

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
        _FireOn = false;
        _NextFireChange = Time.time;
    }

    new private void Update()
    {
        base.Update();
        if (Time.time > _NextFireChange)
        {
            if (_FireOn)
            {
                SetFire(false);
                _FireOn = false;
                _NextFireChange = Time.time + FireOffTime;
            }
            else
            {
                SetFire(true);
                _FireOn = true;
                _NextFireChange = Time.time + FireOnTime;
            }
        }
    }

    private void SetFire(bool value)
    {
        foreach (var square in _LevelSquares.Where(s => s.Identifier == "fire"))
        {
            square.Triggered = value;
        }
    }
}


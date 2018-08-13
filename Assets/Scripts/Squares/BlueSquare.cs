﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueSquare : Square
{
    private KeyCode _KeyMapping = KeyCode.L;

    private bool _Mapped = true;

    private Color _TriggeredColor = new Color(0,1,1);

    protected override void SetSquareProperties()
    {
        this.Health = 1;
        this.Durability = 5;
        this.Invincible = false;
        this.Color = SquareType.Blue;
        this.MinimumTransparency = 0.25f;
    }

    public void MapKey(KeyCode key)
    {
        _KeyMapping = key;
        _Mapped = true;
    }

    public void UnmapKey()
    {
        _Mapped = false;
    }

    private void Update()
    {
        if (this.Player != null)
        {
            this.ProcessTriggers();
        }
    }

    private void SetTriggerSurrounding(bool value)
    {
        this.Triggered = value;
        var directions = new List<Vector3>
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };
        foreach (var d in directions)
        {
            if (this.Player.Squares.ContainsKey(this.PositionInPlayer + d))
            {
                this.Player.Squares[this.PositionInPlayer + d].Triggered = value;
            }
        }
    }

    private void ProcessTriggers()
    {
        if (Input.GetKeyDown(_KeyMapping))
        {
            SetTriggerSurrounding(true);
        }
        if (Input.GetKeyUp(_KeyMapping))
        {
            SetTriggerSurrounding(false);
        }

        if (this.Triggered)
        {
            this.GetComponent<SpriteRenderer>().color = _TriggeredColor;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Game.GetColor(this.Color);
        }
    }
}
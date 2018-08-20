﻿using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class YellowSquare : Square
{
    public float BulletSpeed { get; set; }

    /// <summary>
    /// Tracks if triggered is held.
    /// </summary>
    private bool _TriggerHeld;

    protected override void SetSquareProperties()
    {
        this.Health = 1f;
        this.Durability = 7.5f;
        this.Color = SquareType.Yellow;
        this.MinimumTransparency = 0.25f;
        this.RegenerationSpeed = 0.2f;
        this.BulletSpeed = 40f;
    }

    private void Start()
    {
        _TriggerHeld = false;
    }

    private void Update()
    {
        if (Triggered &&
            _TriggerHeld == false &&
            this.Player != null &&
            !this.Player.Squares.ContainsKey(this.PositionInPlayer + Vector3.up))
        {
            var angle = this.transform.rotation.eulerAngles.z * Mathf.PI / 180;
            var bulletDirection = new Vector3(-Mathf.Sin(angle),
                                             Mathf.Cos(angle),
                                              0).normalized;
            Factory.SpawnBullet(transform.position + Game.SquareSize * bulletDirection,
                                bulletDirection * BulletSpeed);
            _TriggerHeld = true;
        }

        if (Triggered == false)
        {
            _TriggerHeld = false;
        }
    }
}

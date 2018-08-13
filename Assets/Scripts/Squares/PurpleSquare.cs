﻿using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.PlayerLoop;

public class PurpleSquare : Square
{
    private float _Acceleration;
    public GameObject Particles;

    protected override void SetSquareProperties()
    {
        this.Health = 1;
        this.Durability = 30;
        this.Invincible = false;
        this.Color = SquareType.Purple;
        this.MinimumTransparency = 0.25f;
        this._Acceleration = 10;
    }

    private void Update()
    {
        //TEST STUB REMOVE!
        if (Input.GetKey(KeyCode.P))
        {
            Triggered = true;
        }
        else
        {
            Triggered = false;
        }
    }

    private void FixedUpdate() 
    {
        if (Triggered)
        {
            var angle = this.transform.rotation.eulerAngles.z * Mathf.PI / 180;
            var boostDirection = new Vector3(-Mathf.Sin(angle),
                                             Mathf.Cos(angle),
                                             0);

            this.GetComponent<Rigidbody2D>()
                .AddForce(boostDirection * _Acceleration);
            if (Particles != null)
            {
                Particles.SetActive(true);
            }
        }
        else
        {
            if (Particles != null)
            {
                Particles.SetActive(false);
            }
        }
    }
}

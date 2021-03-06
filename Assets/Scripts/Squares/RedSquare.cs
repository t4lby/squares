﻿using UnityEngine;
using System.Collections;
using System;

public class RedSquare : Square
{
    public GameObject Particles;

    protected override void SetSquareProperties()
    {
        this.Health = 1;
        this.Durability = 50;
        this.Invincible = false;
        this.Color = SquareType.Red;
        this.MinimumTransparency = 0.25f;
        this.RegenerationSpeed = 0.1f;
    }

    private void Start()
    {
        if (IsBuildSquare)
        {
            Destroy(Particles.GetComponent<Collider>());
        }
    }

    protected override void UpdateSquare()
    {
        if (Triggered)
        {
            if (Particles != null)
            {
                Particles.GetComponent<ParticleSystem>().Play();
                Particles.SetActive(true);
            }
        }
        else
        {
            if (Particles != null)
            {
                Particles.GetComponent<ParticleSystem>().Stop();
                Particles.SetActive(false);
            }
        }
    }
}

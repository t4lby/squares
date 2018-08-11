using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareController : MonoBehaviour {

    public SquareType SType;

    public float Health = 1;
    public float Durability = 10;
    public float MinimumTransparency = 0.25f;
    public GameObject DeathParticles;
    public float DeathParticleTransparency = 0.5f;
    public GameObject Pickup;

    protected SpriteRenderer _SR;
    protected Rigidbody2D _RB;
	
	void Start ()
    {
        _SR = GetComponent<SpriteRenderer>();
        _RB = GetComponent<Rigidbody2D>();
        Health = 1;
        _SR.color = Game.GetColor(this.SType);
	}
	
    /// <summary>
    /// Updates the squares transparency based on current health.
    /// </summary>
    protected void UpdateTransparency()
    {
        var current = _SR.color;
        _SR.color = new Color(current.r, current.g, current.b, MinimumTransparency + Health * (1 - MinimumTransparency));
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")|
            collision.gameObject.CompareTag("Square"))
        {
            Health -= collision.relativeVelocity.magnitude / Durability;
            this.UpdateTransparency();

            if (Health < 0)
            {
                //SPAWN PARTICLE SYSTEM
                Color particleColor = _SR.color;
                particleColor.a = DeathParticleTransparency;
                ParticleSystem.MainModule settings = DeathParticles.GetComponent<ParticleSystem>().main;
                settings.startColor = particleColor;
                Instantiate(DeathParticles, this.transform.position, DeathParticles.transform.rotation);

                //SPAWN DEAD SQUARE OBJECT THAT GRAVITATES TOWARDS PLAYER.

                GameObject spawnedPickup = Instantiate(Pickup, this.transform.position, Quaternion.identity);
                spawnedPickup.GetComponent<Pickup>().Type = this.SType;
                Color pickupColor = _SR.color;
                pickupColor.a = 1;
                spawnedPickup.GetComponent<SpriteRenderer>().color = pickupColor;
                spawnedPickup.GetComponent<Rigidbody2D>().velocity = _RB.velocity;

                //DESTROY THE GAME OBJECT
                Destroy(this.gameObject);
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class PlayerSquareController : SquareController
{
    public float Regen = 0.01f;

    private void Awake()
    {
        _SR = GetComponent<SpriteRenderer>();
        _RB = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        var joint = this.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = Game.Player.GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (Health < 1)
        {
            Health += Regen * Time.deltaTime;
            this.UpdateTransparency();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            var squareType = collision.gameObject.GetComponent<Pickup>().Type;
            Game.PlayerInventory[squareType] += 1;
            Game.Player.GetComponent<PlayerController>().UpdateSquareCountUI();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Square"))
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

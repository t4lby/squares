using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Networking;

public abstract class Square : NetworkBehaviour
{
    /// <summary>
    /// The health of the square. from 0 to 1.
    /// </summary>
    protected float Health { get; set; }

    /// <summary>
    /// The squares durability. Responsible for deciding how much health is
    /// removed if the square is hit.
    /// </summary>
    protected float Durability { get; set; }

    /// <summary>
    /// Indicates whether the square is immune to all damage.
    /// </summary>
    protected bool Invincible { get; set; }

    /// <summary>
    /// Indicates whether the square regenrates it's health;
    /// </summary>
    public bool Regenerates { get; set; }

    /// <summary>
    /// The regeneration speed of the square. (only relevent if 
    /// <see cref="Regenerates"/> is true)
    /// </summary>
    protected float RegenerationSpeed { get; set; }

    /// <summary>
    /// The color of the square.
    /// </summary>
    public SquareType Color { get; set; }

    /// <summary>
    /// From 0 to 1, the minimum transparency the square gets to before death.
    /// </summary>
    protected float MinimumTransparency { get; set; }

    /// <summary>
    /// The player object that this square is part of.
    /// </summary>
    public Player Player { get; set; }

    /// <summary>
    /// The key for this square in the players build.
    /// </summary>
    public Vector3 PositionInPlayer { get; set; }

    public bool IsLocalPlayerSquare { get; set; }

    /// <summary>
    /// The factory responsible for the creation of this object.
    /// </summary>
    public Factory Factory { get; set; }

    /// <summary>
    /// Determines whether the squares behavior is triggered. ie a booster
    /// square that is currently <see cref=" Triggered"/> will be exerting a
    /// force. But for non functional squares this will do nothing.
    /// </summary>
    public bool Triggered { get; set; }

    /// <summary>
    /// If the square has a key mapping it is stored here.
    /// </summary>
    public KeyCode Mapping { get; set; }

    /// <summary>
    /// Indicates whether the squuare has a mapping.
    /// </summary>
    public bool Mapped { get; set; }

    protected abstract void SetSquareProperties();

    /// <summary>
    /// Overridable method for child classes to call update.
    /// </summary>
    protected virtual void UpdateSquare(){}

    /// <summary>
    /// Updates the squares transparency based on current health.
    /// </summary>
    protected void UpdateTransparency()
    {
        var spriteRenderer = this.GetComponent<SpriteRenderer>();
        var current = spriteRenderer.color;
        spriteRenderer.color = 
            new Color(current.r,
                      current.g,
                      current.b,
                      MinimumTransparency + Health * (1 - MinimumTransparency));
    }

    private void Update()
    {
        UpdateSquare();

        if (Regenerates & Health < 1)
        {
            Health += RegenerationSpeed * Time.deltaTime;
            this.UpdateTransparency();
        }
    }

    private void Awake()
    {
        this.SetSquareProperties();
        this.GetComponent<SpriteRenderer>().color = Game.GetColor(this.Color);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") ||
            collision.gameObject.CompareTag("Square")
            & !this.Invincible &&
            (this.Player == null | collision.gameObject.GetComponent<Square>().Player == null))
        {
            this.Health -= collision.relativeVelocity.magnitude / this.Durability;
            this.UpdateTransparency();
        }

        if (collision.gameObject.CompareTag("Pickup") & this.Player != null)
        {
            var color = collision.gameObject.GetComponent<Pickup>().Color;
            this.Player.PickupSquare(color);
            Destroy(collision.gameObject);
        }

        if (Health < 0)
        {
            Factory.DestroySquare(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            this.Health -= Time.deltaTime * Game.FireDamage / Durability;
            this.UpdateTransparency();
        }

        if (Health < 0)
        {
            Factory.DestroySquare(this);
        }
    }
}

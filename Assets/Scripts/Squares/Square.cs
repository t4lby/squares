using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class Square : MonoBehaviour
{
    /// <summary>
    /// The health of the square.
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
    protected bool Regenerates { get; set; }

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
    /// The player object that this square is part of. Only relevent if
    /// <see cref="IsPlayerSquare"/> is true.
    /// </summary>
    public Player Player { get; set; }

    /// <summary>
    /// The factory responsible for the creation of this object.
    /// </summary>
    public Factory Factory { get; set; }

    protected abstract void SetSquareProperties();

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

    private void Awake()
    {
        this.SetSquareProperties();
        this.GetComponent<SpriteRenderer>().color = Game.GetColor(this.Color);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Square") & !this.Invincible)
        {
            this.Health -= collision.relativeVelocity.magnitude / this.Durability;
            this.UpdateTransparency();

            if (Health < 0)
            {
                Factory.DestroySquare(this);
            }
        }

        if (collision.gameObject.CompareTag("Pickup") & this.Player != null)
        {
            var color = collision.gameObject.GetComponent<Pickup>().Type;
            this.Player.PickupSquare(color);
            Destroy(collision.gameObject);
        }
    }
}

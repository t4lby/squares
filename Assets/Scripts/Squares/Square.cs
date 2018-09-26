using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public abstract class Square : MonoBehaviour
{
    /// <summary>
    /// The health of the square. from 0 to 1.
    /// </summary>
    public float Health { get; set; }

    /// <summary>
    /// The squares durability. Responsible for deciding how much health is
    /// removed if the square is hit.
    /// </summary>
    protected float Durability { get; set; }

    /// <summary>
    /// Indicates whether the square is immune to all damage.
    /// </summary>
    public bool Invincible { get; set; }

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

    /// <summary>
    /// Flag for builds squares. Indicates whether the build square is currently
    /// snapped to another square in the scene.
    /// </summary>
    /// <value><c>true</c> if snapped; otherwise, <c>false</c>.</value>
    public bool Snapped { get; set; }

    /// <summary>
    /// The square that the build square is snapped to.
    /// </summary>
    public Square SnapTarget { get; set; }

    /// <summary>
    /// The squares this one is connected to.
    /// </summary>
    public List<Square> ConnectedTo { get; set; }

    /// <summary>
    /// Indicates whether the square is a 'build square', a UI placeholder
    /// for spawning squares.
    /// </summary>
    public bool IsBuildSquare { get; set; }

    /// <summary>
    /// Identifies the square in the level.
    /// </summary>
    public string Identifier { get; set; }

    protected abstract void SetSquareProperties();

    /// <summary>
    /// Overridable method for child classes to call update.
    /// </summary>
    protected virtual void UpdateSquare(){}

    /// <summary>
    /// Updates the squares transparency based on current health.
    /// </summary>
    public void UpdateTransparency()
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

    protected void Awake()
    {
        this.SetSquareProperties();
        this.ConnectedTo = new List<Square>();
        this.GetComponent<SpriteRenderer>().color = Game.GetColor(this.Color);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") 
            && !this.Invincible ||
            collision.gameObject.CompareTag("Square")
            && !this.Invincible &&
            (this.Player == null || collision.gameObject.GetComponent<Square>().Player == null))
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

        if (Health < 0 && !Invincible)
        {
            Factory.DestroySquare(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("BuildSquare"))
        {
            var buildSquare = other.GetComponent<Square>();
            buildSquare.Snapped = true;
            if (buildSquare.SnapTarget == null)
            {
                buildSquare.SnapTarget = this;
                buildSquare.Triggered = this.Player != null;
            }
            if (buildSquare.SnapTarget == this)
            {
                buildSquare.transform.parent = this.transform;
                buildSquare.transform.rotation = this.transform.rotation;
                buildSquare.transform.localPosition =
                               Tools.BestDirection(other.transform.localPosition)
                               * Game.SquareSize;
            }
            if (!Game.Players[0].Builder.JointTargets.Contains(this) /*&&
                Algorithms.AreInSameComponent(this, buildSquare.SnapTarget)*/)
            {
                Game.Players[0].Builder.JointTargets.Add(this);
            }
        }

        if (Health < 0 && !Invincible)
        {
            Factory.DestroySquare(this);
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BuildSquare"))
        {
            while(Game.Players[0].Builder.JointTargets.Remove(this));
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire")
            && !Invincible
            && !other.GetComponentInParent<Square>().IsBuildSquare)
        {
            this.Health -= Time.deltaTime * Game.FireDamage / Durability;
            this.UpdateTransparency();
        }
    }
}

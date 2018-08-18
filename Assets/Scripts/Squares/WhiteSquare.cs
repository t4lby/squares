using UnityEngine;
using System.Collections;

/// <summary>
/// Square type synonymous with a player, moves with basic forces 
/// according to input from axis.
/// </summary>
public class WhiteSquare : Square
{
    /// <summary>
    /// The acceleration of the square on player input.
    /// </summary>
    public float Acceleration { get; set; }

    private float _xInput;
    private float _yInput;

    /// <summary>
    /// All the magic numbers!!
    /// </summary>
    protected override void SetSquareProperties()
    {
        this.Health = 1;
        this.Durability = 10;
        this.Invincible = true;
        this.Color = SquareType.White;
        this.Acceleration = 20;
    }

    private void Update()
    {
        _xInput = Input.GetAxis("Horizontal");
        _yInput = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        this.GetComponent<Rigidbody>()
            .AddForce(new Vector3(_xInput, _yInput) * Acceleration);
    }
}

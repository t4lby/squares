using UnityEngine;
using System.Collections;

public class BlackSquare : Square
{
    // Use this for initialization
    protected override void SetSquareProperties()
    {
        this.Health = 1f;
        this.Durability = 50f;
        this.Color = SquareType.Black;
        this.MinimumTransparency = 0.25f;
        this.RegenerationSpeed = 0.1f;
        this.Invincible = true;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlueSquare : Square
{
    private Color _TriggeredColor = new Color(0,1,1);

    protected override void SetSquareProperties()
    {
        this.Health = 1;
        this.Durability = 5;
        this.Invincible = false;
        this.Color = SquareType.Blue;
        this.MinimumTransparency = 0.25f;
        this.RegenerationSpeed = 0.5f;
    }

    protected override void UpdateSquare()
    {
        if (this.Player != null)
        {
            this.ProcessTriggers();
        }
    }

    private void ProcessTriggers()
    {
        if (Input.GetKey(Mapping))
        {
            Tools.SetTriggerSurrounding(this, true);
        }
        if (Input.GetKeyUp(Mapping))
        {
            Tools.SetTriggerSurrounding(this, false);
        }

        if (this.Triggered)
        {
            this.GetComponent<SpriteRenderer>().color = _TriggeredColor;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = Game.GetColor(this.Color);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;

public class Player
{
    /// <summary>
    /// Each player will have their own UI;
    /// </summary>
    public UIController UI;

    public Inventory Inventory { get; set; }

    public Dictionary<Vector3,Square> Squares { get; set; }

    public void PickupSquare(SquareType color)
    {
        Inventory.Squares[color] += 1;

        UI.UpdateSquareCountUI(Inventory.Squares);
    }

    public Player()
    {
        this.Squares = new Dictionary<Vector3, Square>();
    }

    public Vector3 GetPosition()
    {
        var total = new Vector3();
        foreach (var pair in Squares)
        {
            total += pair.Value.transform.position;
        }
        return total / Squares.Count;
    }
}

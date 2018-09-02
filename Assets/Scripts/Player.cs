using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Runtime.InteropServices;
using System.Linq;

public class Player
{
    /// <summary>
    /// Each player will have their own UI;
    /// </summary>
    public UIController UI;

    public Inventory Inventory { get; set; }

    public RealtimeBuilder Builder { get; set; }

    /// <summary>
    /// The players build (should correspond to squares)
    /// </summary>
    public Build Build { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The square objects themselves.
    /// </summary>
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

    public void DropSmallestComponents()
    {
        Algorithms.Tarjan(this.Build);
        Dictionary<int, int> count = new Dictionary<int, int>();
        foreach (var pair in this.Build.Components)
        {
            count[pair.Value] = count.ContainsKey(pair.Value) ?
                                     count[pair.Value] + 1 : 1;
        }
        if (count.Count == 1)
        {
            return;
        }
        var largest = 0;
        foreach (var pair in count)
        {
            if (pair.Value > count[largest])
            {
                largest = pair.Key;
            }
        }
        var toDrop = this.Build.Components.Where((pair) => pair.Value != largest);
        var toKeep = this.Build.Components.Where((pair) => pair.Value == largest);
        foreach (var pair in toDrop)
        {
            this.Squares[pair.Key].Player = null;
            this.Squares.Remove(pair.Key);
            this.Build.RemoveFromAll(pair.Key);
        }
        if (toDrop.Count() > 0)
        {
            foreach (var pair in toKeep)
            {
                this.Squares[pair.Key].Triggered = false;
            }
        }
    }
}

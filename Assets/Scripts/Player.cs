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

    private bool Alive;
    private Vector3 _DeathPosition;

    /// <summary>
    /// The square objects themselves.
    /// </summary>
    public List<Square> Squares { get; set; }

    public void PickupSquare(SquareType color)
    {
        Inventory.Squares[color] += 1;

        UI.UpdateSquareCountUI(Inventory.Squares);
    }

    public Player()
    {
        this.Squares = new List<Square>();
        this.Alive = true;
    }

    public Vector3 GetPosition()
    {
        if (Squares.Count == 0 && this.Alive)
        {
            this._DeathPosition = this.Position;
            this.Alive = false;
        }
        var total = Vector3.zero;
        foreach (var square in Squares)
        {
            total += square.transform.position;
        }
        return Squares.Count == 0 ? _DeathPosition : total / Squares.Count;
    }

    public void DropSmallestComponents()
    {
        var components = Algorithms.Tarjan(this.Squares);
        Dictionary<int, int> count = new Dictionary<int, int>();
        foreach (var pair in components)
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
        var toDrop = components.Where((pair) => pair.Value != largest);
        var toKeep = components.Where((pair) => pair.Value == largest);
        foreach (var squareIntPair in toDrop)
        {
            squareIntPair.Key.Player = null;
            squareIntPair.Key.Regenerates = false;
            squareIntPair.Key.Triggered = false;
            this.Squares.Remove(squareIntPair.Key);
        }
        if (toDrop.Count() > 0)
        {
            foreach (var squareIntPair in toKeep)
            {
                squareIntPair.Key.Triggered = false;
            }
        }
    }
}

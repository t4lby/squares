using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    /// <summary>
    /// The colour of the Pickup.
    /// </summary>
    public SquareType Color { get; set; }


    private void FixedUpdate()
    {
        this.GravitateTowardsPlayers();
    }

    /// <summary>
    /// Gravitates pickup towards player objects. Currently has no
    /// multiplier.
    /// </summary>
    private void GravitateTowardsPlayers()
    {
        var rB = this.GetComponent<Rigidbody>();
        foreach(Player player in Game.Players)
        {
            var diff = player.Position - this.transform.position;
            rB.AddForce(diff / Mathf.Pow(diff.magnitude, 2));
        }
    }
}

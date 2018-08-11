using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    /// <summary>
    /// The colour of the Pickup.
    /// </summary>
    public SquareType Type { get; set; }

    /// <summary>
    /// A list of all player transforms present in the scene.
    /// </summary>
    public List<Transform> PlayerTransforms { get; set; }

	private void Start ()
    {
        PlayerTransforms.Add(Game.Player.transform);
	}
	
	private void Update ()
    {
		
	}

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
        var rB = this.GetComponent<Rigidbody2D>();
        foreach(Transform playerTransform in PlayerTransforms)
        {
            var diff = playerTransform.position - this.transform.position;
            rB.AddForce(diff / Mathf.Pow(diff.magnitude, 2));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    
    public float Acceleration;
    public UIController UI;

    private float _xInput;
    private float _yInput;
    private Rigidbody2D _RB;

	// Use this for initialization
	private void Start ()
    {
        Game.Player = this.gameObject;
        _RB = GetComponent<Rigidbody2D>();
        if (Game.PlayerInventory == null)
        {
            this.InitializePlayerInventory();
        }
	}
	
	// Update is called once per frame
	private void Update ()
    {
        _xInput = Input.GetAxis("Horizontal");
        _yInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("Editor");
        }
	}

    private void FixedUpdate()
    {
        _RB.AddForce(new Vector3(_xInput,_yInput) * Acceleration);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            var squareType = collision.gameObject.GetComponent<Pickup>().Type;
            Game.PlayerInventory[squareType] += 1;
            this.UpdateSquareCountUI();
            Destroy(collision.gameObject);
        }
    }

    public void UpdateSquareCountUI()
    {
        UI.UpdateSquareCountUI(Game.PlayerInventory);
    }

    /// <summary>
    /// Initializes the players inventory. For All square types setting values to 0.
    /// </summary>
    private void InitializePlayerInventory()
    {
        Game.PlayerInventory = new Dictionary<SquareType, int>();
        foreach (SquareType square in Enum.GetValues(typeof(SquareType)))
        {
            Game.PlayerInventory.Add(square, 0);
        }
    }
}

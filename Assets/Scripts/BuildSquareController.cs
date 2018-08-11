using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSquareController : MonoBehaviour {

    public SquareType SType;

	void Start ()
    {
        GetComponent<SpriteRenderer>().color = Game.GetColor(SType);
	}
	
}

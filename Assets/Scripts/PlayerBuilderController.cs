using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuilderController : MonoBehaviour {

    private SquareSelector _Selector;
	
	void Start () {
        _Selector = GetComponent<SquareSelector>();
        Assemble(Game.PlayerBuild);
	}
	
	void Update () {
		
	}

    private void Assemble(Dictionary<Vector3, SquareType> build)
    {
        foreach (var item in build)
        {
            var square = Instantiate(_Selector.GetPrefab(item.Value),
                                    item.Key * Game.SquareSize,
                                    Quaternion.identity);
        }
    }
}

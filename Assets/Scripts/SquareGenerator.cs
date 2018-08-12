using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class SquareGenerator : MonoBehaviour {

    public float spawnDiff;
    public float MinSpawnRadius;
    public float MaxSpawnRadius;
    private float nextSpawn;

    private SquareSelector _Selector;


	void Start ()
    {
        _Selector = GetComponent<SquareSelector>();
        nextSpawn = 0;
	}
	
	void Update ()
    {
        if (Time.time > nextSpawn)
        {
            //RandomInCircle(Game.Player.transform.position, MinSpawnRadius, MaxSpawnRadius, true);
            nextSpawn = Time.time + spawnDiff;
        }
	}

    private void SpawnSquare(SquareType sType, Vector3 position, Quaternion rotation)
    {
        Instantiate(_Selector.GetPrefab(sType), position, rotation);
    }

    private void RandomInCircle(Vector3 centre, float minRadius, float maxRadius, bool randomRotation)
    {
        var rotation = Quaternion.identity;
        if (randomRotation)
        {
            rotation.eulerAngles = new Vector3(0,0,Random.Range(0,90));
        }

        SquareType sType = Game.ActiveSquareTypes[Random.Range(0, Game.ActiveSquareTypes.Count)];

        var angle = Random.Range(0, 360);
        var position = centre + Random.Range(minRadius, maxRadius) * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        SpawnSquare(sType, position, rotation);
    }
}

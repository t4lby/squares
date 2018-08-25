using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Player _Player;

	void Start () 
    {
        var inv = new Inventory();
        inv.Squares[SquareType.Purple] = 50;
        inv.Squares[SquareType.Blue] = 20;
        inv.Squares[SquareType.Yellow] = 10;
        inv.Squares[SquareType.Red] = 10;
        var build = new Build();
        build.Squares[Vector3.zero] = SquareType.White;
        build.Squares[Vector3.up] = SquareType.Blue;
        build.Mappings[Vector3.up] = KeyCode.I;
        build.Squares[Vector3.left] = SquareType.Blue;
        build.Mappings[Vector3.left] = KeyCode.L;
        build.Squares[Vector3.right] = SquareType.Blue;
        build.Mappings[Vector3.right] = KeyCode.J;
        build.Squares[Vector3.right + Vector3.down] = SquareType.Purple;
        build.Squares[Vector3.left + Vector3.down] = SquareType.Purple;
        build.Squares[Vector3.up * 2] = SquareType.Yellow;
        _Player = Game.Factory.SpawnPlayer(build, inv, Vector3.zero);
        Game.Players.Add(_Player);
    }

    private void Update()
    {
        transform.position = _Player.Position;
    }
}

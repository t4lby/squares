using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    private Player _Player;
    private bool _Spawned = false;



    private void Update()
    {
        if (!_Spawned && Game.Factory != null)
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
            _Player = Game.Factory.SpawnPlayer(build, inv, Vector3.zero, isLocal: true);
            Game.Players.Add(_Player);
            _Spawned = true;
        }
        //transform.position = _Player.Position;
    }
}

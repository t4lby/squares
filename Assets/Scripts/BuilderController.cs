using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;

public class BuilderController : MonoBehaviour {

    public GameObject BuildSquarePrefab; 

    public SquareType SelectedSquare;

    private Dictionary<SquareType, int> invStub;
    private List<GameObject> _BuildSquares;

	private void Start ()
    {
        _BuildSquares = new List<GameObject>();
        SelectedSquare = SquareType.Green;


        //TESTING STUB VALUES
        Game.PlayerBuild[Vector3.zero] = SquareType.White;
        Assemble(Game.PlayerBuild);

        invStub = new Dictionary<SquareType, int>();

        invStub[SquareType.Red] = 2;
        invStub[SquareType.Green] = 4;
	}
	
	private void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 gridPoint = GridSnap2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Game.SquareSize);
            if (gridPoint != Vector3.zero)
            {
                Game.PlayerBuild[gridPoint] = SelectedSquare;
            }
            //BAD dissasembles and reassembles everytime.
            //EVEN BADDER, adds a white player square and removes it every time.
            Game.PlayerBuild[Vector3.zero] = SquareType.White;
            Dismantle();
            Assemble(Game.PlayerBuild);
            Game.PlayerBuild.Remove(Vector3.zero);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("Test");
        }
	}

    /// <summary>
    /// Returns the nearest grid point to <paramref name="input"/>.
    /// </summary>
    private Vector3 GridSnap2D(Vector3 input, float gridSize)
    {
        int snapX = Mathf.RoundToInt(input.x / gridSize);
        int snapY = Mathf.RoundToInt(input.y / gridSize);

        return new Vector3(snapX, snapY, 0);
    }

    private void Dismantle()
    {
        foreach (GameObject buildSquare in _BuildSquares)
        {
            Destroy(buildSquare);
        }
    }

    private void Assemble(Dictionary<Vector3, SquareType> build)
    {
        foreach (var item in build)
        {
            var square = Instantiate(BuildSquarePrefab,
                                    item.Key * Game.SquareSize,
                                    Quaternion.identity);
            square.GetComponent<SpriteRenderer>().color = Game.GetColor(item.Value);
            _BuildSquares.Add(square);
            //var Joint = square.AddComponent<FixedJoint2D>();
            //Joint.connectedBody = Game.Player.GetComponent<Rigidbody2D>();
        }
    }
}

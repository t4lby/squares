using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class BuilderController : MonoBehaviour {

    public GameObject BuildSquarePrefab; 

    public SquareType SelectedSquare;

    private bool _Erase;

    public UIController UI;

    private Player _Player;
    private List<GameObject> _BuildSquares;

	private void Start ()
    {
        if (Game.Players.Count > 0)
        {
            _Player = Game.Players[0];
        }
        else
        {
            _Player = new Player();
            Game.Players.Add(_Player);
            _Player.Build = new Build();
            _Player.Inventory = new Inventory();
            _Player.Build.Squares.Add(Vector3.zero, SquareType.White);
        }

        _BuildSquares = new List<GameObject>();
        SelectedSquare = SquareType.Green;

        UI.UpdateSquareCountUI(_Player.Inventory.Squares);
        this.UpdateUIButtons(UI);

        Assemble(_Player.Build.Squares);
	}
	
	private void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 gridPoint = 
                GridSnap2D(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                           Game.SquareSize);
            if (_Erase)
            {
                if (_Player.Build.Squares.ContainsKey(gridPoint))
                {
                    var color = _Player.Build.Squares[gridPoint];
                    _Player.Inventory.Squares[color] += 1;
                    _Player.Build.Squares.Remove(gridPoint);
                }
            }
            else if (_Player.Inventory.Squares[SelectedSquare] > 0 &&
                (IsNextToSquare(gridPoint) | _Player.Build.Squares.Count == 0))
            {
                _Player.Build.Squares[gridPoint] = SelectedSquare;
                _Player.Inventory.Squares[SelectedSquare] -= 1;
            }

            UI.UpdateSquareCountUI(_Player.Inventory.Squares);
            this.UpdateUIButtons(UI);
            Dismantle();
            Assemble(_Player.Build.Squares);
        }
        if (Input.GetKeyDown(KeyCode.T))
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

    private void Assemble(Dictionary<Vector3, SquareType> squares)
    {
        foreach (var item in squares)
        {
            var square = Instantiate(BuildSquarePrefab,
                                    item.Key * Game.SquareSize,
                                    Quaternion.identity);
            square.GetComponent<SpriteRenderer>().color = Game.GetColor(item.Value);
            _BuildSquares.Add(square);
        }
    }

    private bool IsNextToSquare(Vector3 position)
    {
        var directions = new List<Vector3>
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };
        foreach (var d in directions)
        {
            if (_Player.Build.Squares.ContainsKey(position+d))
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateUIButtons(UIController uI)
    {
        foreach(var squareCount in uI.SquareCounts)
        {
            var button = squareCount.Value.GetComponent<Button>();
            button.onClick.AddListener(delegate { SelectSquare(squareCount.Key); });
        }
    }

    public void SetErase(bool value)
    {
        _Erase = value;
    }

    public void SelectSquare(SquareType color)
    {
        SelectedSquare = color;
        SetErase(false);
    }
}

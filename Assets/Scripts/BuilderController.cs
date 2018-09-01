using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System;

public enum Tool 
{
    Select,
    Build,
    Erase,
    Rotate,
    Assign,
    WaitForKeyPress
}

public class BuilderController : MonoBehaviour {

    public GameObject BuildSquarePrefab; 

    public SquareType SelectedSquare;

    private Tool _Tool;

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
        _Tool = Tool.Select;
        SelectedSquare = SquareType.Green;

        UI.UpdateSquareCountUI(_Player.Inventory.Squares);
        this.UpdateUIButtons(UI);

        Assemble(_Player.Build);
	}
	
	private void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 gridPoint = 
                GridSnap2D(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                           Game.SquareSize);

            switch (_Tool)
            {
                case (Tool.Erase):
                    if (_Player.Build.Squares.ContainsKey(gridPoint))
                    {
                        var color = _Player.Build.Squares[gridPoint];
                        _Player.Inventory.Squares[color] += 1;
                        _Player.Build.Squares.Remove(gridPoint);
                    }
                    break;
                case (Tool.Select):
                    if (_Player.Inventory.Squares[SelectedSquare] > 0 &
                        !_Player.Build.Squares.ContainsKey(gridPoint) &
                        (IsNextToSquare(gridPoint) | _Player.Build.Squares.Count == 0))
                    {
                        _Player.Build.Squares[gridPoint] = SelectedSquare;
                        _Player.Inventory.Squares[SelectedSquare] -= 1;
                    }
                    break;
                case (Tool.Rotate):
                    if (_Player.Build.Squares.ContainsKey(gridPoint))
                    {
                        _Player.Build.Rotations[gridPoint] = 
                            _Player.Build.Rotations.ContainsKey(gridPoint) ?
                            Rotate2D(_Player.Build.Rotations[gridPoint], 90) :
                            Rotate2D(Quaternion.identity, 90);
                    }
                    break;
                case(Tool.Assign):
                    if (_Player.Build.Squares.ContainsKey(gridPoint) &&
                        _Player.Build.Squares[gridPoint] == SquareType.Blue)
                    {
                        StartCoroutine(MapKeyFromUser(gridPoint));
                    }
                    break;
            }

            UI.UpdateSquareCountUI(_Player.Inventory.Squares);
            this.UpdateUIButtons(UI);
            Dismantle();
            Assemble(_Player.Build);
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

    private void Assemble(Build build)
    {
        foreach (var item in build.Squares)
        {
            var rotation = build.Rotations.ContainsKey(item.Key) ?
                                build.Rotations[item.Key] :
                                Quaternion.identity;
            var square = Instantiate(BuildSquarePrefab,
                                    item.Key * Game.SquareSize,
                                     rotation);
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
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { SelectSquare(squareCount.Key); });
        }
    }

    public void SetErase()
    {
        _Tool = Tool.Erase;
    }

    public void SetRotate()
    {
        _Tool = Tool.Rotate;
    }

    public void SetAssign()
    {
        _Tool = Tool.Assign;
    }

    public void SelectSquare(SquareType color)
    {
        _Tool = Tool.Select;
        SelectedSquare = color;
    }

    private Quaternion Rotate2D(Quaternion q, int a)
    {
        var eulers = q.eulerAngles;
        eulers.z += a;
        q.eulerAngles = eulers;
        return q;
    }

    IEnumerator WaitForKeyPress()
    {
        while (!Input.anyKeyDown)
        {
            Debug.Log("waiting for key");
            yield return null;
        }
    }

    private IEnumerator MapKeyFromUser(Vector3 gridPoint)
    {
        while (!Input.anyKeyDown | Input.GetKey(KeyCode.Mouse0))
        {
            yield return null;
        }
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                _Player.Build.Mappings[gridPoint] = kcode;
                Debug.Log(kcode);
            }
        }
    }
}

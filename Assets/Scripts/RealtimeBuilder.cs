using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

public enum Tool
{
    Erase,
    Rotate,
    Assign,
    Select,
    Build
}

/// <summary>
/// Uses player input to spawn squares via the factory.
/// </summary>
public class RealtimeBuilder : MonoBehaviour
{

    private float _SnapThreshold = 0.5f;

    private Vector3 BuildSquareColliderSize1 = new Vector3(0.7f,
                                                          0.4f,
                                                          1f);
    private Vector3 BuildSquareColliderSize2 = new Vector3(0.4f,
                                                          0.7f,
                                                          1f);

    public Factory Factory { get; set; }

    public Player Player { get; set; }

    public List<Square> JointTargets { get; set; }

    public SquareType SelectedSquare;

    private float SelectorRadius = 0.025f;

    private Square _BuildSquare;

    public GameObject SelectorPrefab;

    private Selector _Selector;

    private Tool _Tool;

    private Vector3 _DragStartPos;

    private bool _DraggingRotation;

    public UIController UI;

    private void Start()
    {
        UI.UpdateListeners(SelectSquare,
                           SetErase,
                           SetRotate,
                           SetAssign);
        SelectedSquare = SquareType.Green;
        this.JointTargets = new List<Square>();
    }

    private void Update ()
    {
        if (_Selector != null)
        {
            _Selector.transform.position = Tools.GetMousePositionInScene();
        }
        if (_BuildSquare != null)
        {
            var mousePosition = Tools.GetMousePositionInScene();
            if ((_BuildSquare.transform.position - mousePosition).magnitude > _SnapThreshold
                && !_DraggingRotation)
            {
                _BuildSquare.Snapped = false;
                _BuildSquare.Triggered = false;
                _BuildSquare.SnapTarget = null;
                _BuildSquare.transform.parent = null;
            }
            if (_BuildSquare.Snapped == false)
            {
                _BuildSquare.transform.position = mousePosition;
            }
            if (_DraggingRotation && _BuildSquare.Snapped)
            {
                var dragDirection = mousePosition - _BuildSquare.transform.position;
                var dragRotation = Quaternion.LookRotation(Vector3.forward, dragDirection);
                var zDiff = (_BuildSquare.SnapTarget.transform.rotation.eulerAngles.z - dragRotation.eulerAngles.z+405)%360;
                var rotation = new Quaternion();
                rotation.eulerAngles = new Vector3(0, 0,
                                _BuildSquare.SnapTarget.transform.rotation.eulerAngles.z -
                                                   (zDiff - (zDiff % 90)));
                _BuildSquare.transform.rotation = rotation;
                                
            }
            _BuildSquare.Health = 0.5f + Mathf.Sin(Time.time * 5) / 2;
            _BuildSquare.UpdateTransparency();
        }


        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            switch (_Tool)
            {
                case (Tool.Erase):
                    if (_Selector.Target != null && _Selector.Target.Player != null)
                    {
                        Factory.DestroySquare(_Selector.Target);
                    }
                    break;
                case (Tool.Build):
                    if (Player.Inventory.Squares[SelectedSquare] > 0)
                    {
                        _DraggingRotation = true;
                        _DragStartPos = Tools.GetMousePositionInScene();
                    }
                    break;
                case (Tool.Rotate):
                    break;
                case (Tool.Assign):
                    if (_Selector.Target != null && _Selector.Target.Player != null)
                    {
                        StartCoroutine(MapKeyFromUser(_Selector.Target.PositionInPlayer));
                    }
                    break;
            }
        }
        if (Input.GetMouseButtonUp(0) && !IsPointerOverUIObject())
        {
            switch (_Tool)
            {
                case (Tool.Build):
                    if (Player.Inventory.Squares[SelectedSquare] > 0)
                    {
                        if (PlaceBuildSquare())
                        {
                            Player.Inventory.Squares[SelectedSquare] -= 1;
                        }
                        UI.UpdateSquareCountUI(Player.Inventory.Squares);
                    }
                    _DraggingRotation = false;
                    break;
            }
        }
	}

    private void UpdateUIListeners(UIController uI)
    {
        foreach (var squareCount in uI.SquareCounts)
        {
            var button = squareCount.Value.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { SelectSquare(squareCount.Key); });
        }
        uI.Erase.onClick.RemoveAllListeners();
        uI.Erase.onClick.AddListener(SetErase);
        uI.Rotate.onClick.RemoveAllListeners();
        uI.Rotate.onClick.AddListener(SetRotate);
        uI.Assign.onClick.RemoveAllListeners();
        uI.Assign.onClick.AddListener(SetAssign);
    }

    public void SetErase()
    {
        DestroyBuildSquare();
        _Tool = Tool.Erase;
        if (_Selector == null)
        {
            _Selector = SpawnSelector();
        }
    }

    public void SetRotate()
    {
        DestroyBuildSquare();
        _Tool = Tool.Rotate;
        if (_Selector == null)
        {
            _Selector = SpawnSelector();
        }
    }

    public void SetAssign()
    {
        DestroyBuildSquare();
        _Tool = Tool.Assign;
        if (_Selector == null)
        {
            _Selector = SpawnSelector();
        }
    }

    public void SelectSquare(SquareType color)
    {
        _Tool = Tool.Build;
        SelectedSquare = color;
        DestroyBuildSquare();
        _BuildSquare = SpawnBuildSquare();
    }

    private Square SpawnBuildSquare()
    {
        var buildSquare = Factory.SpawnSquare(SelectedSquare,
                                           Tools.GetMousePositionInScene(),
                                           Vector3.zero,
                                           Quaternion.identity);
        buildSquare.GetComponent<Collider>().isTrigger = true;
        buildSquare.GetComponent<BoxCollider>().size = BuildSquareColliderSize1;
        var collider2 = buildSquare.gameObject.AddComponent<BoxCollider>();
        collider2.size = BuildSquareColliderSize2;
        collider2.isTrigger = true;
        buildSquare.Invincible = true;
        buildSquare.tag = "BuildSquare";
        buildSquare.IsBuildSquare = true;
        return buildSquare;
    }

    private Selector SpawnSelector()
    {
        var selector = Instantiate(SelectorPrefab);
        selector.name = "Selector";
        var selectorCollider = selector.AddComponent<SphereCollider>();
        selectorCollider.radius = SelectorRadius;
        selectorCollider.isTrigger = true;
        return selector.AddComponent<Selector>();
    }

    private void DestroyBuildSquare()
    {
        if (_BuildSquare != null)
        {
            Factory.SpawnedSquares.Remove(_BuildSquare);
            Destroy(_BuildSquare.gameObject);
            _BuildSquare = null;
        }
    }

    private Quaternion Rotate2D(Quaternion q, int a)
    {
        var eulers = q.eulerAngles;
        eulers.z += a;
        q.eulerAngles = eulers;
        return q;
    }

    /// <summary>
    /// Attempts to place a square where the build square is currently.
    /// </summary>
    /// <returns><c>true</c>, if build square was placed, <c>false</c> otherwise.</returns>
    private bool PlaceBuildSquare()
    {
        //prevent from building on top of an existing square.
        foreach (var square in this.JointTargets)
        {
            if (square.transform.position == _BuildSquare.transform.position)
            {
                //play some sort of sound indicating can't place.
                return false;
            }
        }
        var spawnedSquare = Factory.SpawnSquare(SelectedSquare,
                                                _BuildSquare.transform.position,
                                                Vector3.zero,
                                                _BuildSquare.transform.rotation);
        foreach (var square in this.JointTargets)
        {
            Factory.FixSquares(square, spawnedSquare);
            Factory.FixSquares(spawnedSquare, square);
            if (square.Player != null)
            {
                spawnedSquare.Player = square.Player;
                spawnedSquare.Regenerates = true;
                spawnedSquare.transform.parent = square.transform;
                var posInPlayer = Game.RoundVectorToInt(square.PositionInPlayer +
                                                        spawnedSquare.transform.localPosition / Game.SquareSize);
                spawnedSquare.Player.Squares[posInPlayer] = spawnedSquare;
                spawnedSquare.Player.Build.Squares[posInPlayer] = SelectedSquare;
                spawnedSquare.PositionInPlayer = posInPlayer;
                spawnedSquare.transform.parent = null;
            }
        }
        return true;
    }

    private bool IsPointerOverUIObject()
    {
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    IEnumerator WaitForKeyPress()
    {
        while (!Input.anyKeyDown)
        {
            Debug.Log("waiting for key");
            yield return null;
        }
    }

    private IEnumerator MapKeyFromUser(Vector3 posInPlayer)
    {
        while (!Input.anyKeyDown | Input.GetKey(KeyCode.Mouse0))
        {
            yield return null;
        }
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                Player.Build.Mappings[posInPlayer] = kcode;
                Player.Squares[posInPlayer].Mapped = true;
                Player.Squares[posInPlayer].Mapping = kcode;
                Debug.Log(kcode);
            }
        }
    }
}

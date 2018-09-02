using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Uses player input to spawn squares via the factory.
/// </summary>
public class RealtimeBuilder : MonoBehaviour {

    private float _SnapThreshold = 0.5f;

    private Vector3 BuildSquareColliderSize = new Vector3(0.6f,
                                                          0.6f,
                                                          1f);

    public Factory Factory { get; set; }

    public SquareType SelectedSquare;

    private Square _BuildSquare;

    private Square _SnappedSquare;

    private Tool _Tool;

    public UIController UI;

    private Player _Player;


    private void Start()
    {
        AddListenersToUI(UI);
        SelectedSquare = SquareType.Green;
    }

    private void Update ()
    {
        if (_BuildSquare != null)
        {
            if ((_BuildSquare.transform.position - UITools.GetMousePositionInScene()).magnitude > _SnapThreshold)
            {
                _BuildSquare.Snapped = false;
                _BuildSquare.SnapTarget = null;
                _BuildSquare.transform.parent = null;
            }
            if (_BuildSquare.Snapped == false)
            {
                _BuildSquare.transform.position = UITools.GetMousePositionInScene();
            }
            _BuildSquare.Health = 0.5f + Mathf.Sin(Time.time * 5) / 2;
            _BuildSquare.UpdateTransparency();
        }

        if (Input.GetMouseButtonDown(0))
        {
            switch (_Tool)
            {
                case (Tool.Erase):
                    break;
                case (Tool.Build):
                    var spawnedSquare = Factory.SpawnSquare(SelectedSquare,
                                                            _BuildSquare.transform.position,
                                                            Vector3.zero,
                                                            _BuildSquare.transform.rotation);
                    foreach (var square in Factory.SpawnedSquares)
                    {
                        if (square.IsJointTarget)
                        {
                            Factory.FixSquares(square, spawnedSquare);
                        }
                    }
                    break;
                case (Tool.Rotate):
                    break;
                case (Tool.Assign):
                    break;
            }

        }
	}

    private void AddListenersToUI(UIController uI)
    {
        foreach (var squareCount in uI.SquareCounts)
        {
            var button = squareCount.Value.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { SelectSquare(squareCount.Key); });
        }
        uI.Erase.onClick.RemoveAllListeners();
        uI.Erase.onClick.AddListener(delegate { SetErase(); });
        uI.Rotate.onClick.RemoveAllListeners();
        uI.Rotate.onClick.AddListener(delegate { SetRotate(); });
        uI.Assign.onClick.RemoveAllListeners();
        uI.Assign.onClick.AddListener(delegate { SetAssign(); });
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
        _Tool = Tool.Build;
        SelectedSquare = color;
        if(_BuildSquare != null)
        {
            Destroy(_BuildSquare);
        }
        _BuildSquare = Factory.SpawnSquare(SelectedSquare,
                                           UITools.GetMousePositionInScene(),
                                           Vector3.zero,
                                           Quaternion.identity);
        _BuildSquare.GetComponent<Collider>().isTrigger = true;
        _BuildSquare.GetComponent<BoxCollider>().size = BuildSquareColliderSize;
        _BuildSquare.tag = "BuildSquare";
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

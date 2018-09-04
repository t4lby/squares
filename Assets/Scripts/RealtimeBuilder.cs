﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;

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

    private Square _BuildSquare;

    private Square _SnappedSquare;

    private Tool _Tool;

    public UIController UI;

    private void Start()
    {
        AddListenersToUI(UI);
        SelectedSquare = SquareType.Green;
        this.JointTargets = new List<Square>();
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
                    if (!IsPointerOverUIObject() &&
                        Player.Inventory.Squares[SelectedSquare] > 0)
                    {
                        PlaceBuildSquare();
                        Player.Inventory.Squares[SelectedSquare] -= 1;
                        UI.UpdateSquareCountUI(Player.Inventory.Squares);
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
            Factory.SpawnedSquares.Remove(_BuildSquare);
            Destroy(_BuildSquare.gameObject);
        }
        _BuildSquare = Factory.SpawnSquare(SelectedSquare,
                                           UITools.GetMousePositionInScene(),
                                           Vector3.zero,
                                           Quaternion.identity);
        _BuildSquare.GetComponent<Collider>().isTrigger = true;
        _BuildSquare.GetComponent<BoxCollider>().size = BuildSquareColliderSize1;
        var collider2 = _BuildSquare.gameObject.AddComponent<BoxCollider>();
        collider2.size = BuildSquareColliderSize2;
        collider2.isTrigger = true;
        _BuildSquare.Invincible = true;
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

    private void PlaceBuildSquare()
    {
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
                //may need to fire down chain of interconnected squares in future.
                spawnedSquare.Player = square.Player;
                spawnedSquare.Regenerates = true;
                spawnedSquare.transform.parent = square.transform;
                var posInPlayer = square.PositionInPlayer +
                                spawnedSquare.transform.localPosition / Game.SquareSize;
                spawnedSquare.Player.Squares[posInPlayer] = spawnedSquare;
                spawnedSquare.PositionInPlayer = posInPlayer;
                spawnedSquare.transform.parent = null;
            }
        }
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
}

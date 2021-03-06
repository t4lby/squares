﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    public GameObject SquareCountPrefab;
    public Button Erase;
    public Button Assign;
    public Button Restart;
    public GameObject GameOverUI;
    public Vector3 FirstCountPosition;
    public Vector3 DiffCountPosition;
    public Dictionary<SquareType, GameObject> SquareCounts;

    public delegate void SelectSquare(SquareType color);
    public delegate void SetTool();

    private SelectSquare _SelectSquareMethod;

    private void Start()
    {
        GameOverUI.SetActive(false);
        Restart.onClick.AddListener(delegate { RestartLevel(); });
    }


    private void RestartLevel()
    {
        Game.Players.Clear();
        SceneManager.LoadScene(0);
    }

    public void UpdateListeners(SelectSquare selectSquare,
                                 SetTool setErase,
                                 SetTool setAssign)
    {
        UpdateSquareListeners(selectSquare);
        this.Erase.onClick.RemoveAllListeners();
        this.Erase.onClick.AddListener(delegate { setErase(); });
        this.Assign.onClick.RemoveAllListeners();
        this.Assign.onClick.AddListener(delegate { setAssign(); });
        _SelectSquareMethod = selectSquare;
    }

    private void UpdateSquareListeners(SelectSquare selectSquare)
    {
        foreach (var squareCount in this.SquareCounts)
        {
            var button = squareCount.Value.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { selectSquare(squareCount.Key); });
        }
    }

    public void UpdateSquareCountUI(Dictionary<SquareType, int> inventory)
    {
        if (SquareCounts == null)
        {
            SquareCounts = new Dictionary<SquareType, GameObject>();
        }
        //count is strictly for positioning.
        int count = 0;
        foreach (SquareType sType in Enum.GetValues(typeof(SquareType)))
        {
            if (inventory[sType] > 0)
            {
                if (!SquareCounts.ContainsKey(sType))
                {
                    GameObject countObject = Instantiate(SquareCountPrefab);
                    countObject.transform.SetParent(this.transform, worldPositionStays: false);
                    countObject.GetComponentInChildren<Image>().color = Game.GetColor(sType);
                    SquareCounts[sType] = countObject;
                }
                SquareCounts[sType].GetComponentInChildren<Text>().text = inventory[sType].ToString();
                SquareCounts[sType].GetComponent<RectTransform>().anchoredPosition3D =
                    FirstCountPosition + count * DiffCountPosition;
                count += 1;
            }
            else 
            {
                if (SquareCounts.ContainsKey(sType))
                {
                    Destroy(SquareCounts[sType]);
                    SquareCounts.Remove(sType);
                }
            }
        }
        if (_SelectSquareMethod != null)
        {
            UpdateSquareListeners(_SelectSquareMethod);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public GameObject SquareCountPrefab;
    public Vector3 FirstCountPosition;
    public Vector3 DiffCountPosition;

    private Dictionary<SquareType, GameObject> SquareCounts;

    private void Start()
    {

    }

    private void Update()
    {

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
    }


}

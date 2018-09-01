using System;
using UnityEngine;

public static class UITools
{
    public static Vector3 GetMousePositionInScene()
    {
        return new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                           Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                           0);
    }
}    
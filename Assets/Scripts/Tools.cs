using System;
using UnityEngine;

public static class Tools
{
    public static Vector3 GetMousePositionInScene()
    {
        return new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                           Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
                           0);
    }

    public static Vector3 BestDirection(Vector3 origin, Vector3 target)
    {
        return BestDirection((target - origin).normalized);
    }

    public static Vector3 BestDirection(Vector3 targetVector)
    {
        Vector3 goalVector = targetVector.normalized;
        Vector3 bestDir = Vector3.zero;
        float minAngle = 1000f;
        foreach (var direction in Game.Directions)
        {
            var angle = Mathf.Abs(Vector3.Angle(goalVector, direction));
            if (angle < minAngle)
            {
                minAngle = angle;
                bestDir = direction;
            }
        }
        return bestDir;
    }

    public static void SetTriggerSurrounding(Square square, bool value)
    {
        square.Triggered = value;
        foreach (var connectedSquare in square.ConnectedTo)
        {
            connectedSquare.Triggered = value;
        }
    }
}    
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Algorithms
{
    /// <summary>
    /// Popolates the component part of a specific build.
    /// </summary>
    public static void Tarjan(Build build)
    {
        build.Components = new Dictionary<Vector3, int>();

        int i = 0;
        foreach (var buildPoint in build.Squares)
        {
            if (!build.Components.ContainsKey(buildPoint.Key))
            {
                CheckNode(buildPoint.Key, build, i);
                i++;
            }
        }
    }

    private static void CheckNode(Vector3 node, Build build, int componentNo)
    {
        build.Components[node] = componentNo;
        foreach (var direction in Game.Directions)
        {
            if (build.Squares.ContainsKey(node + direction) &
                !build.Components.ContainsKey(node + direction))
            {
                CheckNode(node + direction, build, componentNo);
            }
        }
    }

    public static Dictionary<Square, int> Tarjan(List<Square> squares)
    {
        var components = new Dictionary<Square, int>();

        int i = 0;
        foreach (var square in squares)
        {
            if (!components.ContainsKey(square))
            {
                CheckNode(square, components, i);
                i++;
            }
        }
        return components;
    }

    private static void CheckNode(Square square,
                                  Dictionary<Square, int> components,
                                  int componentNo)
    {
        components[square] = componentNo;
        foreach (var connectedSquare in square.ConnectedTo)
        {
            if (!components.ContainsKey(connectedSquare))
            {
                CheckNode(connectedSquare, components, componentNo);
            }
        }
    }

    public static bool AreInSameComponent(Square SquareA, Square SquareB)
    {
        if (SquareA == SquareB)
        {
            return true;
        }
        var toCheck = SquareA.ConnectedTo.ToList();
        var checkedSquares = new List<Square>();
        while (toCheck.Count > 0)
        {
            var stateBeforeCheck = toCheck.ToArray();
            var toAdd = new List<Square>();
            foreach (Square s in toCheck)
            {
                checkedSquares.Add(s);
                if (s == SquareB)
                {
                    return true;
                }
                foreach (var t in s.ConnectedTo)
                {
                    if (!checkedSquares.Contains(t))
                    {
                        toAdd.Add(t);
                    }
                }
            }
            toCheck.AddRange(toAdd);
            foreach (Square t in stateBeforeCheck)
            {
                toCheck.Remove(t);
            }
        }
        return false;
    }
}


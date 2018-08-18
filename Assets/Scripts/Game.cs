using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Actual colors and squares are defined below
/// </summary>
public static class Game
{
    public const float SquareSize = 0.5f;
    public const float DeathParticleTransparency = 0.5f;
    public static List<Player> Players = new List<Player>();
    public static Dictionary<Vector3, SquareType> PlayerBuild = new Dictionary<Vector3, SquareType>();
    public static Dictionary<SquareType, int> PlayerInventory;
    public static UIController UI;

    public static List<SquareType> ActiveSquareTypes = new List<SquareType>
    { 
        SquareType.Green,
        SquareType.Blue,
        SquareType.Purple,
        SquareType.Yellow
    };

    public static Vector3[] Directions =
    {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };  



    /*-----------------------------------------------------------------------
     * Define colors
     *----------------------------------------------------------------------*/
    public static Color White = new Color(1,1,1);
    public static Color Black = new Color(0,0,0);
    public static Color Grey = new Color(0.5f, 0.5f, 0.5f);
    public static Color Yellow = new Color(1,1,0);
    public static Color Red = new Color(1,0,0);
    public static Color Blue = new Color(0,0,1);
    public static Color Green = new Color(0,1,0);
    public static Color Brown = new Color(0.75f, 0.5f, 0.25f);
    public static Color Pink = new Color(1, 0.5f, 0.75f);
    public static Color Orange = new Color(1, 0.5f, 0);
    public static Color Purple = new Color(0.75f, 0, 1);

    public static Color GetColor(SquareType sType)
    {
        switch (sType)
        {
            case SquareType.White:
                return White;
            case SquareType.Black:
                return Black;
            case SquareType.Grey:
                return Grey;
            case SquareType.Yellow:
                return Yellow;
            case SquareType.Red:
                return Red;
            case SquareType.Blue:
                return Blue;
            case SquareType.Green:
                return Green;
            case SquareType.Brown:
                return Brown;
            case SquareType.Pink:
                return Pink;
            case SquareType.Orange:
                return Orange;
            case SquareType.Purple:
                return Purple;
            default:
                return White;
        }
    }
}

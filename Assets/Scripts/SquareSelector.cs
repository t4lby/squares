using UnityEngine;
using System.Collections;

public class SquareSelector : MonoBehaviour
{
    /*
     * Each square has it's own prefab. Assigned in the unity editor.
     */
    public GameObject WhitePrefab;
    public GameObject BlackPrefab;
    public GameObject GreyPrefab;
    public GameObject YellowPrefab;
    public GameObject RedPrefab;
    public GameObject BluePrefab;
    public GameObject GreenPrefab;
    public GameObject BrownPrefab;
    public GameObject PinkPrefab;
    public GameObject OrangePrefab;
    public GameObject PurplePrefab;

    public GameObject GetPrefab(SquareType sType)
    {
        switch (sType)
        {
            case SquareType.White:
                return WhitePrefab;
            case SquareType.Black:
                return BlackPrefab;
            case SquareType.Grey:
                return GreyPrefab;
            case SquareType.Yellow:
                return YellowPrefab;
            case SquareType.Red:
                return RedPrefab;
            case SquareType.Blue:
                return BluePrefab;
            case SquareType.Green:
                return GreenPrefab;
            case SquareType.Brown:
                return BrownPrefab;
            case SquareType.Pink:
                return PinkPrefab;
            case SquareType.Orange:
                return OrangePrefab;
            case SquareType.Purple:
                return PurplePrefab;
            default:
                throw new UnityException(message: "Square prefab not found.");
        }
    }
}

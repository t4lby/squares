using UnityEngine;
using System.Collections;

public class Selector : MonoBehaviour
{
    public Square Target;

    private void OnTriggerStay(Collider other)
    {
        var hit = other.GetComponent<Square>();
        if (hit != null)
        {
            Target = hit;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Target = null;
    }
}

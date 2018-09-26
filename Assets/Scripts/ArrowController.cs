using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour {

    //Displays the arrow between the mouse location and the basetarget;

    public Transform BaseTarget;

    public float MinVisibleDistance = 0.1f;

    private void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update ()
    {
        //rotation must be the difference between the diff and east.

        var diff = Tools.GetMousePositionInScene() - BaseTarget.position;

        this.GetComponent<SpriteRenderer>().enabled = diff.magnitude > MinVisibleDistance;

        var angle = Vector3.Angle(
            Vector3.left,
            Tools.GetMousePositionInScene() - BaseTarget.position);
        angle = diff.y > 0 ? -angle : angle;
        var rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0, 0, angle);
        this.transform.rotation = rotation;

        // position is the mean of the 2 vectors

        transform.position = (BaseTarget.position + Tools.GetMousePositionInScene()) / 2;

        //scale is tha magnitude of the difference (roughly)
        transform.localScale = new Vector3(diff.magnitude, transform.localScale.y, transform.localScale.z);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour {
    
    public Player Target;

    public float SmoothSpeed = 0.125f;

    public Vector3 Offset;
	
	private void LateUpdate ()
    {
        transform.position = Vector3.Lerp(transform.position,
                                          Target.GetPosition() + Offset,
                                          SmoothSpeed);
	}
}

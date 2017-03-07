using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour {

    public Transform Lookat;
    private bool smooth = true;
    private float smoothSpeed = 0.05f;
    private Vector3 offset = new Vector3(0, 0f, -5f);

    private void LateUpdate()
    {
        Vector3 desiredPosition = Lookat.transform.position + offset;
        
        if (smooth)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
        else
        {
            transform.position = desiredPosition;
        }
    }
}

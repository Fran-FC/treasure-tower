using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{

    float rotationDest = 0f, rotationOrientation = 1f, diff;
    private void FixedUpdate() {
        diff = transform.eulerAngles.z - rotationDest ;
        if (Mathf.Abs(diff) <= 4f)
        {
            if(Input.GetKey(KeyCode.Q))
            {
                rotationOrientation = 1f;
                rotationDest = (360 + rotationDest + 90f) % 360f;
            } else if(Input.GetKey(KeyCode.E)) 
            {
                rotationOrientation = -1f;
                rotationDest = (360 + rotationDest - 90f) % 360f;
            }
        } else {
            transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), rotationOrientation * 360f * Time.deltaTime);
        }
    }
}
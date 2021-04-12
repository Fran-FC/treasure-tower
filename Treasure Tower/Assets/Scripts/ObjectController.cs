using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{

    float rotationDest = 0f, rotationOrientation = 1f, diff;
    private void FixedUpdate() {
        diff = transform.eulerAngles.z - rotationDest ;
        // when we reach rotation point, capture key events and update rotation dest
        if (Mathf.Abs(diff) <= 4f)
        {
            if(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Mouse0))
            {
                rotationOrientation = 1f;
                rotationDest = (360 + rotationDest + 90f) % 360f;

                // if we moved, send event for moving
                Messenger.Broadcast(GameEvent.MOVED);
            } else if(Input.GetKey(KeyCode.E)|| Input.GetKey(KeyCode.Mouse1) )
            {
                rotationOrientation = -1f;
                rotationDest = (360 + rotationDest - 90f) % 360f;

                // if we moved, send event for moving
                Messenger.Broadcast(GameEvent.MOVED);
            }
        } else {
            transform.RotateAround(transform.parent.position, new Vector3(0, 0, 1), rotationOrientation * 360f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Enemy")) {

        } else if (other.CompareTag("Item")) {

        }
    }
}
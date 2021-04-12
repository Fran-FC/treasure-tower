using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    bool throwObject = false, pickUpItem = false, firstObject = false;
    Transform item;

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


    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            throwObject = true;
        } 
        if(Input.GetKeyDown(KeyCode.X)) {
            if(pickUpItem) {
                pickUpItem = false;
                item.parent = transform;

                if(!firstObject ){
                    firstObject = true;
                    Messenger.Broadcast("SPAWN_ENEMY");
                }
            }
        }
        if (throwObject) {
            throwObject = false;
            Debug.Log(item.parent.ToString());
            if(item.parent == transform)
            {
                item.parent = null;
            }
        }  
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Item")) {
            // pick up object if we pressed X key
            pickUpItem = true;
            item = other.gameObject.transform;
            // rotate item to face opposite PLayer's side 
            // ToDo: show in the UI that we picked up the item
        }
    }
}
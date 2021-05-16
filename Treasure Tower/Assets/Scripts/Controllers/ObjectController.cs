using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Transform movePoint;
    Vector3 distance;
    bool throwObject = false, pickUpItem = false, firstObject = false;
    Transform item;
    float rotationDest = 0f, rotationOrientation = 1f, diff;

    private void FixedUpdate() {
        diff = transform.eulerAngles.z - rotationDest ;
        // when we reach rotation point, capture key events and update rotation dest
        if (Mathf.Abs(diff) <= 0.05f)
        {
            if(item!= null && item.parent == transform){
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
            }
        } else {
            transform.Rotate(new Vector3(0f,0f,rotationOrientation*9f), Space.Self);
        }
    }


    private void Update() {
        if(item != null)
            distance = item.position - transform.position;
        if(Vector3.Distance(transform.parent.position, movePoint.position) <= 0.005f)
        {
            if (Input.GetKeyDown(KeyCode.Z)) {
                throwObject = true;
            } 
            if(Input.GetKeyDown(KeyCode.X)) {
                if(pickUpItem) {
                    pickUpItem = false;
                    // orientar objeto para el lado que toca
                    float rotation = 0f;

                    if(Mathf.Abs(distance.x - 1.0f) <= 0.05f)
                        rotation = -90f; 
                    else if(Mathf.Abs(distance.x + 1.0f) <= 0.05f)
                        rotation = 90f;
                    else if(Mathf.Abs(distance.y -1.0f) <= 0.05f)
                        rotation = 0f; 
                    else
                        rotation = 180f;

                    rotation = rotation - item.eulerAngles.z;
                    item.Rotate(new Vector3(0f, 0f, rotation), Space.Self);
                    item.parent = transform;

                    // para la primera demo, spawn enemigo cuando se coja el arma
                    if(!firstObject ){
                        firstObject = true;
                        Messenger.Broadcast("SPAWN_ENEMY");
                    }
                }
            }
            if (throwObject) {
                throwObject = false;
                if(item.parent == transform)
                {
                    item.parent = null;
                    pickUpItem = true;
                }
            }  
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Item")) {
            // set flag to indicate we can pick up item 
            pickUpItem = true;
            item = other.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Item")) {
            pickUpItem = false;
        }
    }
}
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
        if (Input.GetKeyDown(KeyCode.Z)) {
            throwObject = true;
        } 
        if(Input.GetKeyDown(KeyCode.X)) {
            if(pickUpItem) {
                pickUpItem = false;
                // orientar objeto para el lado que toca
                Vector3 distance = item.position - transform.position;
                Debug.Log(distance);
                float rotation = 0f;
                switch (distance.x)
                {
                    case 1f:
                        rotation = -90f; 
                        break;
                    case -1f:
                        rotation = 90f;
                        break;
                }
                switch (distance.y)
                {
                    case 1f:
                        rotation = 0f;
                        break;
                    case -1f:
                        rotation = 180f;
                        break;
                }
                rotation = (item.rotation.z -item.rotation.z) + rotation; 
                Debug.Log("Rotating" + rotation);
                item.Rotate(new Vector3(item.rotation.x, item.rotation.y, rotation), Space.Self);
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
            Debug.Log(item.parent.ToString());
            if(item.parent == transform)
            {
                item.parent = null;
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
}
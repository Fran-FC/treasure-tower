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

    private TileMapGenerator mapGridInfo; 

    Vector3[] neightTiles;
    int neightIndex = 2;

    bool rotating = false;

    void Start() {
        mapGridInfo = (TileMapGenerator)FindObjectOfType(typeof(TileMapGenerator));
        neightTiles = new Vector3[8];
    }

    private void FixedUpdate() {

        diff = transform.eulerAngles.z - rotationDest ;
        // when we reach rotation point, capture key events and update rotation dest
        if (Mathf.Abs(diff) <= 0.05f)
        {
            if(item!= null && item.parent == transform){
                if(rotating) {
                    rotating = false;
                    Messenger.Broadcast(GameEvent.REACHED);
                }
                if(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Mouse0))
                {
                    // check if it is possible to rotate
                    if(canRotate(-1)) {
                        // if we moved, send event for moving
                        Messenger.Broadcast(GameEvent.MOVED);

                        neightIndex = (8 + neightIndex - 2) % 8;
                        rotationOrientation = 1f;
                        rotationDest = (360 + rotationDest + 90f) % 360f;

                        rotating = true;
                    } 
                } else if(Input.GetKey(KeyCode.E)|| Input.GetKey(KeyCode.Mouse1) )
                {
                    if(canRotate(1)) {
                        // if we moved, send event for moving
                        Messenger.Broadcast(GameEvent.MOVED);

                        neightIndex = (8 + neightIndex + 2) % 8;
                        rotationOrientation = -1f;
                        rotationDest = (360 + rotationDest - 90f) % 360f;

                        rotating = true;
                    }
                }
            }
        } else {
            transform.Rotate(new Vector3(0f,0f,rotationOrientation*9f), Space.Self);
        }
    }


    private void Update() {
        if(item != null){
            UpdateNeightbourTiles();
            distance = item.position - transform.position;
        }
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
                    bool canPickUp = true;

                    if(Mathf.Abs(distance.x - 1.0f) <= 0.05f) {
                        rotation = -90f; 
                        neightIndex = 2;
                    }
                    else if(Mathf.Abs(distance.x + 1.0f) <= 0.05f) {
                        rotation = 90f;
                        neightIndex = 6;
                    }
                    else if(Mathf.Abs(distance.y -1.0f) <= 0.05f){
                        rotation = 0f; 
                        neightIndex = 0;
                    }
                    else if(Mathf.Abs(distance.y + 1.0f) <= 0.05f) {
                        rotation = 180f;
                        neightIndex = 4;
                    } else {
                        canPickUp = false; 
                    }

                    if(canPickUp) {
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

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("Item")) {
            pickUpItem = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Item")) {
            pickUpItem = false;
            Debug.Log("CANT PICK UP");
        }
    }

    void UpdateNeightbourTiles() {
        neightTiles[0] = transform.position + new Vector3(0, 1, 0);
        neightTiles[1] = transform.position + new Vector3(1, 1, 0);
        neightTiles[2] = transform.position + new Vector3(1, 0, 0);
        neightTiles[3] = transform.position + new Vector3(1, -1, 0);
        neightTiles[4] = transform.position + new Vector3(0, -1, 0);
        neightTiles[5] = transform.position + new Vector3(-1, -1, 0);
        neightTiles[6] = transform.position + new Vector3(-1, 0, 0);
        neightTiles[7] = transform.position + new Vector3(-1, 1, 0);
    }

    private bool canRotate(int orientation) {
        //Debug.Log(item.position);
        bool res = true;
        Vector3 offsetOne, offsetTwo;
        offsetOne = neightTiles[(neightIndex + 8 + orientation)%8];
        offsetTwo = neightTiles[(neightIndex + 8 + 2*orientation)%8];

        if(mapGridInfo.isTileWalkable(offsetOne.x, offsetOne.y) || mapGridInfo.isTileEnemy(offsetOne.x, offsetOne.y)) {
            if(mapGridInfo.isTileWalkable(offsetTwo.x, offsetTwo.y) || mapGridInfo.isTileEnemy(offsetTwo.x, offsetTwo.y)) {
                res = true;
            } else {
                res = false;
            }
        } else {
            res = false;
        }
        return res;    
    }
}
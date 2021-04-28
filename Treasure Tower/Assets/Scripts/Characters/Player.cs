using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject rotationObject;
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool skipTurn = false;
    private TileMapGenerator mapGridInfo; 
    // Variables for movement
    Vector2 movement;
    public Transform movePoint;
    public float moveSpeed = 5.0f;
    float orientation = 1f;
    bool flip = false;
    enum CharWalkStates { idle = 0, walk = 1 }
    CharWalkStates prevWalkState;

    // life states
    float lifes = 3;
    enum CharLifeStates {
        full = 3, 
        half = 2,
        nacked = 1,
        dead = 0
    }
    CharLifeStates lifeState;

    void Start()
    {
        mapGridInfo = (TileMapGenerator)FindObjectOfType(typeof(TileMapGenerator));
        movePoint.parent = null;        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        prevWalkState = CharWalkStates.walk;
    }
    private void Awake() {
        lifes = 3f;
    }
    private void FixedUpdate() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = Vector2.ClampMagnitude(movement, 1.0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            skipTurn = true;
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            Messenger.Broadcast("SPAWN_ENEMY");
        }
        // grid movement logic
        CalcMovement();
    }
    void CalcMovement(){
        CharWalkStates state = prevWalkState;

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);
    
        if(Vector3.Distance(transform.position, movePoint.position) <= 0.005f)
        {
            Vector3 cachePos = movePoint.position;
            Vector3 oldPos = cachePos;
            Vector3 objectOffset;

            //get object orientation
            Vector3 objPos = GetObjectPos();
            bool walkable = false;
            if(!Vector3.Equals(objPos, Vector3.zero) )  {
                objectOffset = objPos + new Vector3(movement.x, movement.y, 0f); 
                if(mapGridInfo.isTileWalkable(objectOffset.x, objectOffset.y)|| mapGridInfo.isTileEnemy(objectOffset.x, objectOffset.y)) {
                    walkable = true;        
                }
                float dist = Vector3.Distance(transform.position , objectOffset);
                if(dist < 0.05f) {
                    walkable = true;
                }
            } else {
                walkable = true;
            }
            if ( Mathf.Abs(movement.x) == 1f)
            {
                cachePos += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                if(walkable){
                    if (mapGridInfo.isTileWalkable(cachePos.x, cachePos.y)) {
                        movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                        mapGridInfo.setTileWalkableState(oldPos.x, oldPos.y, true);
                        mapGridInfo.setTileWalkableState(cachePos.x, cachePos.y, false);

                        if (movement.x > 0f)
                        {
                            prevWalkState = CharWalkStates.walk;
                            if (orientation < 0f)
                            {
                                orientation = 1f;
                                flip = false;
                            }
                        }
                        else
                        {
                            prevWalkState = CharWalkStates.walk;
                            if (orientation > 0f)
                            {
                                orientation = -1f;
                                flip = true;
                            }
                        }
                        // if we moved, send event for moving
                        Messenger.Broadcast(GameEvent.MOVED);
                    }
                }
                
            } else if ( Mathf.Abs(movement.y) == 1f)
            {
                cachePos += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                //cachePos.y += objPos.y;
                if(walkable) {
                    if (mapGridInfo.isTileWalkable(cachePos.x, cachePos.y)) {
                        movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                        mapGridInfo.setTileWalkableState(oldPos.x, oldPos.y, true);
                        mapGridInfo.setTileWalkableState(cachePos.x, cachePos.y, false);
                        prevWalkState = CharWalkStates.walk;

                        // if we moved, send event for moving
                        Messenger.Broadcast(GameEvent.MOVED);
                    }
                }
            } else if(skipTurn) 
            {
                    Messenger.Broadcast(GameEvent.MOVED);
                    skipTurn = false;
            } else {
                prevWalkState = CharWalkStates.idle;
            }
        } 
        animator.SetInteger("WalkState", (int)state);
        spriteRenderer.flipX = flip;
    }
    
    private Vector3 GetObjectPos(){
        Transform obj;
        if(rotationObject.transform.childCount > 0) {
            obj = rotationObject.transform.GetChild(0);

            //Debug.Log(obj.position);
            return  obj.position;
        }
        return Vector3.zero;
    }

    private Vector3 ObjectOffsetPosition(Vector3 obj, Vector3 move) {
        Debug.Log("Object position: "+ obj);
        Debug.Log("Movement orientation: "+move);
        float dist = Vector3.Distance(obj, move);
        if(dist < 0.05f){
            Debug.Log("Offset applied");
            return move;
        }
        return Vector3.zero;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Armor")){
            if(lifes < 3){
                lifes++;
                UpdateLifeState();
                other.gameObject.SetActive(false);
            }
        }
        if(other.CompareTag("Enemy")){
            Damage();
        }
    } 
    public void Damage(){
        lifes--;
        UpdateLifeState();
        StartCoroutine(FlickerCharacter());
    }
    
    public virtual IEnumerator FlickerCharacter() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.white;
    }

    private void UpdateLifeState()
    {
        switch (lifes)
        {
            case 3:
                lifeState = CharLifeStates.full;
                break;
            case 2:
                lifeState = CharLifeStates.half;
                break;
            case 1:
                lifeState = CharLifeStates.nacked;
                break;
            default:
                lifeState = CharLifeStates.dead;
                transform.gameObject.SetActive(false);
                break;
        }


        animator.SetInteger("HealthState", (int)lifeState);

    }
}
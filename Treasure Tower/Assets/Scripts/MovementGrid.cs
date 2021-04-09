using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGrid : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    //BoxCollider2D collider2D;

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

    private void Awake() {
        lifes = 3f;
    }
    void Start()
    {
        movePoint.parent = null;        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //collider2D = GetComponent<BoxCollider2D>();
        prevWalkState = CharWalkStates.walk;
    }

    private void FixedUpdate() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = Vector2.ClampMagnitude(movement, 1.0f);
    }

    void Update()
    {
        CalcMovement();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("COLISION");
        if(other.gameObject.CompareTag("Enemy"))
        {
            lifes--;
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
        }
        animator.SetInteger("HealthState", (int)lifeState);
    }
    void CalcMovement(){
        CharWalkStates state = prevWalkState;

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if(Vector3.Distance(transform.position, movePoint.position) <= 0.005f)
        {
            if ( Mathf.Abs(movement.x) == 1f)
            {
                movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                if (movement.x  > 0f)
                {
                    prevWalkState = CharWalkStates.walk;
                    if(orientation < 0f){
                        orientation = 1f;
                        flip = false;
                    }
                }
                else
                {
                    prevWalkState = CharWalkStates.walk;
                    if(orientation > 0f){
                        orientation = -1f;
                        flip = true;
                    }
                }

                // if we moved, send event for moving
                Messenger.Broadcast(GameEvent.MOVED);
            } else if ( Mathf.Abs(movement.y) == 1f)
            {
                movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                prevWalkState = CharWalkStates.walk;

                // if we moved, send event for moving
                Messenger.Broadcast(GameEvent.MOVED);
            } else {
                prevWalkState = CharWalkStates.idle;
            }
        } 
        animator.SetInteger("WalkState", (int)state);
        spriteRenderer.flipX = flip;
    }
}
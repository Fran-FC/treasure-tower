using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementGrid : MonoBehaviour
{
    public Transform movePoint;
    public float moveSpeed = 5.0f;
    Vector2 movement;
    Animator animator;
    SpriteRenderer spriteRenderer;
    float orientation = 1f;
    bool flip = false;
    enum CharStates { idle = 0, walk = 1 }
    CharStates prevState;

    void Start()
    {
        movePoint.parent = null;        
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        prevState = CharStates.walk;
    }

    private void FixedUpdate() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = Vector2.ClampMagnitude(movement, 1.0f);
    }

    void Update()
    {
        CharStates state = prevState;

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if(Vector3.Distance(transform.position, movePoint.position) <= 0.005f)
        {
            if ( Mathf.Abs(movement.x) == 1f)
            {
                movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                if (movement.x  > 0f)
                {
                    prevState = CharStates.walk;
                    if(orientation < 0f){
                        orientation = 1f;
                        flip = false;
                    }
                }
                else
                {
                    prevState = CharStates.walk;
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
                prevState = CharStates.walk;

                // if we moved, send event for moving
                Messenger.Broadcast(GameEvent.MOVED);
            } else {
                prevState = CharStates.idle;
            }
        } 
        animator.SetInteger("WalkState", (int)state);
        spriteRenderer.flipX = flip;
    }
}
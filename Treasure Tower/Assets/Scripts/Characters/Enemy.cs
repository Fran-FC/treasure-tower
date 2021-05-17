﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Transform movePoint;
    public Transform warn;
    public float thrust = 3f;
    float knockTime = 0.2f;
    private TileMapGenerator mapGridInfo;
    private Vector3 siguienteMovimiento;
    private Vector3 newPoint;
    public bool armor;
    public float moveSpeed;
    public Vector2 orientation = new Vector2(0, 0);
    private bool inRange = false;
    [SerializeField]
    private GameObject player;
    private bool isFlippedX = false;
    private bool walking = false;
    private bool turn = true;
    private bool isFlippedY = false;
    private bool attackTurn = false;
    private bool knockback = false;
    public int hp = 1;
    private bool stuned = false;
    int stunCounter = 0;

    private List<GridInfo> currentPath;

    // Start is called before the first frame update
    /*enum WhichEnemy
    {
        skeleton = 0,
        ghost = 1,
        troll = 2,
        fire = 3
    }*/
    int whichEnemy;
    SpriteRenderer spriteRenderer;
    Animator animator;

    Rigidbody2D rigidbody;

    void Start()
    {
        //player = GetComponent<Player>();
        rigidbody = GetComponent<Rigidbody2D>();
        mapGridInfo = (TileMapGenerator)FindObjectOfType(typeof(TileMapGenerator));
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        whichEnemy = new System.Random().Next(0, 3);
        InitSprite();

        movePoint.parent = null;
    }

    void InitSprite()
    {
        switch (whichEnemy)
        {
            case 0:

                break;
        }
    }
    private void Awake()
    {
        Messenger.AddListener(GameEvent.MOVE_ORDER, OnMyTurn);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.MOVE_ORDER, OnMyTurn);
    }

    private void OnMyTurn()
    {
        if(!stuned) {
            turn = true;
            attackTurn = true;

            player = GameObject.FindWithTag("Player");
            CalcInRange();
            if (inRange )
            {
                Attack();
            }
            else
            {
                //newPoint = movePoint.position + siguienteMovimiento;
                //siguienteMovimiento = mapGridInfo.DrawPath(transform.position, player.transform.position);

                Vector3 currentPos = transform.position;
                Vector3 playerPos = player.transform.position;

                if(Vector3.Distance(currentPos, playerPos) < 10f)
                {
                    if (mapGridInfo.isTargetVisible(currentPos, playerPos)) {
                        List<GridInfo> generatedPath = mapGridInfo.GetValidPath(currentPos, playerPos);
                        if (generatedPath != null) {
                            currentPath = generatedPath;

                        }
                    }
                }

                if (currentPath != null && currentPath.Count > 0) {

                    GridInfo nextTile = currentPath[0];
                    currentPath.RemoveAt(0);

                    siguienteMovimiento = mapGridInfo.getGridInfoGlobalTransform(nextTile);

                    newPoint = siguienteMovimiento;

                    if (mapGridInfo.isTileWalkable(newPoint.x, newPoint.y) && hp > 0)
                    {
                        mapGridInfo.setTileWalkableState(movePoint.position.x, movePoint.position.y, true);
                        mapGridInfo.setTileEnemyState(movePoint.position.x, movePoint.position.y, false);
                        movePoint.position = newPoint;
                        mapGridInfo.setTileWalkableState(movePoint.position.x, movePoint.position.y, false);
                        mapGridInfo.setTileEnemyState(movePoint.position.x, movePoint.position.y, true);
                    }

                    if (animator.GetInteger("WalkState") == 0)
                    {
                        animator.SetInteger("WalkState", 1);
                    }
                }
            }
        }
        else  
        {
            stunCounter++;
            if(stunCounter >= 2) {
                stunCounter = 0;
                stuned = false;
                animator.SetBool("Stuned", false);
            }
        }
    }
    void Update()
    {
        if (!knockback)
        {
            // calculate path and paint next move
            if (!walking && turn)
            {
                turn = false;
                //siguienteMovimiento = CalcPath();

                //siguienteMovimiento = mapGridInfo.DrawPath(transform.position, player.transform.position);

                //Debug.Log(siguienteMovimiento);

                newPoint = movePoint.position;

                warn.position = siguienteMovimiento;
                //ShowWarnTile(true);
            }
            Vector3 aux = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y, 0);
            float nuevaX = Math.Sign(aux.x) * -1;
            if (nuevaX > 0) isFlippedX = false; else isFlippedX = true;
            spriteRenderer.flipX = isFlippedX;

            // move towards movepoint
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
            //warn.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;
            walking = true;
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                walking = false;
                animator.SetInteger("WalkState", 0);
            }
            ShowWarnTile(!walking);
        }
        else
        {
            // remove next tile mark
            ShowWarnTile(false);
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime * 2);
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                knockback = false;
            }
        }
    }

    private void ShowWarnTile(bool show)
    {
        Vector3 hidden = new Vector3(warn.position.x, warn.position.y, 1f);
        Vector3 nothidden = new Vector3(warn.position.x, warn.position.y, 0f);

        warn.position = show ? nothidden : hidden;
    }

    private void CalcInRange()
    {
        if (player.transform.position.x == this.transform.position.x && (player.transform.position.y == this.transform.position.y - 1 || player.transform.position.y == this.transform.position.y + 1))
        {
            inRange = true;
        }
        else
            if (player.transform.position.y == this.transform.position.y && (player.transform.position.x == this.transform.position.x - 1 || player.transform.position.x == this.transform.position.x + 1))
        {
            inRange = true;
        }
        else
        {
            inRange = false;
        }
    }
    private void Attack()
    {
        //TD
        if (hp > 0 && attackTurn)
        {
            attackTurn = false;
            player.GetComponent<Player>().Damage();
        }
    }
    private Vector3 CalcPath()
    {
        Vector3 aux = new Vector3(this.transform.position.x - player.transform.position.x, this.transform.position.y - player.transform.position.y, 0);
        float nuevaX = Math.Sign(aux.x) * -1;
        if (nuevaX > 0) isFlippedX = false; else isFlippedX = true;
        spriteRenderer.flipX = isFlippedX;

        if (mapGridInfo.isTileWalkable(this.transform.position.x + nuevaX, 0) && Mathf.Abs(aux.x) > Mathf.Abs(aux.y))
        {
            orientation.x = nuevaX;
            return new Vector3(nuevaX, 0f, 0f);
        }

        float nuevaY = Math.Sign(aux.y) * -1;
        return new Vector3(0f, nuevaY, 0f);

    }

    private bool isPlayerNearby()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(distance);
        return distance <= 10.0f;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            if (other.gameObject.transform.parent != null)
            {
                stuned = true;
                animator.SetBool("Stuned", true);
                hp--;
                KnockBack(other.gameObject.transform.position);
                animator.SetBool("Damage", true);
                Invoke("HandleDamage", 0.4f);
            }
        }

    }

    private void HandleDamage()
    {
        if (hp <= 0)
        {
            Destroy(gameObject);
            mapGridInfo.setTileWalkableState(transform.position.x, transform.position.y, true);
            mapGridInfo.setTileEnemyState(transform.position.x, transform.position.y, false);
        }
        animator.SetBool("Damage", false);
    }

    private void KnockBack(Vector3 weaponPosition)
    {
        knockback = true;
        //calc position knockback
        mapGridInfo.setTileWalkableState(transform.position.x, transform.position.y, true);
        mapGridInfo.setTileEnemyState(transform.position.x, transform.position.y, false);

        Vector3 distance = transform.position - weaponPosition;
        if (distance.x > 0.1f)
        {
            distance.x = -1f;
        }
        else if (distance.x < -0.1f)
        {
            distance.x = 1f;
        }
        if (distance.y > 0.1f)
        {
            distance.y = -1f;
        }
        else if (distance.y < -0.1f)
        {
            distance.y = 1f;
        }

        //Debug.Log(distance);
        Vector3 newPoint = transform.position - distance;
        if (mapGridInfo.isTileWalkable(newPoint.x, newPoint.y))
        {
            mapGridInfo.setTileWalkableState(movePoint.position.x, movePoint.position.y, true);
            mapGridInfo.setTileEnemyState(movePoint.position.x, movePoint.position.y, false);
            movePoint.position = newPoint;
            mapGridInfo.setTileWalkableState(movePoint.position.x, movePoint.position.y, false);
            mapGridInfo.setTileEnemyState(movePoint.position.x, movePoint.position.y, true);
        } 
    }
}
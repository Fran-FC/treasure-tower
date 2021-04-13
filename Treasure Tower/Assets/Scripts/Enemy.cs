using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Transform movePoint;
    private TileMapGenerator mapGridInfo;
    private Vector3 siguienteMovimiento;
    public bool armor;
    public float moveSpeed;
    public Vector2 orientation = new Vector2(0,0);
    private bool inRange = false;
    [SerializeField]
    private GameObject player;
    private bool isFlippedX = false;
    private bool isFlippedY = false;

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

    void Start()
    {
        //player = GetComponent<Player>();
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
        Messenger.AddListener(GameEvent.MOVED, OnMyTurn);
        Messenger.AddListener(GameEvent.MOVE_ORDER, OnMyTurn);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.MOVED, OnMyTurn);
        Messenger.RemoveListener(GameEvent.MOVE_ORDER, OnMyTurn);

    }

    private void OnMyTurn()
    {
        player = GameObject.FindWithTag("Player");
        CalcInRange();
        if (inRange)
        {
            Attack();
        } else
        {
            ///if (CanWalk())
            //{
                siguienteMovimiento = CalcPath();
                //this.transform.position += siguienteMovimiento;
                animator.SetInteger("WalkState", 1);
                movePoint.position += siguienteMovimiento;
                // ToDo : pintar siguiente movePoint
            //}
        }
        //TD

    }
    void Update()
    {
        Vector3 aux = new Vector3(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y, 0);
        float nuevaX = Math.Sign(aux.x) * -1;
        if (nuevaX > 0) isFlippedX = false; else isFlippedX = true;
        spriteRenderer.flipX = isFlippedX;

        // move towards movepoint
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime );
        if(Vector3.Distance(transform.position, movePoint.position) <= 0.005f)
        {
            animator.SetInteger("WalkState", 0);
        }
    }

    private void CalcInRange()
    {
        /**
        if(Vector3.Distance(player.transform.position, transform.position) <= 1.05f){

        }**/
        if(player.transform.position.x == this.transform.position.x && (player.transform.position.y == this.transform.position.y - 1 || player.transform.position.y == this.transform.position.y + 1))
        {
            inRange = true;
        } else 
            if(player.transform.position.y == this.transform.position.y && (player.transform.position.x == this.transform.position.x - 1 || player.transform.position.x == this.transform.position.x + 1)) {
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
        player.GetComponent<Player>().Damage();
    }
    private Vector3 CalcPath()
    {
        //orientation = new Vector2(0, 0);
        // ToDO: check player distance
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
   /*private bool CanWalk()
    {
        bool canWalk = false;
        if(mapGridInfo.isTileWalkable(this.transform.position.x-1,0) || mapGridInfo.isTileWalkable(this.transform.position.x + 1, 0) || mapGridInfo.isTileWalkable(0, this.transform.position.y - 1) || mapGridInfo.isTileWalkable(0, this.transform.position.y - 1))
        {
            canWalk = true;
        }
        return canWalk;
    }*/
}
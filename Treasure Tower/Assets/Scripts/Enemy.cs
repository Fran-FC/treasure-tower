using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public Transform movePoint;
    public float thrust = 3f;
    float knockTime = 0.2f;
    private TileMapGenerator mapGridInfo;
    private Vector3 siguienteMovimiento;
    public bool armor;
    public float moveSpeed;
    public Vector2 orientation = new Vector2(0, 0);
    private bool inRange = false;
    [SerializeField]
    private GameObject player;
    private bool isFlippedX = false;
    private bool walking = false;
    private bool isFlippedY = false;
    public int hp = 1;

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
        player = GameObject.FindWithTag("Player");
        CalcInRange();
        if (inRange)
        {
            Attack();
        }
        else
        {
            ///if (CanWalk())
            //{
            siguienteMovimiento = CalcPath();
            //this.transform.position += siguienteMovimiento;

            Vector3 newPoint = movePoint.position + siguienteMovimiento;

            if (mapGridInfo.isTileWalkable(newPoint.x, newPoint.y)) {
                mapGridInfo.setTileWalkableState(movePoint.position.x, movePoint.position.y, true);
                movePoint.position += siguienteMovimiento;
                mapGridInfo.setTileWalkableState(movePoint.position.x, movePoint.position.y, false);
            }

            

            if (animator.GetInteger("WalkState") == 0)
            {
                animator.SetInteger("WalkState", 1);
            }
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
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, movePoint.position) <= 0.005f)
        {
            animator.SetInteger("WalkState", 0);
        }
    }

    private void CalcInRange()
    {
        /**
        if(Vector3.Distance(player.transform.position, transform.position) <= 1.05f){

        }**/
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

        Debug.Log("Call to CalcPath " + aux);


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


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            // animation of damage
            // knockback
            if (other.gameObject.transform.parent != null) {
                rigidbody.isKinematic = false;
                Vector2 diff = rigidbody.transform.position - other.transform.position;
                diff = diff.normalized * thrust;
                rigidbody.AddForce(diff, ForceMode2D.Impulse);

                StartCoroutine("KnockCo");

                animator.SetBool("Damage", true);
                Invoke("RecieveDamage", 0.4f);
            }
        }

    }
    private void RecieveDamage(){
        hp--;
        if (hp == 0) {

            mapGridInfo.setTileWalkableState(gameObject.transform.position.x, gameObject.transform.position.y, true);
            Destroy(gameObject);

        }
        animator.SetBool("Damage", false);
    }

    private IEnumerator KnockCo(){
        yield return new WaitForSeconds(knockTime);

        rigidbody.velocity = Vector2.zero;
        rigidbody.isKinematic = true;
    }

}
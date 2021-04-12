using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    private Vector3 siguienteMovimiento;
    public bool armor;
    public float moveSpeed;
    public float orientation = 1f;
    private bool inRange = false;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        //player = GetComponent<Player>();

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
        CalcInRange();
        if (inRange)
        {
            Attack();
        } else
        {
            CalcPath();
            this.transform.position += siguienteMovimiento;
            Debug.Log("Transform position: " + this.transform.position);
        }
        //TD

    }

    private void CalcInRange()
    {
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
    }
    private void CalcPath()
    {
        Vector3 aux = new Vector3(this.transform.position.x - player.transform.position.x, this.transform.position.y - player.transform.position.y, 0);
        if (Math.Abs(aux.x) > Math.Abs(aux.y))
        {
            float nuevaX = Math.Sign(aux.x) * -1;
            siguienteMovimiento = new Vector3(nuevaX, 0f, 0f);
        }
        else
        {
            float nuevaY = Math.Sign(aux.y) * -1;
            siguienteMovimiento = new Vector3(0f, nuevaY, 0f);
        }
    }
}
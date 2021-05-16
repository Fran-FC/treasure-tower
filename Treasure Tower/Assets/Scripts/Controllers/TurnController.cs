using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    float countdown;
    public float timeToMove;
    public bool playerMoved = false;

    private void Start() {
        countdown = timeToMove;
    }
    private void Awake() {
        Messenger.AddListener(GameEvent.MOVED, OnMoved);
    }
    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.MOVED, OnMoved);
    }
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f) {
            countdown = timeToMove;
            // send signal to all enemies to move
            SignalMove();
        } else if (playerMoved) {
            countdown = timeToMove;
            playerMoved = false;
            // send signal to all enemies to move
            SignalMove();
        }
        
    }

    private void SignalMove()
    {
        Messenger.Broadcast(GameEvent.MOVE_ORDER);
    }

    private void OnMoved()
    {
        playerMoved = true;
    }
}

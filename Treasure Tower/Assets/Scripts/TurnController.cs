using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    float countdown;
    public float timeToMove;

    private void Start() {
        countdown = timeToMove;
    }
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f) {
            countdown = timeToMove;
            // ToDo: send signal to all enemies to move
            Debug.Log(" MOVE");
        } else if (Input.anyKey) {
            // send signal to all enemies to move
            Debug.Log(" MOVE");
        }
        
    }
}

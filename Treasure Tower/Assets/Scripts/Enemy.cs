using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() {
        Messenger.AddListener(GameEvent.MOVE_ORDER, OnMyTurn);
    }
    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.MOVE_ORDER, OnMyTurn);
        
    }

    private void OnMyTurn() {
        // ToDo move enemy
        transform.position += new Vector3(1, 0, 0);
    }
}

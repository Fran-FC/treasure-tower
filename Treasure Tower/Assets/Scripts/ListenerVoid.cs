using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListenerVoid : MonoBehaviour
{
    private void Awake() {
        Messenger.AddListener(GameEvent.MOVE_ORDER, Nothing);
    }
    private void OnDestroy() {
        Messenger.RemoveListener(GameEvent.MOVE_ORDER, Nothing);
    }

    private void Nothing(){}
}

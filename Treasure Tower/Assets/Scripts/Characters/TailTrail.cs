using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailTrail : MonoBehaviour
{
    public GameObject trail;
    static List<GameObject> trailPool;
    public int poolSize;

    private void Awake() {
        Messenger.AddListener(GameEvent.MOVE_ORDER, OnMyTurn);
        if(trailPool == null) {
            trailPool = new List<GameObject>();
        } 
        for(int i = 0; i < poolSize; i++)
        {
            GameObject trailObject = Instantiate(trail);
            trailObject.SetActive(false);
            trailPool.Add(trailObject);
        }
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.MOVE_ORDER, OnMyTurn);
    }

    private void OnMyTurn (){
        SpawnTrail(transform.position);
    }
    
    private void SpawnTrail(Vector3 location) {
        foreach (GameObject t in trailPool) {
            if (t.activeSelf == false) {
                t.SetActive(true);
                t.transform.position = location;
                return;
            }
        }
    }



}

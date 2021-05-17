using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailTrail : MonoBehaviour
{
    public GameObject trail;
    static List<GameObject> trailPool;
    public int poolSize;
    public Vector3 positionCache; 

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
        positionCache = transform.position;
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.MOVE_ORDER, OnMyTurn);
    }

    private void OnMyTurn(){
        float distance = Vector3.Distance(positionCache, transform.position);
        if(distance > 0.5f){
            SpawnTrail(transform.position);
            positionCache = transform.position;
            Debug.Log("TRAILING");
        } else {
            Debug.Log("NOT TRAILING");
        }

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

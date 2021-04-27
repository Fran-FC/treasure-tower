using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstLevelController : MonoBehaviour
{

    private bool enemiesSpawned = false;


    private void Awake()
    {
        Messenger.AddListener(GameEvent.SPAWN_ENEMY, setEnemiesSpawned);

    }
    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.SPAWN_ENEMY, setEnemiesSpawned);
    }


    public void setEnemiesSpawned() {
        enemiesSpawned = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesSpawned) {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {
                SceneManager.LoadScene("ProcMapGen");
            }
        }
    }
}

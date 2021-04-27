using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatileObject : MonoBehaviour
{
    public float timeToShow;
    float countdown;
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f) {
            countdown = timeToShow;
            // set active false
            gameObject.SetActive(false);
        }         
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Space)) {
           transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z+90f), Space.Self);
       }
    }
}

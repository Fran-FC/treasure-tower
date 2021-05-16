using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private static MusicController _instance;
    private static MusicController instance {
        get {
            if(_instance==null){
                _instance = GameObject.FindObjectOfType<MusicController>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    void Awake() {
        if(_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        } else {
            if(this != _instance) 
                Destroy(this.gameObject);
        }
    }

    public void Play() { 
        this.gameObject.GetComponent<AudioSource>().Play();
    }

}

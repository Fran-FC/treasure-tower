using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverController : MonoBehaviour
{
    public void Setup() {
        gameObject.SetActive(true);
    }

    public void RestartButton(){
        SceneManager.LoadScene("ProcMapGen");
    }

    public void ExitButton() {
        SceneManager.LoadScene("Menu");
    }
}

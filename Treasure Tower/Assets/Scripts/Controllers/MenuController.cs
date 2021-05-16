using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void NuevaPartida()
    {
        SceneManager.LoadScene("Assets/Scenes/BaseMapScene.unity");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Cerrando aplicación");
    }
}

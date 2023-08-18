using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadConfigScene()
    {
        // Cargar la escena "Configuracion"
        SceneManager.LoadScene("Configuracion");
    }
}

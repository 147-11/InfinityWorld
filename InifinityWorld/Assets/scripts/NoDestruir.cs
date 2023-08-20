using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoDestruir : MonoBehaviour
{
    //public Button _button;
    //public GameObject botonConfig;
  void Awake() {
   //_button = GameObject.Find("config").GetComponent<Button>();
    GameObject objs=GameObject.FindWithTag("Boton");
    //GameObject[] objs=GameObject.FindGameObjectsWithTag("Boton");
    //GameObject menuBoton = GameObject.Find("config");
    GameObject menuBoton2 = GameObject.Find("volver");
    DontDestroyOnLoad(objs);
     DontDestroyOnLoad(gameObject);
         // DontDestroyOnLoad(_button);
    //Debug.Log(menuBoton);
        Debug.Log(gameObject);
        Debug.Log(objs);
        // Debug.Log(_button);
  }
}

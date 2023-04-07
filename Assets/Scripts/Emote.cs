using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emote : MonoBehaviour
{
   
    public GameObject button;
    void Start()
    {
        Debug.Log("hello start");
        button.gameObject.SetActive(false);
    }



    void Update()
    {
        Debug.Log("hello"); 
        if (Input.GetKeyDown(KeyCode.B))
        {
            button.gameObject.SetActive(true);
        }
    }
}

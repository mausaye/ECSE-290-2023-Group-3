using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emote : MonoBehaviour
{
   
    public GameObject button;
    private const float BUTTON_DURATION = 5f;
    private bool buttonActive = false;
    private float buttonTimer = 0f;
    void Start()
    {
        
        button.gameObject.SetActive(false);
    }



    void Update()
    {
        float yOffset = 80f;

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("hello");
            buttonActive = true;
            button.gameObject.SetActive(true);
            
            button.transform.position = transform.position + Vector3.up * yOffset;
            buttonTimer = 0f;
        }
        if (buttonActive)
        {
            buttonTimer += Time.deltaTime;
            if (buttonTimer >= BUTTON_DURATION)
            {
                button.SetActive(false);
                buttonActive = false;
                buttonTimer = 0f;
            }
        }
    }
}

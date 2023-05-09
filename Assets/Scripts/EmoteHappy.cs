using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteHappy : MonoBehaviour
{
    //[SerializeField] private GameObject emotePrefab;
    public GameObject emote;
    private const float BUTTON_DURATION = 5f;
    private bool buttonActive;
    private float buttonTimer = 0f;
    

    void Start()
    {
        
        //emote = GameObject.FindWithTag("heart_emote");
        emote.SetActive(false);
    }

    void Update()
    {
        //float yOffset = 80f;

        if (buttonActive && Input.GetKey(KeyCode.N))
        {
            //Debug.Log("pressed N"); 
            emote.SetActive(true);
            //Debug.Log("setActive");
            //emote.transform.position = transform.position + Vector3.up * yOffset;
           buttonActive = true;
            buttonTimer = 0f;
            
        
         }

        if (buttonActive)
        {
            buttonTimer += Time.deltaTime;
            if (buttonTimer >= BUTTON_DURATION)
            {
                emote.SetActive(false);
                buttonActive = false;
                buttonTimer = 0f;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteAngry : MonoBehaviour
{
    public GameObject emoteHeart;
    public GameObject emoteSad;
    public GameObject emoteAngry;
    public GameObject emoteHappy;
    private const float BUTTON_DURATION = 2f;
    private bool buttonActive = false;
    private float buttonTimer = 0f;


    void Start()
    {
        emoteHeart.SetActive(false);
        emoteSad.SetActive(false);
        emoteHappy.SetActive(false);
        emoteAngry.SetActive(false);
        buttonActive = false;
    }

    void Update()
    {
        
            if (Input.GetKey(KeyCode.H))
            {
                Debug.Log("pressed H");
                emoteAngry.SetActive(true);
                buttonActive = true;
                buttonTimer = 0f;
                emoteHeart.SetActive(false);
                emoteSad.SetActive(false);
                emoteHappy.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.J))
            {
                Debug.Log("pressed J");
                emoteHeart.SetActive(true);
                buttonActive = true;
                buttonTimer = 0f;
                emoteAngry.SetActive(false);
                emoteSad.SetActive(false);
                emoteHappy.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.K))
            {
                Debug.Log("pressed K");
                emoteSad.SetActive(true);
                buttonActive = true;
                buttonTimer = 0f;
                emoteAngry.SetActive(false);
                emoteHeart.SetActive(false);
                emoteHappy.SetActive(false);
            }
            else if (Input.GetKey(KeyCode.L))
            {
                Debug.Log("pressed L");
                emoteHappy.SetActive(true);
                buttonActive = true;
                buttonTimer = 0f;
                emoteAngry.SetActive(false);
                emoteHeart.SetActive(false);
                emoteSad.SetActive(false);
            }
        

        if (buttonActive)
        {
            buttonTimer += Time.deltaTime;
            if (buttonTimer >= BUTTON_DURATION)
            {
                if (emoteHeart.activeSelf)
                {
                    emoteHeart.SetActive(false);
                }
                else if (emoteSad.activeSelf)
                {
                    emoteSad.SetActive(false);
                }
                else if (emoteHappy.activeSelf)
                {
                    emoteHappy.SetActive(false);
                }
                else if (emoteAngry.activeSelf)
                {
                    emoteAngry.SetActive(false);
                }

                buttonActive = false;
                buttonTimer = 0f;
            }
        }
    }
}
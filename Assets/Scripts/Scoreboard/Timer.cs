using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using Frosty.Scoreboards;

public class Timer : MonoBehaviour
{
    private float gameTime = 0;
    public static float finishTime;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject finishPanel; 
    [SerializeField] private TextMeshProUGUI finishTimeText;
    
    [SerializeField] private GameObject player;
    


    // Start is called before the first frame update
    void Start()
    {
        DisplayTime(gameTime);
    }

    void DisplayTime(float timeDisplay){
        float minutes = Mathf.FloorToInt(timeDisplay / 60);
        float seconds = Mathf.FloorToInt(timeDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (minutes > 5.0){
            timerText.color = Color.red;
        }
    }

    void DisplayFinishTime(float finishtime) {
        float minutes = Mathf.FloorToInt(finishtime / 60);
        float seconds = Mathf.FloorToInt(finishtime % 60);

        if (minutes < 1) {
            finishTimeText.text = string.Format("You solved the puzzle in {0}s", seconds);
        }
        else {
            finishTimeText.text = string.Format("You solved the puzzle in {0}m{1}s", minutes, seconds);
        }
        finishPanel.SetActive(true);

    }


    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y > 90)
        {
            finishTime = gameTime;
            DisplayFinishTime(finishTime);
        }
        else {
            gameTime += Time.deltaTime;
            DisplayTime(gameTime);
        }
    }

}

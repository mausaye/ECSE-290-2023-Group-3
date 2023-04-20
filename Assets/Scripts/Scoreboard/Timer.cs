using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using Frosty.Scoreboards;

public class Timer : MonoBehaviour
{
    public static float gameTime;
    public bool isAdded = false;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI finishTimeText;
    [SerializeField] private GameObject player;
    private RPC rpc;

    public bool endGameCondition = gameTime > 60;

    // Start is called before the first frame update
    void Start()
    {
        gameTime = 0;
        rpc = this.gameObject.AddComponent(typeof(RPC)) as RPC;
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

        panel.SetActive(true);
        if (minutes < 1) {
            finishTimeText.text = string.Format("You solve the puzzle in {0}s", seconds);
        }
        else {
            finishTimeText.text = string.Format("You solve the puizzle in {0}m{1}s", minutes, seconds);
        }
    }


    // Update is called once per frame
    void Update()
    {
        //change gameTime > 5 to game end condition
        if (player.transform.position.y > 122 && !isAdded)
        {
                DisplayFinishTime(gameTime);
            if (!isAdded) {
                rpc.UploadTime((int)gameTime);
                isAdded = true;
            }
        }
        else {
            gameTime += Time.deltaTime;
            DisplayTime(gameTime);
        }
    }
}

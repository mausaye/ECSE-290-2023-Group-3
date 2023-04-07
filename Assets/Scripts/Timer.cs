using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float gameTime;
    public TextMeshProUGUI timerText;

    // Start is called before the first frame update
    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
        RectTransform rectTransform = timerText.rectTransform;

        float width = timerText.preferredWidth;
        float height = timerText.preferredHeight;

        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(1, 1);

        rectTransform.anchoredPosition = new Vector3(-width / 2, -height / 2, 0);

        gameTime = 0;
        DisplayTime(gameTime);
    }

    void DisplayTime(float timeDisplay){
        float minutes = Mathf.FloorToInt(timeDisplay / 60);
        float seconds = Mathf.FloorToInt(timeDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;
        DisplayTime(gameTime);
    }
}

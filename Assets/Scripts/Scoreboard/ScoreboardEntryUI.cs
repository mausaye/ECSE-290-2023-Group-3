using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


namespace Frosty.Scoreboards
{
    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;

        public void Init(ScoreboardEntryData scoreboardEntryData){
            int time = scoreboardEntryData.time;
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            if (minutes < 1)
            {
                timeText.text = string.Format("{0}s", seconds);
            }
            else
            {
                timeText.text = string.Format("{0}m{1}s", minutes, seconds);
            }
        }
    }
}



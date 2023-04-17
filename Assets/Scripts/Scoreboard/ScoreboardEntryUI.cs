using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


namespace Frosty.Scoreboards
{
    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText = null;

        public void Init(ScoreboardEntryData scoreboardEntryData){
            timeText.text = scoreboardEntryData.time.ToString() + " seconds";
        }
    }
}



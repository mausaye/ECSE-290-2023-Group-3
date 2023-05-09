using UnityEngine;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;


namespace Frosty.Scoreboards{
    public class Scoreboard : MonoBehaviour
    {
        private RPC rpc;
        private List<Score> top5Times;
        [SerializeField] private TextMeshProUGUI time1;
        [SerializeField] private TextMeshProUGUI time2;
        [SerializeField] private TextMeshProUGUI time3;
        [SerializeField] private TextMeshProUGUI time4;
        [SerializeField] private TextMeshProUGUI time5;
        [SerializeField] private TextMeshProUGUI name1;
        [SerializeField] private TextMeshProUGUI name2;
        [SerializeField] private TextMeshProUGUI name3;
        [SerializeField] private TextMeshProUGUI name4;
        [SerializeField] private TextMeshProUGUI name5;

        // Start is called before the first frame update
        void Start()
        {
            rpc = this.gameObject.AddComponent(typeof(RPC)) as RPC;
            GetScoreFromServer((top5Times) => {
                //Debug.Log(top5Times);
                this.top5Times = top5Times;

                // This code will be executed once the top5Times array has been processed
                UpdateUI();
            });
        }

        public void GetScoreFromServer(Action<List<Score>> callback)
        {
            rpc.GetTimes((records) =>
            {
                //Debug.Log(records);
                this.top5Times = records;

                callback(this.top5Times);
            });
        }

        public void UpdateUI()
        {
            time1.text = this.FormatTime(this.top5Times[0].score);
            time2.text = this.FormatTime(this.top5Times[1].score);
            time3.text = this.FormatTime(this.top5Times[2].score);
            time4.text = this.FormatTime(this.top5Times[3].score);
            time5.text = this.FormatTime(this.top5Times[4].score);

            name1.text = this.top5Times[0].name;
            name2.text = this.top5Times[1].name;
            name3.text = this.top5Times[2].name;
            name4.text = this.top5Times[3].name;
            name5.text = this.top5Times[4].name;
        }

        private string FormatTime(int time)
        {
            float minutes = Mathf.FloorToInt(time / 60);
            float seconds = Mathf.FloorToInt(time % 60);
            string formatedTime;

            if (minutes < 1)
            {
                formatedTime = string.Format("{0}s", seconds);
            }
            else
            {
                formatedTime = string.Format("{0}m{1}s", minutes, seconds);
            }
            return formatedTime;
        }
    }
}



using UnityEngine;
using System.IO;
using System;
using System.Collections;
using TMPro;


namespace Frosty.Scoreboards{
    public class Scoreboard : MonoBehaviour
    {
        private RPC rpc;
        private int[] top5Times = new int[5];
        [SerializeField] private TextMeshProUGUI time1;
        [SerializeField] private TextMeshProUGUI time2;
        [SerializeField] private TextMeshProUGUI time3;
        [SerializeField] private TextMeshProUGUI time4;
        [SerializeField] private TextMeshProUGUI time5;        

        // Start is called before the first frame update
        void Start()
        {
            rpc = this.gameObject.AddComponent(typeof(RPC)) as RPC;
            GetScoreFromServer((top5Times) => {
                Debug.Log(top5Times);
                this.top5Times = top5Times;

                // This code will be executed once the top5Times array has been processed
                UpdateUI();
            });
        }

        public void GetScoreFromServer(Action<int[]> callback)
        {
            rpc.GetTimes((times) =>
            {
                times = times.Trim('[', ']');
                string[] strArray = times.Split(',');

                for (int i = 0; i < this.top5Times.Length; i++)
                {
                    this.top5Times[i] = int.Parse(strArray[i]);
                }

                callback(this.top5Times);
            });
        }

        public void UpdateUI()
        {
            time1.text = this.FormatTime(this.top5Times[0]);
            time2.text = this.FormatTime(this.top5Times[1]);
            time3.text = this.FormatTime(this.top5Times[2]);
            time4.text = this.FormatTime(this.top5Times[3]);
            time5.text = this.FormatTime(this.top5Times[4]);
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



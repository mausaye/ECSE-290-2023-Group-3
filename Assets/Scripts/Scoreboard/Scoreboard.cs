using UnityEngine;
using System.IO;
using System;


namespace Frosty.Scoreboards{
    public class Scoreboard : MonoBehaviour
    {
        private static int maxEntries = 5;
        [SerializeField] private Transform highScoresHolderTransform;
        [SerializeField] private GameObject scoreboardEntryObject;

        [Header("test")]
        [SerializeField] ScoreboardEntryData testEntryData = new ScoreboardEntryData();

        private static string savePath = "./Assets/Scripts/Scoreboard/score.json";
        

        // Start is called before the first frame update
        void Start()
        {
            ScoreboardSaveData saveScores = GetSavedScores();
            SaveScores(saveScores);
            UpdateUI(saveScores);
        }

        //Test purpose. Delete later
        [ContextMenu("Add Test Entry")]
        public void AddTestEntry(){
            AddEntry(testEntryData);
        }

        public ScoreboardSaveData GetSavedScores(){
            if (!File.Exists(savePath)){
                File.Create(savePath).Dispose();
                return new ScoreboardSaveData();
            }
            using (StreamReader stream = new StreamReader(savePath)){
                string json = stream.ReadToEnd();

                return JsonUtility.FromJson<ScoreboardSaveData>(json);
            }
        }

        public void SaveScores(ScoreboardSaveData scoreboardSaveData){
            using (StreamWriter stream = new StreamWriter(savePath)){
                string json = JsonUtility.ToJson(scoreboardSaveData, true);

                stream.Write(json);
            }
        }

        public void UpdateUI(ScoreboardSaveData saveScores){
            foreach (Transform child in highScoresHolderTransform)
            {
                Destroy(child.gameObject);
            }

            foreach (ScoreboardEntryData highscore in saveScores.highScores){
                Instantiate(scoreboardEntryObject, highScoresHolderTransform).GetComponent<ScoreboardEntryUI>().Init(highscore);
            }
        }

        public void AddEntry(ScoreboardEntryData scoreboardEntryData){
            ScoreboardSaveData saveScores = GetSavedScores();

            for (int i = 0; i < saveScores.highScores.Count; i++){
                if (scoreboardEntryData.time < saveScores.highScores[i].time){
                    saveScores.highScores.Insert(i, scoreboardEntryData);
                    break;
                }
            }


            if (!saveScores.highScores.Contains(scoreboardEntryData) && saveScores.highScores.Count < maxEntries)
            {
                saveScores.highScores.Add(scoreboardEntryData);
            }

            if (saveScores.highScores.Count > maxEntries){
                saveScores.highScores.RemoveRange(maxEntries, saveScores.highScores.Count - maxEntries);
            }

            UpdateUI(saveScores);
            SaveScores(saveScores);

        }
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Frosty.Scoreboards
{
    [Serializable]
    public class ScoreboardSaveData
    {
        public List<ScoreboardEntryData> highScores = new List<ScoreboardEntryData>();
    }
}



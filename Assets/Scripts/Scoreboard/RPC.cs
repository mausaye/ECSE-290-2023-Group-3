using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
/*
    HOW TO USE: PLEASE READ!

        - If the scene needs to connect to the scoreboard in any way, 
          go to whatever file that you want to call this script from (So like Scoreboard.cs),
          add a field "private RPC rpc;", and then in the Start() function,
          add this line: "rpc = this.gameObject.AddComponent(typeof(RPC)) as RPC;".
          Now, if you want to upload a score, do "rpc.uploadTime(time)". To retrieve 
          the top 5 scores, do "rpc.getTimes()". 

          IMPORTANT NOTE: It will take about 1 second from when you initialize the RPC 
          and for the the scores to be downloaded from the server. Make sure you structure your
          code such that the initialization of the RPC happens a good bit before you need to display scores.

          Note that the same concept applies for uploading. It'll take about 1 second for the scores to download 
          from when you call uploadTime(score). As stated, structure the code such that there's some level
          of delay between when you call uploadTime(score) and when you actually need to display the updated times.
*/



public class RPC : MonoBehaviour
{
    private List<Score> returnedScoreboard;

    // production uri
    private readonly string uri = "http://3.19.53.71:5050/";

    // local uri
    // private readonly string uri = "http://localhost:5050/";

    // Send over game time in seconds, also assigns times with updated scoreboard.
    public void UploadTime(string name, int time) {
        ScoreMessage m = new ScoreMessage();
        m.score = time;
        m.name = name;
        StartCoroutine(PostRequest(uri, JsonUtility.ToJson(m)));
    }

    public List<Score> GetTimes(Action<List<Score>> callback)
    {
        StartCoroutine(GetRequest(uri, (retrivedScoreboard) =>
        {
            this.returnedScoreboard = retrivedScoreboard;
            callback(this.returnedScoreboard);
        }));

        

        return this.returnedScoreboard;
    }

    void Start() 
    {
        // Just so we have something to always display, fetch on initialization.
        GetTimes((times) => Debug.Log(times));
    }

    /* -- Internal stuff. Don't worry about this. -- */

    // Converting classes to JSON is much simpler than converting strings in Unity. Pretty stupid.
    private struct ScoreMessage {
        public string name;
        public int score;
    }

    //edited from Unity docs.
    private IEnumerator GetRequest(string uri, Action<List<Score>> callback) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {

            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    var result = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                    result = "{\"result\":" + result + "}";
                    var resultScoreList = JsonHelper.FromJson<Score>(result);
                    this.returnedScoreboard = resultScoreList;
                    break;
            }
        }

        callback(this.returnedScoreboard);
    }

    //also edited from Unity docs.
    private IEnumerator PostRequest(string uri, string json)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, json))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("Error While Sending: " + webRequest.error);
            }
            else
            {
                if (webRequest.isDone)
                {
                    // handle the result
                    var result = System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data);
                    result = "{\"result\":" + result + "}";
                    var resultScoreList = JsonHelper.FromJson<Score>(result);
                    this.returnedScoreboard = resultScoreList;
                }
                else
                {
                    //handle the problem
                    Debug.Log("Error! data couldn't get.");
                }
            }
        }
    }
}
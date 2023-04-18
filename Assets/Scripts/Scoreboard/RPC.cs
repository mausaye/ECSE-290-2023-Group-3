using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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
    private string returnedScoreboard;
    // Send over game time in seconds, also assigns times with updated scoreboard.
    public void uploadTime(int time) {
        ScoreMessage m = new ScoreMessage();
        m.score = time;
        StartCoroutine(PostRequest("http://3.19.53.71:5050/", JsonUtility.ToJson(m)));
    }

    // Return the times as a string in the format [time1, time2, time3, etc]. Sorted in descending order.
    public string getTimes() {
        return this.returnedScoreboard;
    }

    void Start() 
    {
        returnedScoreboard = "UNASSIGNED";
        // Just so we have something to always display, fetch on initialization.
        fetchInitialScoreboardData();
    }







    /* -- Internal stuff. Don't worry about this. -- */

    // Converting classes to JSON is much simpler than converting strings in Unity. Pretty stupid.
    private struct ScoreMessage {
        public int score;
    }


    private void fetchInitialScoreboardData() {
        if (returnedScoreboard == "UNASSIGNED") {
            StartCoroutine(GetRequest("http://3.19.53.71:5050/"));
        }
    }

    //edited from Unity docs.
    private IEnumerator GetRequest(string uri) {
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
                    this.returnedScoreboard = webRequest.downloadHandler.text;
                    break;
            }
        }
    }

    //also edited from Unity docs.
    private IEnumerator PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            this.returnedScoreboard = uwr.downloadHandler.text;
        }
    }

    

}
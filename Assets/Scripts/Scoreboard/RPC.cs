using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public struct ScoreMessage {
    public int score;
}
public class RPC : MonoBehaviour
{

    private string returnedScoreboard = "UNASSIGNED";

    void Start() 
    {
        // Just so we have something to always display
        fetchInitialScoreboardData();
    }

    // Send over game time in seconds, also assigns times with updated scoreboard.
    public void uploadTime(int time) {
        ScoreMessage m = new ScoreMessage();
        m.score = time;
        StartCoroutine(PostRequest("http://3.19.53.71:5050/", JsonUtility.ToJson(m)));
    }

    public void fetchInitialScoreboardData() {
        if (returnedScoreboard == "UNASSIGNED") {
            StartCoroutine(GetRequest("http://3.19.53.71:5050/"));
        }
    }

    public string getTimes() {
        return returnedScoreboard;
    }

    //from Unity docs
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
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }

    

}
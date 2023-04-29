using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmitButton : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameText;
    private RPC rpc;
    private string playerName;

    void Start()
    {
        rpc = this.gameObject.AddComponent(typeof(RPC)) as RPC;
    }

    public void OnSubmit()
    {
        this.playerName = nameText.text;
        rpc.UploadTime(this.playerName, (int)Timer.finishTime);
        SceneManager.LoadScene("Scoreboard", LoadSceneMode.Single);
    }
}

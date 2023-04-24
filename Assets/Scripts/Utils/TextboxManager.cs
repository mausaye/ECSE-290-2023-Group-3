using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TextboxManager : MonoBehaviour 
{
    private static GameObject textbox;
    private static Text dialogueText;

    void Start() {
        textbox = GameObject.FindWithTag("Textbox");
        dialogueText = GetComponentInChildren<Text>();
        dialogueText.fontSize = 32;
        textbox.SetActive(false);
    }

    void Update() {
    }

    public static void createTextbox(string initialText) {
        checkTextSize(initialText);
        textbox.SetActive(true);
        dialogueText.text = initialText;
    }

    public static void setText(string newText) {
        checkTextSize(newText);
        dialogueText.text = newText;
    }

    public static void removeTextbox() {
        textbox.SetActive(false);
    }

    private static void checkTextSize(string s) {
        if (s.Length > 60) {
            throw new Exception(String.Format("This piece of text is too long. Stuff might be cut off: {0}", s));
        }
    }

}
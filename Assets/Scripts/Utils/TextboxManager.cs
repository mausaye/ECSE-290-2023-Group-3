using UnityEngine;
using TMPro;

public class TextboxManager : MonoBehaviour 
{
    private static GameObject textbox;
    private static TextMeshProUGUI dialogueText;
    private static bool visible = false;

    void Start() {
        textbox = GameObject.FindWithTag("Textbox");
        dialogueText = Object.FindObjectOfType<TextMeshProUGUI>();
        textbox.transform.localScale = new Vector3(0, 0, 0);
    }

    void Update() {
        if (visible) {
            fitToScreen();
        }
    }

    public static void createTextbox(string initialText) {
        visible = true;
        dialogueText.text = initialText;
        fitToScreen();
    }

    public static void setText(string newText) {
        dialogueText.text = newText;
    }

    public static void removeTextbox() {
        textbox.transform.localScale = new Vector3(0, 0, 0);
        visible = false;
    }

    public static void fitToScreen() {
        textbox.transform.localScale = new Vector3((3 * (Screen.width / ((RectTransform)textbox.transform).rect.width)) / 4, (Screen.height / ((RectTransform)textbox.transform).rect.height) / 7, 1.0f);
        textbox.transform.localPosition = new Vector3(0, -175, 0);
        dialogueText.transform.localPosition = new Vector3(-100, -100, -100);
    }
}
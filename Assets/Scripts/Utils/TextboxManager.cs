using UnityEngine;
using TMPro;

public class TextboxManager : MonoBehaviour 
{
    [SerializeField] private static GameObject textbox;
    [SerializeField] private static TextMeshProUGUI dialogueText;

    void Start() {
        textbox = GameObject.FindWithTag("Textbox");
        dialogueText = GetComponent<TextMeshProUGUI>();
        textbox.transform.localScale = new Vector3(0, 0, 0);
    }

    public static void createTextbox(string initialText) {
        textbox.transform.localScale = new Vector3((3 * (Screen.width / ((RectTransform)textbox.transform).rect.width)) / 4, (Screen.height / ((RectTransform)textbox.transform).rect.height) / 7, 1.0f);
    }

    public static void setText(string newText) {

    }

    public static void removeTextbox() {
        textbox.transform.localScale = new Vector3(0, 0, 0);

    }
}
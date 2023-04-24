using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHatNPC : InteractableNPC
{
    int textInd = 0;
    public override void onInteraction() {
        convo(new string[] {"Yo,", "wassup", "this is a longer piece of text to see if it'll wrap around."}, ref textInd);
    }
}

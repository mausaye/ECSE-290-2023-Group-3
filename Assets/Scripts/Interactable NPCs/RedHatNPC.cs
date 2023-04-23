using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHatNPC : InteractableNPC
{
    int textInd = 0;
    public override void onInteraction() {
        convo(new string[] {"Yo,", "wassup"}, ref textInd);
    }
}

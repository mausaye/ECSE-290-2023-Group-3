using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHatNPC : InteractableNPC
{
    public override void onInteraction() {
        Debug.Log("interaction successful");
    }
}

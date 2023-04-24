/*
    How to make a basic convo NPC
        - Put down the NPC prefab. This script should already be attached.
        - Change the sprite to be whatever you want.
        - Set the 'Text' field in the editor. Each element represents what will be shown contiguously, in order. So text in element 0 is shown, and once the player presses space, text in element 1 is shown, etc.

*/

public class BasicConvoNPC : InteractableNPC
{
    private int textInd = 0;
    public string[] text;

    public override void onInteraction() {
        convo(text, ref textInd);
    }
}

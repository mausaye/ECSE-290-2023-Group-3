using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour
{
    protected GameObject player;
    protected Vector2 playerPos;
    private const double RANGE_THRESHOLD = 3.0; //maximum range in which the player is "near" this NPC. 

    protected PlayerInfo playerInfo = PlayerInfo.Instance;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        playerPos = player.transform.position;
        //if the player is close and facing me, there should be some indicator that they can interact with me
        if (playerInteractionPossible()) {

            //if the interaction can happen and they press enter, they're trying to start an interaction.
            if (interactButtonPressed()) {
                onInteraction();
            }
        }
    }

    protected bool playerInteractionPossible() {
        return playerInRange() && playerFacingMe();
    }

    protected bool playerBeganInteraction() {
        return playerInteractionPossible() && interactButtonPressed();
    }

    private bool playerInRange() {
        Vector2 myPos = transform.position;
        double dist = Math.Sqrt(Math.Pow(playerPos.x - myPos.x, 2) + Math.Pow(playerPos.y - myPos.y, 2));
        return dist < RANGE_THRESHOLD;
    }

    //Note that this function is only called if the player is within 3 units. Wouldn't work well otherwise.
    private bool playerFacingMe() {
        Vector2 myPos = transform.position; 
        Direction d = playerInfo.getLastDirection();

        //could be one refactored into one giant OR statement.
        if (playerPos.x > myPos.x && (d == Direction.MOVE_LEFT || d == Direction.IDLE_LEFT)) //if player is to the right
            return true;
        else if (playerPos.x < myPos.x && (d == Direction.MOVE_RIGHT || d == Direction.IDLE_RIGHT)) {//if player is to the left
            return true;
        }
        else if (playerPos.y > myPos.y && (d == Direction.MOVE_DOWN || d == Direction.IDLE_DOWN)) { //player is above me 
            return true;
        }
        else if (playerPos.y < myPos.y && (d == Direction.MOVE_UP || d == Direction.IDLE_UP)) { //player is below me 
            return true;
        }

        return false;
    }

    protected void convo(string[] text, ref int textInd) {
        if (textInd == 0) { // box not up yet 
            playerInfo.setConvoStatus(true);
            TextboxManager.createTextbox(text[textInd++]);
        }
        else if (textInd < text.Length) { // if there is still stuff to say
            TextboxManager.setText(text[textInd++]);
        }
        else { // nothing left to say
            textInd = 0;
            playerInfo.setConvoStatus(false);
            TextboxManager.removeTextbox();
        }
    }

    protected bool interactButtonPressed() {
        return Input.GetKeyDown(KeyCode.Space);
    }

    //function defines what the NPC should do/say once the player interacts with them.
    public abstract void onInteraction();
}

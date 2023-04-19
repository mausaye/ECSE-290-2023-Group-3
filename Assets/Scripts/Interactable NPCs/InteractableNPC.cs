using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableNPC : MonoBehaviour
{
    private GameObject player;
    private Vector2 playerPos;
    private const double RANGE_THRESHOLD = 5.0; //maximum range in which the player is "near" this NPC. 

    private PlayerInfo playerInformation;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        
    }

    void Update()
    {
        playerPos = player.transform.position;
        //if the player is close and facing me, there should be some indicator that they can interact with me
        if (playerInteractionPossible()) {
            //Insert logic indicator here.

            //if the interaction can happen and they press enter, they're trying to start an interaction.
            if (interactButtonPressed()) {
                onInteraction();
            }
        }

    }

    private bool playerInteractionPossible() {
        return playerInRange() && playerFacingMe();
    }

    private bool playerBeganInteraction() {
        return playerInteractionPossible() && interactButtonPressed();
    }

    private bool playerInRange() {
        Vector2 myPos = transform.position;
        double dist = Math.Sqrt(Math.Pow(playerPos.x - myPos.x, 2) + Math.Pow(playerPos.y - myPos.y, 2));
        Debug.Log(dist);
        return dist < RANGE_THRESHOLD;
    }

    //TODO. Probably need to do a raycast or something like that. Since the rotation is locked for the sake of the camera,
    //we might have to base it off of the last direction the player had been moving.
    private bool playerFacingMe() {
        
        return true;
    }

    //TODO. Interact button will probably be space or something, just need to check if that key is pressed.
    private bool interactButtonPressed() {
        return Input.GetKeyDown(KeyCode.Space);
    }

    //function defines what the NPC should do/say once the player interacts with them.
    public abstract void onInteraction();
}

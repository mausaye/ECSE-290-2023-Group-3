using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using System;

public class WanderingNPC : InteractableNPC
{
    private enum State {
        WALKING, 
        STOPPED,
        IN_CONVO
    }

    private State state;
    private float timer = 0;
    private int textInd = 0;
    private bool up = true; //walking up to start;

    private float timeLeft = 0.0f;

    public string[] text;
    public int speed;
    public float timeForDirection = 3.0f;

    private bool wasWalking = false;

    private Rigidbody2D rb2d;

    void Start() {
        player = GameObject.FindWithTag("Player");
        rb2d = GetComponent<Rigidbody2D> ();
    }

    void Update() {
        playerPos = player.transform.position;
        //if the player is close and facing me, there should be some indicator that they can interact with me
        if (playerInteractionPossible()) {

            //if the interaction can happen and they press enter, they're trying to start an interaction.
            if (interactButtonPressed()) {
                onInteraction();
            }
        }

        if (state == State.WALKING) {
            move();
        }

        if (state == State.IN_CONVO && !playerInfo.isInConvo()) {
            if (wasWalking)
                state = State.WALKING;
            else 
                state = State.STOPPED;
            wasWalking = false;
        }

        //do nothing if stopped.

        if (state != State.IN_CONVO)
            timer += Time.deltaTime;

        if (state == State.WALKING && timer > timeForDirection) {
            state = State.STOPPED;
            timer = 0;
        }
        else if (state == State.STOPPED && timer > timeForDirection) {
            state = State.WALKING;
            up = !up;
            timer = 0;
        }
    }

    public override void onInteraction() {
        if (state == State.WALKING) {
            wasWalking = true;
        }
        state = State.IN_CONVO;
        convo(text, ref textInd);
    }

    private void move() {
        if (up) {
            rb2d.position = rb2d.position + new Vector2(0, speed * Time.deltaTime);
        }
        else {
            rb2d.position = rb2d.position - new Vector2(0, speed * Time.deltaTime);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using System;


/*
    Same purpose as the PlayerMovement script, but FSM based. Theoretically, this will make it easier to understand and bugs will be easier to track.
*/
public class PlayerMovement : MonoBehaviour {
    private enum PlayerState {
        Moving_On_Ground,
        Stopped_On_Ground,
        Moving_On_Ice,
        Stopped_On_Ice,
    }

    public float speed;
    private PlayerInfo playerInformation = PlayerInfo.Instance;

    private PlayerState state;
    private Rigidbody2D rb2d;
    private Vector2 delta;
    public Animator animator;
    public Tilemap snowTiles;
    public Tilemap iceTiles;


    void Start() {
        rb2d = GetComponent<Rigidbody2D> ();
        rb2d.freezeRotation = true;
        delta = new Vector2(0.0f, 0.0f);

    }

    void Update() {
        //Step 1 of update: Figure out what state we're in
        Tile tile = getTileUnderMe();

        //new deltas
        float deltaX = Input.GetAxisRaw("Horizontal") * speed;
        float deltaY = Input.GetAxisRaw("Vertical") * speed;
        Vector2 newDelta = new Vector2(deltaX, deltaY);
        playerInformation.setGridPosition(this.transform.position);
        if (tile == Tile.ICE) {
            // First check is so it isn't repeatedly reassigned.
            if (state != PlayerState.Stopped_On_Ice && deltaX == 0 && deltaY == 0) {
                Debug.Log("state change");
                state = PlayerState.Stopped_On_Ice;
            }
            else if (state != PlayerState.Moving_On_Ice && ((deltaX != 0) || (deltaY != 0))) {
                Debug.Log("state change");
                state = PlayerState.Moving_On_Ice;
            }
        }
        else if (tile == Tile.NORMAL_GROUND) {
            if (state != PlayerState.Stopped_On_Ground && deltaX == 0 && deltaY == 0) {
                Debug.Log("state change: Stopped on normal ground");
                state = PlayerState.Stopped_On_Ground;
            }
            else if ((state != PlayerState.Moving_On_Ground) && ((deltaX != 0) || (deltaY != 0))) {
                Debug.Log("state change : started moving on ground");
                state = PlayerState.Moving_On_Ground;
            }
        }




        //Step 2 of update: Perform actions based on this new state.
        switch (state) {
            case PlayerState.Moving_On_Ground:
                Direction lastDir = playerInformation.getLastDirection();
                playerInformation.setLastDirection(delta.x, delta.y, deltaX, deltaY);
                if (lastDir != playerInformation.getLastDirection())
                    triggerAnimator(playerInformation.getLastDirection());
                rb2d.position = rb2d.position + newDelta * Time.deltaTime;
                delta = newDelta;
                break;
            case PlayerState.Stopped_On_Ground:
                lastDir = playerInformation.getLastDirection();
                playerInformation.setLastDirection(delta.x, delta.y, deltaX, deltaY);
                if (lastDir != playerInformation.getLastDirection())
                    triggerAnimator(playerInformation.getLastDirection());
                rb2d.position = rb2d.position + newDelta * Time.deltaTime;
                delta = newDelta;
                break;
            case PlayerState.Moving_On_Ice:
                break;
            case PlayerState.Stopped_On_Ice:

                break;
        }
        //Debug.Log(state);
    }
    private Tile getTileUnderMe() {
        int2 pos = playerInformation.getGridPosition();
        Vector3Int vecPos = new Vector3Int(pos.x, pos.y, 0);
        if (iceTiles.HasTile(vecPos)) {
            return Tile.ICE;
        }
        else {
            return Tile.NORMAL_GROUND;
        }
    }

    private void triggerAnimator(Direction d) {
        animator.SetTrigger(d.ToString());
    }
}
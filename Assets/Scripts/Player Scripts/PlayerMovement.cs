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
    public Tilemap boundaryTiles;


    void Start() {
        rb2d = GetComponent<Rigidbody2D> ();
        rb2d.freezeRotation = true;
        delta = new Vector2(0.0f, 0.0f);

    }

    void Update() {
        int2 pos = playerInformation.getGridPosition();
        //Step 1 of update: Figure out what state we're in
        Tile tile = getTileUnderMe();

        //new deltas
        float deltaX = Input.GetAxisRaw("Horizontal") * speed;
        float deltaY = Input.GetAxisRaw("Vertical") * speed;
        Vector2 newDelta = new Vector2(deltaX, deltaY);
        playerInformation.setGridPosition(this.transform.position);
        if (tile == Tile.ICE) {

            // If on ice at all, check nearby for boundaries. Set us to stopped on ice if we detect one.
            if (delta.x != 0 || delta.y != 0) {
                if (collidedWithBoundary()) {
                    delta.x = 0;
                    delta.y = 0;
                    Debug.Log("collision detected");
                }
            }

            if (state == PlayerState.Stopped_On_Ice && (deltaX != 0 || deltaY != 0)) {
                delta = new Vector2(deltaX, deltaY);
                state = PlayerState.Moving_On_Ice;
            }

            if (state != PlayerState.Stopped_On_Ice && delta.x == 0 && delta.y == 0) {
                Debug.Log("state change : Stopped on ice");
                state = PlayerState.Stopped_On_Ice;
            }
            else if (state != PlayerState.Moving_On_Ice && ((delta.x != 0) || (delta.y != 0))) {
                //moving on ice and boundary nearby, we stopped.
                Debug.Log("state change : started moving on ice");
                state = PlayerState.Moving_On_Ice;
            }
        }
        else if (tile == Tile.NORMAL_GROUND) {
            if (state != PlayerState.Stopped_On_Ground && deltaX == 0 && deltaY == 0) {
                //Debug.Log("state change: Stopped on normal ground");
                state = PlayerState.Stopped_On_Ground;
            }
            else if ((state != PlayerState.Moving_On_Ground) && ((deltaX != 0) || (deltaY != 0))) {
                //Debug.Log("state change : started moving on ground");
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
                rb2d.position = rb2d.position + delta * Time.deltaTime;
                break;
            case PlayerState.Stopped_On_Ice:
                rb2d.position = rb2d.position + newDelta * Time.deltaTime;

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

    private bool collidedWithBoundary() {
        int2 pos = playerInformation.getGridPosition();
        Vector3Int vecPos = new Vector3Int(pos.x, pos.y, 0);

        //was moving right. If there's a tile 1 to the right, we've collided.
        if (delta.x > 0) {
            return boundaryTiles.HasTile(new Vector3Int(pos.x + 1, pos.y, 0));
        }
        return false;

    }
}
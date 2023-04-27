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
    public float slideSpeed;
    private PlayerInfo playerInformation = PlayerInfo.Instance;

    private PlayerState state;
    private Rigidbody2D rb2d;
    private Vector2 delta;
    public Animator animator;
    public Tilemap snowTiles;
    public Tilemap iceTiles;
    public Tilemap boundaryTiles;
    public Tile tile;

    private const float tileShade = 0.6f; //opacity % for the tile you're under.
    [SerializeField] private AudioSource stepSoundEffect;
    [SerializeField] private AudioSource iceSoundEffect;


    void Start() {
        rb2d = GetComponent<Rigidbody2D> ();
        rb2d.freezeRotation = true;
        delta = new Vector2(0.0f, 0.0f);

    }


    private bool inPuzzle() {
        float x = transform.position.x;
        float y = transform.position.y;
        return inFirstPuzzle(x, y) || inSecondPuzzle(x, y) || inThirdPuzzle(x, y);
    }

    private bool inFirstPuzzle(float x, float y) {
        return ((x>= -4 && x <= 13) && (y >= 17 && y <= 33));
    }

    private bool inSecondPuzzle(float x, float y) {
        return ((x>= -5 && x <= 14) && (y >= 36 && y <= 53));
    }

    private bool inThirdPuzzle(float x, float y) {
        return ((x>= -6 && x <= 16) && (y >= 57 && y <= 77));
    }



    void Update() {
        //Step 1 of update: Figure out what state we're in, get info for next frame.
        int2 pos = playerInformation.getGridPosition();

        tile = getTileUnderMe();

        //could make this only highlight when on a puzzle.
        highlightTileUnderMe();

        if (inPuzzle()) {
            GetComponent<CapsuleCollider2D>().enabled = false;
        }
        else {
            GetComponent<CapsuleCollider2D>().enabled = true;
        }
        //Debug.Log(GetComponent<CapsuleCollider2D>().enabled);

        if (playerInformation.isInConvo()) {
            return;
        }

        //new deltas
        float deltaX = Input.GetAxisRaw("Horizontal") * speed;
        float deltaY = deltaX == 0 ? Input.GetAxisRaw("Vertical") * speed : 0;

        // for sliding velocities.
        float deltaXS = Input.GetAxisRaw("Horizontal") * slideSpeed;
        float deltaYS = Input.GetAxisRaw("Vertical") * slideSpeed;

        Vector2 newDelta = new Vector2(deltaX, deltaY);
        Vector2 newDeltaS = new Vector2(deltaXS, deltaYS);
        playerInformation.setGridPosition(this.transform.position);

        if (tile == Tile.ICE) {
            if (collidedWithBoundary()) {
                delta = new Vector2(deltaXS, deltaYS);
                return;
            }

            if (state == PlayerState.Moving_On_Ground) { //ground to ice translation
                delta = new Vector2(deltaXS, deltaYS);
            }

            if (state == PlayerState.Stopped_On_Ice && (deltaX != 0 || deltaY != 0)) {
                delta = new Vector2(deltaXS, deltaYS);
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


        /* --------------------------------------------------------- */

        //Step 2 of update: Perform actions based on this new state.
        switch (state) {
            case PlayerState.Moving_On_Ground:
                 stepSoundEffect.Play();
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
                iceSoundEffect.Play();
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
        //same concept for the rest.
        else if (delta.x < 0) {
            return boundaryTiles.HasTile(new Vector3Int(pos.x - 1, pos.y, 0));
        }
        else if (delta.y > 0) {
            return boundaryTiles.HasTile(new Vector3Int(pos.x, pos.y + 1, 0));
        }
        else if (delta.y < 0) {
            return boundaryTiles.HasTile(new Vector3Int(pos.x, pos.y - 1, 0));
        }
        return false;
    }

    private void highlightTileUnderMe() {
        int2 pos = playerInformation.getGridPosition();
        Vector3Int vecPos = new Vector3Int(pos.x, pos.y, 0); //position must be a 3d vec, and everything is on 0.

        //A tile we were just on might have it's color modifiedd and we're no longer on it, so reset those.
        resetSurroundingTiles(vecPos);

        switch (tile) {
            case Tile.ICE:
                iceTiles.SetTileFlags(vecPos, TileFlags.None);
                iceTiles.SetColor(vecPos, new Color(1.0f, 1.0f, 1.0f, tileShade));
                break;
            case Tile.NORMAL_GROUND:
                snowTiles.SetTileFlags(vecPos, TileFlags.None);
                snowTiles.SetColor(vecPos, new Color(1.0f, 1.0f, 1.0f, tileShade));
                break;
        }
    }

    private void resetSurroundingTiles(Vector3Int vecPos) {
        int[] di = {0, 1, 0, -1, 1, 1, -1, -1};
        int[] dj = {1, 0, -1, 0, -1, 1, -1, 1};
        for (int k = 0; k < 8; k++) {
                Vector3Int adjacentTilePos = new Vector3Int(vecPos.x + di[k], vecPos.y + dj[k]);
                iceTiles.SetTileFlags(adjacentTilePos, TileFlags.None);
                snowTiles.SetTileFlags(adjacentTilePos,  TileFlags.None);
                iceTiles.SetColor(adjacentTilePos, new Color(1.0f, 1.0f, 1.0f, 1.0f));
                snowTiles.SetColor(adjacentTilePos, new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }
    }
}

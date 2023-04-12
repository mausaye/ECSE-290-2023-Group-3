using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using System;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;
    private PlayerInfo playerInformation = PlayerInfo.Instance;
    public Animator animator;
    
    //to see which direction is the most "recent" press
    private float prevDeltaX = 0.0f;
    private float prevDeltaY = 0.0f;
    // To see when the player stops moving on ice
    private Vector2 prevPos; 
    private int frameCount; // check every x frames if position has changed.


    public Tilemap snowTiles;
    public Tilemap iceTiles;



    void Start()
    {
        rb2d = GetComponent<Rigidbody2D> ();
        rb2d.freezeRotation = true;
    }

    void moveNormally() {
        //get change in X and Y, but if on ice then should remain unchanged from previous state to constantly move in same direction
        float deltaX = Input.GetAxisRaw("Horizontal") * speed;
        float deltaY = Input.GetAxisRaw("Vertical") * speed;
        Direction lastDir = playerInformation.getLastDirection();
        playerInformation.setLastDirection(prevDeltaX, prevDeltaY, deltaX, deltaY);


        //only indicate to animator when the direction is actually changed.
        //sending a message each time causes the animation to reset each frame, which is obviously bad.
        if (lastDir != playerInformation.getLastDirection())
            triggerAnimator(playerInformation.getLastDirection());


        Vector2 delta = new Vector2(deltaX , deltaY);

        //set position (FPS invariant)
        rb2d.position = rb2d.position + delta * Time.deltaTime;

        prevDeltaX = deltaX;
        prevDeltaY = deltaY;
    }

    void moveOnIce() {
        Vector2 delta = new Vector2(prevDeltaX, prevDeltaY);
        rb2d.position = rb2d.position + delta * Time.deltaTime;

        Vector2 curPos = rb2d.position;
    }


    void Update()
    {
        //set position in 2d grid.
        playerInformation.setGridPosition(this.transform.position);
        Tile tile = getTileUnderMe();
        switch (tile) {
            case Tile.ICE:
                moveOnIce();
                break;
            default:
                moveNormally();
                break;
        }
        frameCount++;
    }


    private void triggerAnimator(Direction d) {
        animator.SetTrigger(d.ToString());
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
}

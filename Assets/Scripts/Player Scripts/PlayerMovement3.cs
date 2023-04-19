/*

    -- Depreciated. Just using this as a reference for the new script. -- t 



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using System;

public class PlayerMovement3 : MonoBehaviour
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
    private RPC rpc;
    private Vector2 delta;


    void Start()
    {
        //rpc = this.gameObject.AddComponent(typeof(RPC)) as RPC;
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


        delta = new Vector2(deltaX , deltaY);

        //set position (FPS invariant)
        rb2d.position = rb2d.position + delta * Time.deltaTime;

        prevDeltaX = deltaX;
        prevDeltaY = deltaY;
    }

    void moveOnIce() {
        //stopped on ice.
        if (delta.x == 0 && delta.y == 0) {
            float deltaX = 1.0f * Input.GetAxisRaw("Horizontal") * speed;
            float deltaY = 1.0f * Input.GetAxisRaw("Vertical") * speed;
            Direction lastDir = playerInformation.getLastDirection();
            playerInformation.setLastDirection(prevDeltaX, prevDeltaY, deltaX, deltaY);


            //only indicate to animator when the direction is actually changed.
            //sending a message each time causes the animation to reset each frame, which is obviously bad.
            if (lastDir != playerInformation.getLastDirection())
                triggerAnimator(playerInformation.getLastDirection());


            delta = new Vector2(deltaX , deltaY);

            //set position (FPS invariant)
            rb2d.position = rb2d.position + delta * Time.deltaTime;

            prevDeltaX = deltaX;
            prevDeltaY = deltaY;
        }

        else {
            /*
            to not allow them to slide diagonally. If they're moving in diagonally,
            restrict them to just the y. Somewhat arbitrary.
            */
            /*
            if (Math.Abs(prevDeltaX) > 0.01 && Math.Abs(prevDeltaY) > 0.01) {
                prevDeltaX = 0;
            }

            delta = new Vector2(prevDeltaX, prevDeltaY);
            rb2d.position = rb2d.position + delta * Time.deltaTime;
        }
    }


    void OnCollisionEnter2D(Collision2D collision) {
        playerInformation.setLastDirectionAsIdle(playerInformation.getLastDirection()); //halt animation
        prevDeltaX = 0;
        prevDeltaY = 0;
        if (getTileUnderMe() == Tile.ICE) {

        }
    }


    void Update()
    {
        Debug.Log(prevDeltaX);
        Debug.Log(prevDeltaY);
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
*/
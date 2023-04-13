using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public enum Direction {
    MOVE_UP,
    MOVE_DOWN,
    MOVE_LEFT,
    MOVE_RIGHT,
    IDLE_UP,
    IDLE_DOWN,
    IDLE_LEFT,
    IDLE_RIGHT
}

public enum Tile {
    NORMAL_GROUND,
    ICE
}

//Singleton so the same information is seen everywhere
public sealed class PlayerInfo {
    public static PlayerInfo instance = null;
    private static readonly object threadLock = new object();
    private Direction mostRecentDirection;
    private int2 gridPos = new int2(0, 0);


    private PlayerInfo() {
        mostRecentDirection = Direction.IDLE_DOWN;
    }
    public static PlayerInfo Instance {
        get {
            lock (threadLock) {
                if (instance == null)
                    instance = new PlayerInfo();
                return instance;
            }
        }
    }

    public void setLastDirectionAsIdle(Direction d) {
        switch (d) {
            case Direction.MOVE_DOWN:
                mostRecentDirection = Direction.IDLE_DOWN;
                break;
            case Direction.MOVE_UP:
                mostRecentDirection = Direction.IDLE_UP;
                break;
            case Direction.MOVE_LEFT:
                mostRecentDirection = Direction.IDLE_LEFT;
                break;
            case Direction.MOVE_RIGHT:
                mostRecentDirection = Direction.IDLE_RIGHT;
                break;
            default:
                mostRecentDirection = Direction.IDLE_DOWN;
                break;
        }
    }
    //for the sake of making the animation look good. Could probably be simplified a bit.
    public void setLastDirection(float prevDX, float prevDY, float dX, float dY) {
        //idling 
        if (dX == 0 && dY == 0) {
            Direction prevDir = mostRecentDirection;
            switch (prevDir) {
                case Direction.MOVE_LEFT:
                    mostRecentDirection = Direction.IDLE_LEFT;
                    break;
                case Direction.MOVE_RIGHT:
                    mostRecentDirection = Direction.IDLE_RIGHT;
                    break;
                case Direction.MOVE_UP:
                    mostRecentDirection = Direction.IDLE_UP;
                    break;
                case Direction.MOVE_DOWN:
                    mostRecentDirection = Direction.IDLE_DOWN;
                    break;

            }
        }


        //if you start moving horizontally
        if (dX != 0 && dX != prevDX) {
            if (dX > 0)
                mostRecentDirection = Direction.MOVE_RIGHT;
            else if (dX < 0)
                mostRecentDirection = Direction.MOVE_LEFT;
        }

        //if you start moving vertically
        if (dY != 0 && dY != prevDY) {
            if (dY > 0)
                mostRecentDirection = Direction.MOVE_UP;
            else if (dY < 0)
                mostRecentDirection = Direction.MOVE_DOWN;
        }

        //if you let go of a horizontal keypress
        if (dX == 0 && dY != 0) {
            if (dY > 0)
                mostRecentDirection = Direction.MOVE_UP;
            else if (dY < 0)
                mostRecentDirection = Direction.MOVE_DOWN;
        }

        //if you let go of a vertical keypress
        if (dY == 0 && dX != 0) {
            if (dX > 0)
                mostRecentDirection = Direction.MOVE_RIGHT;
            else if (dX < 0)
                mostRecentDirection = Direction.MOVE_LEFT;
        }
    }

    public Direction getLastDirection() {
        return mostRecentDirection;
    }

    public void setGridPosition(Vector3 position) {
        /* 
            Why -2? I'm not 100% sure, but I think it's because the position of the player
            is basically stored at the top left (i.e. top left of the head of the sprite).
            The sprite is basically 2 blocks big, so the '-2' takes care of things.
            

        */
        this.gridPos = new int2((int)Math.Floor(position.x), (int)Math.Floor(position.y + 1) - 2);
    }

    public int2 getGridPosition() {
        return this.gridPos;
    }
}
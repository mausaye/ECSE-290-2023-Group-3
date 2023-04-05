using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

//Singleton so the same information is seen everywhere
public sealed class PlayerInfo {
    public static PlayerInfo instance = null;
    private static readonly object threadLock = new object();
    private Direction mostRecentDirection;


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
}
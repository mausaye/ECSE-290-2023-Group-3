using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class TerrianGenerator : MonoBehaviour
{

    public Tilemap iceTiles;
    public Tilemap snowTiles; 
    public Tilemap boundaryTiles;
    public Tilemap inanimateObjectTiles;
    public UnityEngine.Tilemaps.Tile snowTile;
    public UnityEngine.Tilemaps.Tile iceTile;
    public UnityEngine.Tilemaps.Tile inPuzzleBoundaryTile;
    public UnityEngine.Tilemaps.Tile edgeBoundaryTile;
    public int difficulty = -1;
    private char[,] puzzle;


    char[,] chooseRandomPuzzle(int difficulty) {
        //TODO: make folders for various difficultys, choose one appropriately.
        return PuzzleDecoder.decode("Assets/Resources/GoodPuzzles/puzzle0.ice");
    }

    void Start()
    {
        if (difficulty == -1) {
            throw new Exception("Difficulty for the puzzle to be generated not set. Set the \"Difficulty\" in the editor.");
        } 
        else if (difficulty < 1 || difficulty > 3) {
            throw new Exception("difficulty set to invalid integer. Please set it between 1 and 3 (inclusive).");
        }

        puzzle = chooseRandomPuzzle(difficulty);
        int n = (int)Math.Sqrt(puzzle.Length); //because C# is stupid, Length = n * n.
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                //TODO: Switch this to a switch statement.
                if (puzzle[j, i] == 'S') {
                    snowTiles.SetTile(pos, snowTile);
                }
                else if (puzzle[j, i] == 'B') {
                    inanimateObjectTiles.SetTile(pos, inPuzzleBoundaryTile);
                }
                else if (puzzle[j, i] == '_') {
                    inanimateObjectTiles.SetTile(pos, edgeBoundaryTile);
                }
                else if (puzzle[j, i] == 'I') {
                    iceTiles.SetTile(pos, iceTile);
                }
            }
        }
    }

    void Update()
    {

    }
}

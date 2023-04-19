using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


/* 
    HOW TO USE: Please read!
        - If you want to generate terrian in a scene, throw this file on the Grid in the Unity Editor. From there, drag in whatever 
          tile should be rendered for the ice tiles, snow tiles, etc all within the Unity editor. Finally, define where the top-left corner 
          of each puzzle should be. You must define the x and y coordinate in the puzzle 1 offset, puzzle 2 offset, and puzzle 3 offset boxes.
          The best way to figure out where you want the puzzle is probably just trial and error.

        - If you want to see this in action with an example, go to the Test Puzzle scene and click on the grid.

        - NOTE: There also exists the "Puzzle Difficulties" field that you can edit in the Unity editor. Once we have a good way to retrieve 
          puzzles in folders specifically designating their difficulty, these can be used to define how hard each puzzle should be. 1 is easy, 2
          is medium, 3 is hard. They'll probably just always maintain the order and numbering they have now, with the first puzzle being the easiest,
          the second puzzle being the second hardest, and the third puzzle being the hardest.
*/
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
    public int[] puzzleDifficulties = { 1, 2, 3 };
    public Vector2Int puzzle1Offset;
    public Vector2Int puzzle2Offset;
    public Vector2Int puzzle3Offset;

    private List<Vector2Int> offsets;


    char[,] chooseRandomPuzzle(int difficulty) {
        // TODO: make it such that we choose a puzzle at random within the folder.
        // Note that these puzzles really aren't super reflective of easy/medium/hard, they're basically just random ones for the sake of testing.
        char[,] puzzle;
        switch (difficulty) {
            case 1:
                return PuzzleDecoder.decode("Assets/Resources/GoodPuzzles/easy/sampleeasypuzzle.ice");
            case 2:
                return PuzzleDecoder.decode("Assets/Resources/GoodPuzzles/medium/samplemedpuzzle.ice");
            case 3:
                return PuzzleDecoder.decode("Assets/Resources/GoodPuzzles/hard/samplehardpuzzle.ice");
            default:
                Debug.Log("Could not find puzzle with specified difficulty. Giving you an easy one by default.");
                return PuzzleDecoder.decode("Assets/Resources/GoodPuzzles/easy/sampleeasypuzzle.ice");

        }
        return puzzle;
    }

    void Start()
    {
        for (int i = 0; i < 3; i++) {
            if (puzzleDifficulties[i] == -1) {
                throw new Exception("Difficulty for the puzzle to be generated not set. Set the \"Difficulty\" in the editor.");
            } 
            else if (puzzleDifficulties[i] < 1 || puzzleDifficulties[i] > 3) {
                throw new Exception("difficulty set to invalid integer. Please set it between 1 and 3 (inclusive).");
            }
        }
        offsets = new List<Vector2Int>();
        offsets.Add(puzzle1Offset);
        offsets.Add(puzzle2Offset);
        offsets.Add(puzzle3Offset);

        List<char[,]> puzzles = new List<char[,]>();
        puzzles.Add(chooseRandomPuzzle(puzzleDifficulties[0]));
        puzzles.Add(chooseRandomPuzzle(puzzleDifficulties[1]));
        puzzles.Add(chooseRandomPuzzle(puzzleDifficulties[2]));
        for (int x = 0; x < 3; x++) {
            char[,] puzzle = puzzles[x];
            Vector3Int offset = new Vector3Int(offsets[x].x, offsets[x].y, 0);
            int n = (int)Math.Sqrt(puzzle.Length); //because C# is stupid, Length = n * n.
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    //TODO: Switch this to a switch statement.
                    if (puzzle[j, i] == 'S') {
                        snowTiles.SetTile(pos + offset, snowTile);
                    }
                    else if (puzzle[j, i] == 'B') {
                        boundaryTiles.SetTile(pos + offset, inPuzzleBoundaryTile);
                    }
                    else if (puzzle[j, i] == '_') {
                        boundaryTiles.SetTile(pos + offset, edgeBoundaryTile);
                    }
                    else if (puzzle[j, i] == 'I') {
                        iceTiles.SetTile(pos + offset, iceTile);
                    }
                }
            }
        }
    }

    void Update()
    {

    }
}

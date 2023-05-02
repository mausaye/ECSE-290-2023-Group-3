using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;
using System.Runtime.InteropServices;

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

    private string chooseRandomPuzzlePathInDirectory(string dir_path) {

        DirectoryInfo d = new DirectoryInfo(dir_path);
        FileInfo[] files = d.GetFiles("*.ice");
        int numFiles = files.Length;

        System.Random r = new System.Random();
        int randInd = r.Next(0, numFiles);
        FileInfo file = files[randInd];

        return dir_path + file.Name;
    }

    private char[,] chooseRandomPuzzleComplexVersion(int difficulty) {
        System.Random r = new System.Random();
        int randInd = r.Next(0, 10);

        TextAsset txt;
        Debug.Log("EasyPuzzle" + randInd);
        switch (difficulty) {
            case 1:
                txt = (TextAsset)Resources.Load("EasyPuzzle", typeof(TextAsset));  
                return PuzzleDecoder.decodeWebGL(new List<string>(txt.text.Split('\n')));
                break;
            case 2:
                txt = (TextAsset)Resources.Load("MediumPuzzle", typeof(TextAsset));  
                return PuzzleDecoder.decodeWebGL(new List<string>(txt.text.Split('\n')));
                break;
            case 3:
                txt = (TextAsset)Resources.Load("HardPuzzle", typeof(TextAsset));  
                return PuzzleDecoder.decodeWebGL(new List<string>(txt.text.Split('\n')));
                break;
            default:
                txt = (TextAsset)Resources.Load("HardPuzzle", typeof(TextAsset));  
                return PuzzleDecoder.decodeWebGL(new List<string>(txt.text.Split('\n')));
        }

    }

    private char[,] chooseRandomPuzzle(int difficulty) {
        string path;

        //My god Unity is garbage. Actually just including everything in the resources folder is too complex for Unity,
        //so random files aren't chosen for WebGL, because it won't include them unless I explicity load the percise file 
        //name for whatever reason. Works fine when built for other platforms of course. 
        #if UNITY_WEBGL
            return chooseRandomPuzzleComplexVersion(difficulty);
        #endif 

        switch (difficulty) {
            case 1:
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                    path = chooseRandomPuzzlePathInDirectory(Application.dataPath + "/Resources/GoodPuzzles/easy/");
                else 
                    path = chooseRandomPuzzlePathInDirectory("Assets/Resources/GoodPuzzles/easy/");
                break;
            case 2:
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                    path = chooseRandomPuzzlePathInDirectory(Application.dataPath + "/Resources/GoodPuzzles/medium/");
                else 
                    path = chooseRandomPuzzlePathInDirectory("Assets/Resources/GoodPuzzles/medium/");
                break;
            case 3:
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
                    path = chooseRandomPuzzlePathInDirectory(Application.dataPath + "/Resources/GoodPuzzles/hard/");
                else 
                    path = chooseRandomPuzzlePathInDirectory("Assets/Resources/GoodPuzzles/hard/");
                break;
            default:
                Debug.Log("Could not find puzzle with specified difficulty. Giving you an easy one by default.");
                path = chooseRandomPuzzlePathInDirectory("Assets/Resources/GoodPuzzles/easy/");
                break;
        }
        return PuzzleDecoder.decode(path);
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
                    Vector3Int pos = new Vector3Int(i, n - 1 - j, 0);
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

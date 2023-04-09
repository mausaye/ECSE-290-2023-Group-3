using UnityEngine;
using System;
using System.IO;

public static class PuzzleDecoder {

    public static char[,] decode(string filepath) {
        string[] rawLines= File.ReadAllLines(filepath);
        int n = Convert.ToInt32(rawLines[0]);
        char[,] puzzle = new char[n + 2, n + 2];
        for (int i = 1; i <= n + 2; i++) {
            string line = rawLines[i];
            int puzzleRow = i - 1;
            int puzzleCol = 0;
            for (int j = 0; j < line.Length; j++) {
                if (line[j] != ' ') {
                    puzzle[puzzleRow, puzzleCol] = line[j];
                    puzzleCol += 1;
                }
            }
        }   
        for (int i = 0; i < n + 2; i++) {
            string test = "";
            for (int j = 0; j < n + 2; j++) {
                test += puzzle[i, j] + " ";
            }
            Debug.Log(test);
        }

        return puzzle;
    }

}
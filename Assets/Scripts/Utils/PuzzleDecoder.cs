using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class PuzzleDecoder {

    public static char[,] decodeWebGL(List<string> rawLines) {
        int n = Convert.ToInt32(rawLines[0]);
        Debug.Log(n);
        char[,] puzzle = new char[n + 2, n + 2];

        for (int i = 1; i <= n + 2; i++) {
            string line = rawLines[i];
            int puzzleRow = i - 1;

            //edge boundaries 
            puzzle[puzzleRow, 0] = '_'; 
            puzzle[puzzleRow, n + 1] = '_';

            //When puzzle col is 0, a boundary is placed. 
            int puzzleCol = 1;
            for (int j = 0; j < line.Length; j++) {
                if (line[j] != ' ') {
                    puzzle[puzzleRow, puzzleCol] = line[j];
                    puzzleCol += 1;
                }
            }
        }   
        return puzzle;
    }

    public static char[,] decode(string filepath) {
        string[] rawLines;
        try {
            rawLines = File.ReadAllLines(filepath);
        }
        catch (FileNotFoundException e) {
            rawLines = new string[1];
            Application.Quit();
        }
        int n = Convert.ToInt32(rawLines[0]);
        Debug.Log(n);
        char[,] puzzle = new char[n + 2, n + 2];

        for (int i = 1; i <= n + 2; i++) {
            string line = rawLines[i];
            int puzzleRow = i - 1;

            //edge boundaries 
            puzzle[puzzleRow, 0] = '_'; 
            puzzle[puzzleRow, n + 1] = '_';

            //When puzzle col is 0, a boundary is placed. 
            int puzzleCol = 1;
            for (int j = 0; j < line.Length; j++) {
                if (line[j] != ' ') {
                    puzzle[puzzleRow, puzzleCol] = line[j];
                    puzzleCol += 1;
                }
            }
        }   
        return puzzle;
    }
}
The index.js file will be permanently running on the server. 
    - What gets sent to the server: A number in the format of "minutes:seconds" for how long it took the player to beat the game (see RPC.cs in Assets/Scripts/Scoreboard)
    - What the server needs to respond with: A list of (at max) 5 scores sorted in increasing order in the format of ["minutes:seconds", "minutes:seconds", ...]
      for each of the top scores. 
      
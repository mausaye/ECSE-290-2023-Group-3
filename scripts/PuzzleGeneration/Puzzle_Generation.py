'''
    This file is used to generate a large set of puzzles that will be used in game. This file does not need to be accessed by the game itself, only the results produced by the file.

    The idea is to run this file, generate a large number of puzzles (maybe 100-10,000 for each difficulty), and then encode them in some simple file format. These will be generated into the
    "EncodedPuzzles" directory. This directory must be copied over to some directory within src/game/. When playing the game, it will randomly choose one of the generated puzzles for each level.
'''
import numpy as np
from enum import Enum
puzzle_id_counter = 0

# Each path can be modeled as a directional inputs.
# UP = move up by 1 tile, DOWN = move down by 1 tile, etc.
class Direction(Enum):
    UP = 1,
    DOWN = 2,
    LEFT = 3, 
    RIGHT = 4


# -------------------------------------------------------------------------------------------------------------------------- #


class Puzzle:
    def __init__(self, n, identifier):
        # size of the puzzle (puzzle is N x N grid)
        self.size = n

        # just so we can easily figure out details about a puzzle without having to print the whole thing 
        # can just get details from the ID 
        self.id = identifier

        # where the player begins and ends in the x-dimension 
        # for now I'm going to assume you start at the bottom of the puzzle, try to make it to the top
        self.begin_x = np.random.choice(np.arange(0, n))
        self.end_x = np.random.choice(np.arange(0, n))

        # total number of solutions 
        self.num_solutions = -1 # unassigned initially

        '''
        - Each path that can get you from the entrance to the exit is a number of direction inputs
        - This variable is the average number of inputs for all "non-trivial" paths
        - A trivial path is one that gets you from entrance to exit in <= 3 direction changes. These puzzles would be too easy, so 
          if a puzzle contains a path like this, it's not even worth considering a valid puzzle.
        '''
        self.avg_nontrivial_path_complexity = -1.0 # unassigned initially

        # Each path in paths is the set of inputs required to get from entrance to exit for that specific path.

        self.grid = np.zeros(shape=(n, n))


        self.generate()


    # Numeric Difficulty = f(num_solutions, avg_nontrivial_path_complexity) = .4 * num_solutions + .6 * avg_nontrivial_path_complexity
    # This equation will make it so difficutly depends slightly more on how complex the average path is. 
    # Likely will need to be adjusted based on our own ideas.
    def get_numeric_difficulty(self):
        pass


    # 78% chance a tile is ice (0), 15% chance a single tile is snow (1), 2% chance it's a 2x2 island of snow, 5% chance it's an obstacle (2).
    # We can test and change these percentages later.
    def generate(self):

        for i in range(self.size):
            for j in range(self.size):
                # if something has already been assigned, don't try to reassign
                if self.grid[i, j] == 0:
                    tile = 0

                    # if we're not on the boundary, also consider generating island 
                    if (i < self.size - 1) and j < (self.size - 1):
                        tile = int(np.random.choice(np.arange(0, 4), p=[0.78, 0.15, .02, 0.05]))
                    else:
                        tile = np.random.choice(np.arange(0, 3), p=[0.8, 0.15, 0.05]) # probabilities a bit adjusted, but it's fine 
                    
                    # if 3 chosen, generate 2x2 snow block
                    if tile == 3:
                        self.grid[i, j] = 1.0
                        self.grid[i + 1, j] = 1.0
                        self.grid[i, j + 1] = 1.0 
                        self.grid[i + 1, j + 1] = 1.0
                    # otherwise, just generate the single chosen tile
                    else:
                        self.grid[i, j] = tile

    
    def get(self, x: int, y: int):
        if x >= self.size or y >= self.size or x < 0 or y < 0:
            raise Exception(f'Indexes for grid are out of range. Max ind: {(self.size - 1, self.size - 1)}, attempted ind: {(x, y)}')
        return self.grid[y, x]
        

            
    # for the sake of printing
    def __str__(self):
        s = f'The following is puzzle with ID {self.id}\n'
        s += "  "
        for i in range(self.size):
            if i == self.begin_x:
                s += "x"
            else:
                s += "_"
            s += "  "
        s += "\n"
        s += str(self.grid)
        s += "\n"
        s += "  "
        for i in range(self.size):
            if i == self.end_x:
                s += "x"
            else:
                s += "_"
            s += "  "
        s += "\n"
        return s

        



# -------------------------------------------------------------------------------------------------------------------------- #


class PuzzleGenerator:
    def __init__(self, num_puzzles_to_generate):
        self.num_puzzles_to_generate = num_puzzles_to_generate
        self.solvable_puzzles = []
        self.num_solvable_generated = 0
        pass


    def generate(self, size):
        while len(self.solvable_puzzles) < self.num_puzzles_to_generate:
            trialPuzzle = Puzzle(size)
            solvable = self.attemptToSolve(trialPuzzle)
            if solvable:
                self.solvable_puzzles.append(trialPuzzle)



    # Find ALL non-circular solutions, return if it can be done. Assign the number of solutions and average path complexity (number of movements needed) variables to get idea of difficulty.
    def attemptToSolve(self, puzzle):

        self.bfs(puzzle)

        if puzzle.num_solutions >= 1:
            print(f'puzzle {puzzle_id_counter} is solvable.')
            return True
        else:
            print(f'puzzle {puzzle_id_counter} is not solvable.')
            return False
    

    def bfs(self, puzzle: Puzzle):
        maxIter = 1000 # only look for solutions within 1000 states from beginning 

        q = [] #bfs queue 
        startingPos = (puzzle.begin_x, -1) # start slightly off the grid
        q.append([startingPos])
        curIter = 0
        
        while (len(q) > 0 and curIter < maxIter):
            path = q.pop(0)

            # always take most recent position to build off of 
            lastPos = path[len(path) - 1]

            #if we win, we're done with this path
            if lastPos == (-1, -1):
                print(f'completed path through these coords: {path}')
                continue

            
            # These if statements simply try to go from the current position and process down, right, up, and down (if possible).
            # "if newMove not in path" -> don't do a cycle. No need to go back to where we already were.
            if (self.canMoveDown(puzzle, lastPos)):
                newMove = self.moveDown(puzzle, lastPos)
                if newMove not in path:
                    newPath = path.copy()
                    newPath.append(newMove)
                    q.append(newPath)

            if (self.canMoveRight(puzzle, lastPos)):
                newMove = self.moveRight(puzzle, lastPos)
                if newMove not in path:
                    newPath = path.copy()
                    newPath.append(newMove)
                    q.append(newPath)

            if (self.canMoveUp(puzzle, lastPos)):
                newMove = self.moveUp(puzzle, lastPos)
                if newMove not in path:
                    newPath = path.copy()
                    newPath.append(newMove)
                    q.append(newPath)

            if (self.canMoveLeft(puzzle, lastPos)):
                newMove = self.moveLeft(puzzle, lastPos)
                if newMove not in path:
                    newPath = path.copy()
                    newPath.append(newMove)
                    q.append(newPath)

            curIter += 1
            

    '''Lots of code duplication here, best to refactor at some point.'''
    def canMoveDown(self, puzzle: Puzzle, pos):
        if (pos[1] + 1 < puzzle.size) and (puzzle.get(pos[0], pos[1] + 1) != 2): #if there isn't an obstacle in the way
            return True
        else:
            return False


    def moveDown(self, puzzle: Puzzle, pos):
        for i in range(pos[1] + 1, puzzle.size):
            if puzzle.get(pos[0], i) == 1:
                return (pos[0], i)
            elif puzzle.get(pos[0], i)== 2:
                return (pos[0], i - 1)

        # if here, we can get to other side
        # if on other side and exit is right below us, then we slide to victory
        if puzzle.end_x == pos[0]:
            return (-1, -1)
        else:
            return (pos[0], puzzle.size - 1)


    def canMoveLeft(self, puzzle: Puzzle, pos):

        # if on beginning border, can't be done
        if pos[1] == -1:
            return False

        if pos[0] - 1 >= 0 and (puzzle.get(pos[0] - 1, pos[1]) != 2): 
            return True
        else:
            return False

    def moveLeft(self, puzzle: Puzzle, pos):
        for i in range(pos[0] - 1, -1, -1):
            if puzzle.get(i, pos[1]) == 1:
                return (i, pos[1])
            elif puzzle.get(i, pos[1]) == 2:
                return (i + 1, pos[1])

        # no victory checks when moving left and right, since exit is only ever on top/bottom.
        return (0, pos[1])

    def canMoveRight(self, puzzle: Puzzle, pos):

        # if on beginning border, can't be done
        if pos[1] == -1:
            return False

        # needed: size check on pos for moving left and right
        if (pos[0] + 1 < puzzle.size) and (puzzle.get(pos[0] + 1, pos[1]) != 2): 
            return True
        else:
            return False

    def moveRight(self, puzzle: Puzzle, pos):
        for i in range(pos[0] + 1, puzzle.size):
            if puzzle.get(i, pos[1]) == 1:
                return (i, pos[1])
            elif puzzle.get(i, pos[1]) == 2:
                return (i - 1, pos[1])
        return (puzzle.size - 1, pos[1])
            
    def canMoveUp(self, puzzle: Puzzle, pos):
        # again, bounds check needed on this next line 
        if pos[1] - 1 >= 0 and (puzzle.get(pos[0], pos[1] - 1) != 2): 
            return True
        else:
            return False


    def moveUp(self, puzzle: Puzzle, pos):
        for i in range(pos[1] - 1, -1, -1):
            if puzzle.get(pos[0], i) == 1:
                return (pos[0], i)
            elif puzzle.get(pos[0], i) == 2:
                return (pos[0], i + 1)


        return (pos[0], 0)

    def writePuzzlesToFiles(self):
        for puzzle in self.solvable_puzzles:
            pass
            # create file 
            # open file 
            # write puzzle to file 
            # close and save file 
            
            


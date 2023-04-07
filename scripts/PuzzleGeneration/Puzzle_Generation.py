'''
    This file is used to generate a large set of puzzles that will be used in game. This file does not need to be accessed by the game itself, only the results produced by the file.

    The idea is to run this file, generate a large number of puzzles (maybe 100-10,000 for each difficulty), and then encode them in some simple file format. These will be generated into the
    "EncodedPuzzles" directory. This directory must be copied over to some directory within src/game/. When playing the game, it will randomly choose one of the generated puzzles for each level.

    As of right now, the puzzle generator generates about 100 valid, solvable 10 x 10 puzzles per second.

    TODO (for the puzzle generation):
        - Make difficulty calculation more accurate
        - Encoding/Decoding algorithm. Encoding (i.e. writing to file) happens in this file, Decoding, happens in a C# script called from Unity.
        - Fix whatever bugs may be in the code
        - Refactor huge chunks of repeated code.
    Bonus Todods:
        - Visualization of puzzles and their solutions beyond text output 
'''

import numpy as np
from enum import Enum
puzzle_id_counter = 0
# -------------------------------------------------------------------------------------------------------------------------- #


# Used by the Solution class below
class Direction(Enum):
    UP = 1,
    DOWN = 2,
    LEFT = 3, 
    RIGHT = 4

    def __str__(self):
        return '%s' % self.value


# -------------------------------------------------------------------------------------------------------------------------- #

'''Just a struct that contains 2 important pieces of info for a solution: the path (exx [(-1, 1), (3, 5), ...]) 
   and the direction changes needed to be on that path (ex: [UP, DOWN, RIGHT, UP, ...])'''
class Solution:
    def __init__(self,path):
        self.path = path

        # Note: This remains in (at least nrearly) non-decreasing order due to the natural way BFS works. 
        # The reason I say "nearly" is because BFS is organizing things based on tile to tile movements, rather than direction changes.
        self.dirs = [] 

        self.calculateDirs()

    def calculateDirs(self):
        # if there's a positive change on the y-axis, we moved down
        # if there's a negative change on the y-axis, we moved up
        # if there's a positive change on the x-axis, we moved right
        # if there's a negative change on the x-axis, we moved left
        # if we were already moving in that direction, it does not count. 
        # at least in theory, only one of these should ever change for adjacent points in the path.
        prevDir = None
        # -2 because the last point in the path will be (-1, -1)
        for i in range(len(self.path) - 2):
            dx = self.path[i + 1][0] - self.path[i][0]
            dy = self.path[i + 1][1] - self.path[i][1]
            if (self.path[i + 1] != (-1, -1) and dx != 0 and dy != 0):
                raise Exception(f'Impossible path somehow found. Somehow went from {self.path[i]} to {self.path[i + 1]}')
            if (dx == 0 and dy == 0):
                raise Exception(f'Bad path found. Didn\'t move between {self.path[i]} and {self.path[i + 1]}')
            
            if dy > 0 and prevDir != Direction.DOWN:
                self.dirs.append(Direction.DOWN)
            elif dy < 0 and prevDir != Direction.UP:
                self.dirs.append(Direction.UP)
            elif dx > 0 and prevDir != Direction.RIGHT:
                self.dirs.append(Direction.RIGHT)
            elif dx < 0 and prevDir != Direction.LEFT:
                self.dirs.append(Direction.LEFT)

            prevDir = self.dirs[-1]

        self.dirs.append(Direction.UP) #path[len - 2] is the last non-victory point, just need to go down again to get victory.

# -------------------------------------------------------------------------------------------------------------------------- #

'''Container for the puzzle grid, with a few extra fields for convience.'''
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
        self.solutions = []

        self.searched = False # bool to represent if we have tried to search this puzzle yet 

        '''
        - Each path that can get you from the entrance to the exit is a number of direction inputs
        - This variable is the average number of inputs for all "non-trivial" paths
        - A trivial path is one that gets you from entrance to exit in <= 3 direction changes. These puzzles would be too easy, so 
          if a puzzle contains a path like this, it's not even worth considering a valid puzzle.
        '''
        self.avg_nontrivial_path_complexity = -1.0 # unassigned initially


        self.grid = np.zeros(shape=(n, n))


        self.generate()

    def num_solutions(self):
        return len(self.solutions)

    # Numeric Difficulty = f(num_solutions, avg_nontrivial_path_complexity) = .4 * num_solutions + .6 * avg_nontrivial_path_complexity
    # avg path complexity = f(path1, path2, ..., pathn) = [(n * path1) + ((n - 1) * path2) + ... 1 * pathn] / n.
    # As the equation suggests, paths aren't weighted equally. Humans naturally will likely choose shorter solutions, so those are weighted much heavier than the really long ones.
    # Likely will need to be adjusted based on our own ideas.
    # If -1.0 returned, it is a trivial puzzle.
    def get_numeric_difficulty(self):
        if not self.searched:
            raise Exception("Attempted to get difficulty on puzzle that hasn't been searched yet. Ya can't do that.")
        sol_weight = 0.4
        path_weight = 0.6
        i = len(self.solutions)
        num_sols = self.num_solutions()

        # path complexity = the number of direction changes needed (which is easily found with the movements list in the Solution class)
        total_path_complexity = 0 #sum of the complexities of each path 

        for solution in self.solutions:
            if len(solution.dirs) <= 3:
                print("This is a trivial puzzle. Probably don't want to use it.")
                return -1.0
            else:
                total_path_complexity = i * len(solution.dirs)
            i -= 1
        #print(self.solutions[0].dirs)
        avg_path_complexity = total_path_complexity / num_sols

        return sol_weight * num_sols + path_weight * avg_path_complexity
    

    def add_solution(self, solution: Solution):
        self.solutions.append(solution)

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
                        tile = int(np.random.choice(np.arange(0, 4), p=[0.78, 0.10, .02, 0.10]))
                    else:
                        tile = np.random.choice(np.arange(0, 3), p=[0.75, 0.15, 0.10]) # probabilities a bit adjusted, but it's fine 
                    
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
        if(x == self.begin_x and y == len(self.grid)):
            return True
        elif x >= self.size or y >= self.size or x < 0 or y < 0:
            raise Exception(f'Indexes for grid are out of range. Max ind: {(self.size - 1, self.size - 1)}, attempted ind: {(x, y)}')
        return self.grid[y, x]
        

            
    # for the sake of printing
    def __str__(self):
        s = f'The following is puzzle with ID {self.id}\n'
        s += "  "
        for i in range(self.size):
            if i == self.end_x:
                s += "x"
            else:
                s += "_"
            s += "  "
        s += "\n"
        s += str(self.grid)
        s += "\n"
        s += "  "
        for i in range(self.size):
            if i == self.begin_x:
                s += "x"
            else:
                s += "_"
            s += "  "
        s += "\n"
        return s

        



# -------------------------------------------------------------------------------------------------------------------------- #

'''Generates and solves puzzles. '''
class PuzzleGenerator:
    def __init__(self, num_puzzles_to_generate):
        self.num_puzzles_to_generate = num_puzzles_to_generate
        self.solvable_puzzles = []
        self.num_solvable_generated = 0
        pass


    def generate(self, puzzle_size, write: bool):
        global puzzle_id_counter
        while len(self.solvable_puzzles) < self.num_puzzles_to_generate:
            trialPuzzle = Puzzle(puzzle_size, puzzle_id_counter)
            puzzle_id_counter += 1
            solvable = self.attemptToSolve(trialPuzzle)
            if solvable:
                self.solvable_puzzles.append(trialPuzzle)
        if (write):
            self.writePuzzlesToFiles()
            



    # Find ALL non-circular solutions, return if it can be done. Assign the number of solutions and average path complexity (number of movements needed) variables to get idea of difficulty.
    def attemptToSolve(self, puzzle: Puzzle):
        self.bfs(puzzle)

        if puzzle.num_solutions() >= 1:
            print(f'puzzle {puzzle_id_counter} is solvable.')
            print(f'Numeric difficulty: {puzzle.get_numeric_difficulty()}')
            print(puzzle)
            print(puzzle.solutions[0].path)
            print(puzzle.solutions[0].dirs)
            return True
        else:
            print(f'puzzle {puzzle_id_counter} is not solvable.')
            return False
    

    def bfs(self, puzzle: Puzzle):
        maxIter = 1000 # only look for solutions within 1000 states from beginning 

        q = [] #bfs queue 
        startingPos = (puzzle.begin_x, len(puzzle.grid)) # start slightly off the grid at bottom
        q.append([startingPos])
        curIter = 0
        
        while (len(q) > 0 and curIter < maxIter):
            path = q.pop(0)

            # always take most recent position to build off of 
            lastPos = path[len(path) - 1]

            #if we win, we're done with this path
            if lastPos == (-1, -1):
                sol = Solution(path)
                puzzle.add_solution(sol)
                #print(f'completed path through these coords: {path}')
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
        puzzle.searched = True
            

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

        if (pos[0] == puzzle.begin_x):
            return (pos[0], len(puzzle.grid))
        else:
            return (pos[0], len(puzzle.grid) - 1)


    def canMoveLeft(self, puzzle: Puzzle, pos):

        # if on beginning border, can't be done
        if pos[1] == len(puzzle.grid):
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
        if pos[1] == len(puzzle.grid):
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


        if puzzle.end_x == pos[0]:
            return (-1, -1)
        else:
            return (pos[0], 0)
        

    def encode(self, puzzle: Puzzle, label: int):
        n = len(puzzle.grid)
        filepath= "EncodedPuzzles/puzzle" + str(label) + ".ice"
        f = open(filepath, 'w')

        f.write(str(n) + '\n')
        for i in range(n):
            if i == puzzle.end_x:
                f.write('X')
            else:
                f.write('_')
            if i != n - 1:
                f.write(' ')
        f.write('\n')
        
        for i in range(n):
            for j in range(n):
                if puzzle.grid[i, j] == 0:
                    f.write('I')
                elif puzzle.grid[i, j] == 1:
                    f.write('S')
                elif puzzle.grid[i, j] == 2:
                    f.write('B')
                else:
                    raise Exception("Something was on the puzzle that was not 0, 1, or 2.")
                if j != n - 1:
                    f.write(' ')
            f.write('\n')
                

        for i in range(n):
            if i == puzzle.begin_x:
                f.write('X')
            else:
                f.write('_')
            if i != n - 1:
                f.write(' ')
        f.close()

    def writePuzzlesToFiles(self):
        for i in range(len(self.solvable_puzzles)):
            self.encode(self.solvable_puzzles[i], i)


            
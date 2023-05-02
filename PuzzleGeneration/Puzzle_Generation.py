'''
    This file is used to generate a large set of puzzles that will be used in game. This file does not need to be accessed by the game itself, only the results produced by the file.

    The idea is to run this file, generate a large number of puzzles (maybe 100-10,000 for each difficulty), and then encode them in some simple file format. These will be generated into the
    "EncodedPuzzles" directory. This directory must be copied over to some directory within src/game/. When playing the game, it will randomly choose one of the generated puzzles for each level.

    As of right now, the puzzle generator generates about 100 valid, solvable 10 x 10 puzzles per second.

    Pretty much entirely done. Could probably be cleaned up a bit more and optimized, but for the most 

'''

import numpy as np
from enum import Enum
puzzle_id_counter = 0
min_solution_length = 10
max_solution_length = 13


# -------------------------------------------------------------------------------------------------------------------------- #


'''Used by the Solution class below'''
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

        # Note: This remains in (at least nearly) non-decreasing order due to the natural way BFS works. 
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
        # -2 because the last point in the path will be (-1, -1), i.e. the victory point
        for i in range(len(self.path) - 2):
            dx = self.path[i + 1][0] - self.path[i][0]
            dy = self.path[i + 1][1] - self.path[i][1]
            if (self.path[i + 1] != (-1, -1) and dx != 0 and dy != 0):
                raise Exception(f'Impossible path somehow found. Somehow went from {self.path[i]} to {self.path[i + 1]}')
            if (dx == 0 and dy == 0):
                raise Exception(f'Bad path found. Didn\'t move between {self.path[i]} and {self.path[i + 1]}')
            
            # only append if the new direction is different than the previous.
            if dy > 0 and prevDir != Direction.DOWN:
                self.dirs.append(Direction.DOWN)
            elif dy < 0 and prevDir != Direction.UP:
                self.dirs.append(Direction.UP)
            elif dx > 0 and prevDir != Direction.RIGHT:
                self.dirs.append(Direction.RIGHT)
            elif dx < 0 and prevDir != Direction.LEFT:
                self.dirs.append(Direction.LEFT)

            prevDir = self.dirs[-1]

        self.dirs.append(Direction.UP) #path[len - 2] is the last non-victory point, just need to go up again to get victory.


# -------------------------------------------------------------------------------------------------------------------------- #


'''Container for the puzzle grid, with a few extra fields for convience.'''
class Puzzle:
    def __init__(self, n, identifier):
        # size of the puzzle (puzzle is N x N grid)
        self.size = n

        # Unique identifier 
        self.id = identifier

        # where the player begins and ends in the x-dimension 
        # for now I'm going to assume you start at the bottom of the puzzle, try to make it to the top
        self.begin_x = np.random.choice(np.arange(0, n))
        self.end_x = np.random.choice(np.arange(0, n))

        # set of all solutions (that aren't insanely long)
        self.solutions = []

        self.searched = False # bool to represent if we have tried to search this puzzle yet 

        self.difficulty = -1.0 #unassigned

        '''
            - A trivial path is one that can be solved in 3 or fewer direction changes 
            - avg path complexity is a weighted average, where shorter (but still non-trivial) paths matter more than longer paths.
            - This is becauese (in theory) humans will try to take shorter paths before longer paths.
        '''
        self.avg_nontrivial_path_complexity = -1.0 # unassigned initially

        self.grid = np.zeros(shape=(n, n))

        self.generate()

    def num_solutions(self):
        return len(self.solutions)
    
    def set_searched(self):
        self.searched = True;
        if self.num_solutions() == 0:
            self.difficulty = float('inf')
        else:
            self.difficulty = self.get_numeric_difficulty()
    

    # Numeric Difficulty = f(num_solutions, avg_nontrivial_path_complexity) = .4 * num_solutions + .6 * avg_nontrivial_path_complexity
    def get_numeric_difficulty(self):
        # if already calculated, return that 
        if self.difficulty != -1.0:
            return self.difficulty

        if not self.searched:
            raise Exception("Attempted to get difficulty on puzzle that hasn't been searched yet. Ya can't do that.")

        sol_weight = 0.3
        path_weight = 0.7
        i = len(self.solutions)
        num_sols = self.num_solutions()

        # weighted sum of all complexities 
        total_path_complexity = 0 

        for solution in self.solutions:
            if len(solution.dirs) < min_solution_length or len(solution.dirs) > max_solution_length:
                #print("This is a trivial puzzle. Probably don't want to use it.")
                return -1.0
            else:
                total_path_complexity = i * len(solution.dirs)
            i -= 1

        avg_path_complexity = total_path_complexity / num_sols

        return sol_weight * num_sols + path_weight * avg_path_complexity
    
    def add_solution(self, solution: Solution):
        self.solutions.append(solution)

    # 78% chance a tile is ice (0), 15% chance a single tile is snow (1), 2% chance it's a 2x2 island of snow, 5% chance it's an obstacle (2).
    # We can test and change these percentages later.
    def generate(self):
        count = 0
        for i in range(self.size):
            for j in range(self.size):
                # if something has already been assigned, don't try to reassign
                if self.grid[i, j] == 0:
                    tile = 0
                    if (count > 20):
                        tile = int(np.random.choice([0, 2], p=[0.91, 0.09]))
                        self.grid[i, j] = tile
                        continue

                    tile = np.random.choice(np.arange(0, 3), p=[0.90, 0.08, 0.02]) # probabilities a bit adjusted, but it's fine 
                    if tile == 1:
                        count += 1
                    
                    self.grid[i, j] = tile

    
    def get(self, x: int, y: int):
        if(x == self.begin_x and y == len(self.grid)):
            return True
        elif x >= self.size or y >= self.size or x < 0 or y < 0:
            raise Exception(f'Indexes for grid are out of range. Max ind: {(self.size - 1, self.size - 1)}, attempted ind: {(x, y)}')
        return self.grid[y, x]
    
    def is_trivial(self):
        return self.difficulty == -1.0
    
    def is_possible(self):
        return self.difficulty != float('inf')

            
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
            



    # Find ALL non-circular solutions, return if it can be done. Assign the number of 
    # solutions and average path complexity (number of movements needed) variables to get idea of difficulty.
    def attemptToSolve(self, puzzle: Puzzle):
        self.bfs(puzzle)
        if puzzle.is_possible() and not puzzle.is_trivial():
            print(f'puzzle {puzzle_id_counter} is solvable.')
            print(f'Numeric difficulty: {puzzle.get_numeric_difficulty()}')
            print(puzzle)
            # print shortest path and corresponding inputs
            print(puzzle.solutions[0].path)
            print(puzzle.solutions[0].dirs)
            return True
        else:
            print(f'puzzle {puzzle_id_counter} is not solvable.')
            return False
    

    def bfs(self, puzzle: Puzzle):
        maxIter = 1500 # only look for solutions within 1000 states from beginning. Anything longer isn't reasonable for a human to do.

        q = [] #bfs queue 
        startingPos = (puzzle.begin_x, len(puzzle.grid)) # start slightly off the grid at bottom
        q.append([startingPos])
        curIter = 0
        
        while (len(q) > 0 and curIter < maxIter):
            path = q.pop(0)

            # always take most recent position to build off of 
            lastPos = path[len(path) - 1]

            #if we reach the end, we're done with this path
            if lastPos == (-1, -1):
                sol = Solution(path)
                puzzle.add_solution(sol)
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
        puzzle.set_searched()
            

    '''Lots of code duplication here, could be simplified if we have nothing better to do.'''
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
        

    def encodePuzzle(self, puzzle: Puzzle, label: int):
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
            self.encodePuzzle(self.solvable_puzzles[i], i)

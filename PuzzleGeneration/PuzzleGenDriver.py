import sys
from Puzzle_Generation import * 

def main():
    num_puzzles = int(sys.argv[1])
    sz = int(sys.argv[2])
    generator = PuzzleGenerator(num_puzzles)
    generator.generate(sz, True)



if __name__ == "__main__":
    main()
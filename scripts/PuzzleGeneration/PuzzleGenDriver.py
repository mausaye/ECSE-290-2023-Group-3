from Puzzle_Generation import * 

def main():
    num_puzzles = input("How many puzzles to generate")
    generator = PuzzleGenerator(num_puzzles)
    p = Puzzle(10, 0)
    print(p)
    generator.attemptToSolve(p)



if __name__ == "__main__":
    main()
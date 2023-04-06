from Puzzle_Generation import * 

def main():
    num_puzzles = int(input("How many puzzles to generate"))
    generator = PuzzleGenerator(num_puzzles)
    generator.generate(10)
    #p = Puzzle(5, 0)
    #print(p)
    #generator.attemptToSolve(p)



if __name__ == "__main__":
    main()
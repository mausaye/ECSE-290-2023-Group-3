from Puzzle_Generation import * 

def main():
    num_puzzles = int(input("How many puzzles to generate"))
    generator = PuzzleGenerator(num_puzzles)
    generator.generate(10, True)



if __name__ == "__main__":
    main()
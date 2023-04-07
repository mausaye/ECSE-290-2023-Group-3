This is where the solvable puzzles will be placed.

Encoded puzzles have the following format:
5
_ _ _ X _ 
I I S I I
I S S B S
I S S I I
I I S I I
_ _ X _ _

- Key
    - The first number represents the dimension of the puzzle. The puzzle is always N x N. So above, the 5 shows that the puzzle is going to be a 5 x 5 puzzle.
    - _ = Edge Barrier (note that there are always edge barriers on the right and left side of the puzzles, not depicted in encoding).
    - X = beginning/end position. The top one is the beginning position, 
    - I = Ice (i.e. place where you slide in the same direction that you were moving)
    - S = Snow (i.e. place where you can move normally)
    - B = Barrier (i.e. tile you can't move onto, but will stop you from sliding)



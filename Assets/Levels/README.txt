Level designer file format:
    Csv grid style files where each comma separated 'cell'' has the following entries.
    First row contains level size and width
    Remaining rows are cells in the following format.

,A:B:C:D:E,

A - Cell Colour:
Single number representing the color of the square taking up this cell.

    0 - nothing (no square)
    1 - White 
    2 - Black 
    3 - Grey
    4 - Yellow
    5 - Red 
    6 - Blue
    7 - Green 
    8 - Brown 
    9 - Pink
    10 - Orange
    11 - Purple

B - Rotation:
Single number representing the rotation of the square in degrees.

C - Fixed indicator:
This indicates whether the square is fixed in the scene or not.

    1 - Fixed
    0 - Not Fixed

D - JoinIndicators
Combination of the letters U,D,L,R indicating if the square is joined in these directions.

E - Identifier
A string identifying the square so that they may be picked up by the level script.

There may be more fields to be introduced.

The level is then built from position (0,0) in the scene.
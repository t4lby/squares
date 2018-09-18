using System;
using System.Collections.Generic;
namespace LevelBuilding
{
    public class CellInfo
    {
        public SquareType Color { get; set; }

        public int Rotation { get; set; }

        public bool Fixed { get; set; }

        public char[] JoinDirections { get; set; }

        public string Identifier { get; set; }
    }
}

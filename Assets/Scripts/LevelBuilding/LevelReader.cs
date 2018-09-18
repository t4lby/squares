using System;
using System.IO;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Playables;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuilding
{
    public static class LevelReader
    {
        private static string[] ReadLines(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public static CellInfo[,] ReadLevel(string filePath)
        {
            var lines = ReadLines(filePath);
            var sizeInfo = lines[0].Split(',');
            int width;
            int height;
            if (!int.TryParse(sizeInfo[0],out width) | 
                !int.TryParse(sizeInfo[1], out height))
            {
                throw new UnityException(message: "invalid size information, please review level doc");
            }
            var output = new CellInfo[height,width];
            for (int i = 1; i < height+1; i++)
            {
                var rawCells = lines[i].Split(',');
                for (int j = 0; j < width; j++ )
                {
                    var fields = rawCells[j].Split(':');
                    output[i-1,j] = new CellInfo
                    {
                        Color = PickColor(int.Parse(fields[0])),
                        Rotation = int.Parse(fields[1]),
                        Fixed = fields[2] == "1",
                        JoinDirections = fields[3].ToCharArray(),
                        Identifier = fields[4]
                    };
                }
            }
            return output;
        }

        private static SquareType PickColor(int i)
        {
            switch (i)
            {
                case 1:
                    return SquareType.White;
                case 2:
                    return SquareType.Black;
                case 3:
                    return SquareType.Grey;
                case 4:
                    return SquareType.Yellow;
                case 5:
                    return SquareType.Red;
                case 6:
                    return SquareType.Blue;
                case 7:
                    return SquareType.Green;
                case 8:
                    return SquareType.Brown;
                case 9:
                    return SquareType.Pink;
                case 10:
                    return SquareType.Orange;
                case 11:
                    return SquareType.Purple;
                default:
                    return SquareType.Blank;
            }
        }
    }
}

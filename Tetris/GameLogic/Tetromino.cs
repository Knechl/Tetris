using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris.GameLogic
{
    public class Tetromino
    {
        /* Block Types */
        public static int[,] IBlock = new int[1, 4] // Hellblau
        {
            { 1, 1, 1, 1 }
        };

        public static int[,] OBlock = new int[2, 2] // Gelb
        {
            { 1, 1 },
            { 1, 1 }
        };

        public static int[,] TBlock = new int[2, 3] // Lila
        {
            { 0, 1, 0 },
            { 1, 1, 1 }
        };

        public static int[,] SBlock = new int[2, 3] // Grün
        {
            { 0, 1, 1 },
            { 1, 1, 0 }
        };

        public static int[,] ZBlock = new int[2, 3] // Dunkel Rot
        {
            { 1, 1, 0 },
            { 0, 1, 1 }
        };

        public static int[,] LBlock = new int[3, 2] // Dunkel Blau
        {
            { 1, 0 },
            { 1, 0 },
            { 1, 1 }
        };

        public static int[,] JBlock = new int[3, 2] // Orange
        {
            { 0, 1 },
            { 0, 1 },
            { 1, 1 }
        };

        // Farben
        public static Color IBlockColor = Colors.Cyan;
        public static Color OBlockColor = Colors.Yellow;
        public static Color TBlockColor = Colors.Purple;
        public static Color SBlockColor = Colors.Green;
        public static Color ZBlockColor = Colors.DarkRed;
        public static Color LBlockColor = Colors.DarkBlue;
        public static Color JBlockColor = Colors.Orange;

        // Holt Block und Farbe basierend auf Typ
        public static (int[,] shape, Color color) GetBlockData(int blockType)
        {
            return blockType switch
            {
                0 => (IBlock, IBlockColor),
                1 => (OBlock, OBlockColor),
                2 => (TBlock, TBlockColor),
                3 => (SBlock, SBlockColor),
                4 => (ZBlock, ZBlockColor),
                5 => (LBlock, LBlockColor),
                6 => (JBlock, JBlockColor),
                _ => (IBlock, IBlockColor)
            };
        }
    }
}
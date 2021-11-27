using System;
using System.Collections.Generic;

namespace Scrabble
{
    #nullable enable
    public class Tile
    {
        public char Letter { get; }

        // The ? signifies that coord should be nullable
        public Coord? Coord { get; set; }

        public Bonus LetterBonus { get; }

        public Tile(char letter, Coord coord)
        {
            Letter = letter;
            Coord = coord;
        }

        public Tile(char letter)
        {
            Letter = letter;
            Coord = null;
        }

        public override string ToString()
        {
            return base.ToString() + ": " + Letter + ", " + Coord + ", " + LetterBonus;
        }

    }
}

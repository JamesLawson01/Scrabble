using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Scrabble
{
    #nullable enable
    public class Tile
    {
        public char Letter { get; }

        // The ? signifies that coord should be nullable
        public Coord? Coord { get; set; }

        private Bonus letterBonus;
        public Bonus LetterBonus
        {
            get => letterBonus;
            set
            {
                if (value != Bonus.None)
                {
                    letterBonus = value;
                }
            }
        }

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

        public Tile Clone()
        {
            Tile newTile;
            if (Coord == null)
            {
                newTile = new Tile(Letter);
            }
            else
            {
                newTile = new Tile(Letter, Coord with { });
            }
            newTile.letterBonus = letterBonus;
            return newTile;
        }
    }
}

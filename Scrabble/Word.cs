using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Scrabble
{
    public class Word
    {
        private const string sowpodsLocation = @"Sowpods.txt";

        private static bool previouslyValidated = false;

        private static HashSet<string> sowpods;

        public List<Tile> word { get; }

        public Bonus WordBonus { get; }

        public Word(List<Tile> word)
        {
            this.word = word;
        }

        public bool Validate()
        {
            if (!previouslyValidated)
            {
                InitSowpods();
                previouslyValidated = true;
            }
            return sowpods.Contains(ToString());
        }

        private static void InitSowpods()
        {
            sowpods = new HashSet<string>();
            StreamReader sr = new StreamReader(sowpodsLocation);
            while (sr.Peek() != -1)
            {
                sowpods.Add(sr.ReadLine());
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach (Tile tile in word)
            {
                str += tile.Letter;
            }
            return str.ToLower();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace Scrabble
{
    public class User
    {
        //words the user has played
        public List<Word> Words { get; }

        //user's name
        public string Name { get; }
        
        //tiles the user can play
        public List<Tile> Tiles { get; }

        public bool IsOutOfTiles { get { return Tiles.Count == 0; } }

        private int numTurns;
        public int NumTurns { get { return numTurns; } }

        private int score;
        public int Score { get { return score; } }

        public User(string username, List<Tile> tiles)
        {
            Name = username;

            if (tiles.Count != 7)
            {
                throw new ArgumentException(paramName: nameof(tiles), message: "Must be a list of length 7");
            }

            Words = new();
            Tiles = tiles;
        }

        /// <summary>
        /// Add a word to the user's list of played words
        /// </summary>
        /// <param name="word"></param>
        public void AddWord(Word word)
        {
            Words.Add(word);
            score += word.Value;
        }

        public void IncrementTurns()
        {
            numTurns++;
        }

        //removes tiles that have been placed and adds new ones
        public void ChangeTiles(List<Tile> oldTiles, List<Tile> newTiles)
        {
            foreach (Tile oldTile in oldTiles)
            {
                Tiles.Remove(Tiles.Find(tile => tile.Letter == oldTile.Letter));
            }

            Tiles.AddRange(newTiles);
        }

    }
}

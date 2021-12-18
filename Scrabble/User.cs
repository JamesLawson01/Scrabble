using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scrabble
{
    public class User
    {
        //words the user has played
        public List<Word> words { get; }

        //user's name
        public string Name { get; }

        //tiles the user can play
        public Tile[] Tiles { get; }

        private int numTurns;
        public int NumTurns { get { return numTurns; } }


        public User(string username, Tile[] tiles)
        {
            Name = username;

            if (tiles.Length != 7)
            {
                throw new ArgumentException(paramName: nameof(tiles), message: "Must be an array of length 7");
            }

            words = new();
            Tiles = tiles;
        }

        /// <summary>
        /// Add a word to the user's list of played words
        /// </summary>
        /// <param name="word"></param>
        public void AddWord(Word word)
        {
            words.Add(word);
        }

        public void IncrementTurns()
        {
            numTurns++;
        }

        //To work on
        public void ChangeTiles(List<Tile> oldTiles, List<Tile> newTiles)
        {
            List<int> indices = new();
            foreach (Tile tile in Tiles)
            {
                foreach (Tile oldTile in oldTiles)
                {
                    if (tile.Letter == oldTile.Letter)
                    {
                        indices.Add(Array.IndexOf(Tiles, tile));
                        break;
                    }
                }
            }

            foreach (int index in indices)
            {
                Tiles[index] = newTiles[0];
                newTiles.RemoveAt(0);
            }
        }
    }
}

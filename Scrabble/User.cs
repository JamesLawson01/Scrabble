using System;
using System.Collections.Generic;

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

        public int numTurns { get; }


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
    }
}

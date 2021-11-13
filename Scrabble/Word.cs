using System;
using System.Collections.Generic;

namespace Scrabble
{
    public class Word
    {
        public string word { get; }

        public Bonus WordBonus { get; }

        public Word(string word)
        {
            this.word = word;
        }

        public bool Validate(Word word)
        {
            //
            return true;
        }
    }
}

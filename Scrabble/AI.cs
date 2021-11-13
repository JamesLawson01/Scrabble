using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrabble
{
    class AI : User
    {
        public enum Difficulty
        {
            Medium,
            Low,
            High
        }

        private readonly Difficulty difficulty;

        public AI(string username, Tile[] tiles, Difficulty difficulty) : base(username, tiles)
        {
            this.difficulty = difficulty;
        }
    }
}

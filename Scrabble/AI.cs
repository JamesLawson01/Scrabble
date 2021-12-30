using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        public List<Word> GenerateAllWords(List<Tile> previousTiles)
        {
            List<Word> words = new();

            for (int i=0; i<15; i++)
            {
                for (int j=0; j<15; j++)
                {
                    Coord coord = new (i, j);
                    List<Word> horizontalWords = IterateWord(coord with { }, Orientation.Horizontal, previousTiles);
                    words.AddRange(horizontalWords);
                    List<Word> verticalWords = IterateWord(coord with { }, Orientation.Vertical, previousTiles);
                    words.AddRange(verticalWords);
                }
            }
            return words;
        }

        private List<Word> IterateWord(Coord coord, Orientation direction, List<Tile> previousTiles)
        {
            //get starting tile
            Tile startTile = previousTiles.Find(tile => tile.Coord == coord);
            if (startTile is null)
            {
                startTile = new(' ', coord with { });
            }

            Word word = new(startTile);
            List<Word> returnWords = new();
            bool loop = true;

            while (loop)
            {
                //move to next coord
                coord = IterateCoord(coord, direction);
                if (coord.IsOutsideBounds())
                {
                    loop = false;
                }
                else
                {
                    //convert coord into tile
                    Tile tile = previousTiles.Find(tile => tile.Coord == coord);
                    if (tile is null)
                    {
                        tile = new(' ', coord with { });
                    }

                    //add tile to word and add a copy of word to list of words
                    word.AppendWord(tile);
                    if (word.word.Count > 1)
                    {
                        returnWords.Add(word.Clone());
                    }
                }
            }
            return returnWords;
        }



        /*private List<Word> IterateWord(Coord coord, Orientation direction, List<Tile> previousTiles)
        {
            Coord startCoord = coord with { };  // store starting point for use when going backwards
            List<Word> returnWordsForwards = new List<Word>();
            List<Word> returnWordsBackwards = new List<Word>();
            Word word;
            bool skip = false;

            for (int change = 1; change >= -1; change -= 2)
            {
                // start tile
                word = new(new List<Tile>() { previousTiles.Find(tile => tile.Coord == startCoord) });

                bool loop = true;
                while (loop)
                {
                    coord = IterateCoord(coord, change, direction);

                    //try and get the next tile along
                    Tile nextTile = previousTiles.Find(nextTile => nextTile.Coord == coord);

                    //add tile to word
                    if (nextTile is null && skip != true)
                    {
                        skip = false;
                        if (change == 1)
                        {
                            returnWordsForwards.Add(word.Clone());
                        }
                        else if (change == -1)
                        {
                            returnWordsBackwards.Insert(0, word.Clone());
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(paramName: nameof(change), message: "Only 1 or -1 is allowed");
                        }
                    }
                    else if (nextTile is not null) //end of word reached
                    {
                        skip = true;
                    }

                    if (coord.IsOutsideBounds())
                    {
                        loop = false;
                    }
                    else
                    {
                        if (change == 1)
                        {
                            if (nextTile is null)
                            {
                                word.AppendWord(new Tile(' ', coord with { }));
                            }
                            else
                            {
                                word.AppendWord(nextTile);
                            }
                        }
                        else if (change == -1)
                        {
                            if (nextTile is null)
                            {
                                word.PrependWord(new Tile(' ', coord with { }));
                            }
                            else
                            {
                                word.PrependWord(nextTile);
                            }
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(paramName: nameof(change), message: "Only 1 or -1 is allowed");
                        }
                    }
                }
            }

            //merge forwards and backwards words
            List<Word> returnWords = new List<Word>();

            foreach (Word word1 in returnWordsForwards)
            {
                foreach (Word word2 in returnWordsBackwards)
                {
                    if (word1.word[1] is null)
                    {
                        returnWords.Add(word2);
                    }
                    if (word2.word.Last() is null)
                    {
                        returnWords.Add(word1);
                    }

                    returnWords.Add(new Word(word2.word.Concat(word1.word).Distinct().ToList()));
                }
            }

            return returnWords;
        }*/

        /*private Coord IterateCoord(Coord coord, int change, Orientation direction)
        {
            if (direction == Orientation.Horizontal)
            {
                coord.X += change;
            }
            else
            {
                coord.Y += change;
            }
            return coord;
        }*/

        private Coord IterateCoord(Coord coord, Orientation direction)
        {
            if (direction == Orientation.Horizontal)
            {
                coord.X += 1;
            }
            else
            {
                coord.Y += 1;
            }
            return coord;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Scrabble
{
    public class AI : User
    {
        public enum Difficulty
        {
            Medium,
            Low,
            High
        }

        private readonly Difficulty difficulty;

        //private List<Word> temp;

        public AI(string username, Tile[] tiles, Difficulty difficulty) : base(username, tiles)
        {
            this.difficulty = difficulty;
        }

        public List<Word> GenerateAllWords(List<Tile> previousTiles)
        {
            List<Word> WordLocations = new();

            //gets all words of all lengths with all possible starting coords
            for (int i=0; i<15; i++)
            {
                for (int j=0; j<15; j++)
                {
                    Coord coord = new (i, j);
                    List<Word> iterationWords = new();

                    iterationWords.AddRange(IterateWord(coord with { }, Orientation.Horizontal, previousTiles));
                    iterationWords.AddRange(IterateWord(coord with { }, Orientation.Vertical, previousTiles));

                    //add Word objects that aren't blank to words
                    foreach (Word word in iterationWords)
                    {
                        bool add = false;
                        int numBlanks = 0;
                        foreach (Tile tile in word.word)
                        {
                            if (tile.Letter != ' ')
                            {
                                add = true;
                            }
                            else
                            {
                                numBlanks++;
                            }
                        }
                        if (add && numBlanks > 0 && numBlanks <= 7)
                        {
                            WordLocations.Add(word);
                        }
                    }
                }
            }

            //removes words that have a placed tile before or after them, as that tile would form part of the word.
            //the word with that tile included is already elsewhere in the words list
            int t = 0;
            bool removed;
            bool loop = true;
            while (loop)
            {
                removed = false;
                Word word = WordLocations[t];
                List<Tile> boundingTiles = new();
                Orientation orientation = word.GetOrientation();

                Coord firstCoord = word.word[0].Coord;
                Coord lastCoord = word.word.Last().Coord;
                List<Coord> boundingCoords;

                //gets the coord immediately before and immediately after the word
                if (orientation == Orientation.Horizontal)
                {
                    boundingCoords = new()
                    {
                        new Coord(firstCoord.X - 1, firstCoord.Y),
                        new Coord(lastCoord.X + 1, lastCoord.Y)
                    };
                }
                else
                {
                    boundingCoords = new()
                    {
                        new Coord(firstCoord.X, firstCoord.Y - 1),
                        new Coord(lastCoord.X, lastCoord.Y + 1)
                    };
                }

                //removes the word if a tile has been placed at a boundingCoord
                foreach (Coord boundingCoord in boundingCoords)
                {
                    if (!boundingCoord.IsOutsideBounds())
                    {
                        Tile boundingTile = previousTiles.Find(tile => tile.Coord == boundingCoord);
                        if (boundingTile is not null)
                        {
                            WordLocations.Remove(word);
                            removed = true;
                            break;
                        }
                    }
                }

                //moves the counter along, if the word hasn't been removed
                if (!removed)
                {
                    t++;
                }
                //reached the end of the words list
                if (t >= WordLocations.Count)
                {
                    loop = false;
                }
            }

            //temp = new();

            //generate all ways of arranging the 7 tiles available to the AI
            List<Word> permutations = new();
            List<Word> combinations = Combinations(new Word(Tiles.ToList()));
            foreach (Word combination in combinations)
            {
                permutations.AddRange(Permutate(combination));
            }


            /*List<Word> words = new();
            foreach (Word location in WordLocations)
            {
                List<Tile> blanks = location.word.FindAll(tile => tile.Letter == ' ');
                foreach (Word permutation in permutations)
                {
                    if (permutation.word.Count == blanks.Count)
                    {
                        List<Tile> newTiles = new();
                    }
                }
            }*/

            return WordLocations;
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

        /*private List<Word> Permutate(Word input, ref List<Word> permutations)
        {
            if (input.word.Count == 1)
            {
                //permutations = new List<Word>();
                permutations.Add(input.Clone());
                return new List<Word>() { input.Clone() };
            }
            else
            {
                foreach (Tile tile in input.word)
                {
                    Word shorterWord = new(input.word);
                    shorterWord.word.Remove(tile);
                    List<Word> depthPermutations = Permutate(shorterWord, ref permutations);
                    foreach (Word permutation in depthPermutations)
                    {
                        permutations.Add(permutation.Clone());
                        permutation.word.Insert(0, tile);
                        permutations.Add(permutation.Clone());
                    }
                    return depthPermutations;
                }
                return new();
            }
        }*/

        private List<Word> Combinations(Word input)
        {
            int n = input.word.Count;
            // Power set contains 2^N subsets.
            int powerSetCount = 1 << n;
            List<Word> output = new();

            for (int setMask = 0; setMask < powerSetCount; setMask++)
            {
                List<Tile> tiles = new();
                for (int i = 0; i < n; i++)
                {
                    // Checking whether i'th element of input collection should go to the current subset.
                    if ((setMask & (1 << i)) > 0)
                    {
                        tiles.Add(input.word[i]);
                    }
                }
                if (tiles.Count > 0)
                {
                    output.Add(new Word(tiles));
                }
            }
            return output;
        }

        public IEnumerable<Word> Permutate(Word input)
        {
            if (input.word.Count == 1)
            {
                yield return input;
            }
            /*if (input.word.Count == 2) //base
            {
                yield return input;
                yield return new Word(new List<Tile>() { input.word[1], input.word[0] } );
            }*/
            else
            {
                foreach (Tile tile in input.word)
                {
                    //create sub-word
                    var subWord = new Word (new List<Tile>(input.word));
                    subWord.word.Remove(tile);

                    //find permutations of sub-word
                    foreach (Word word in Permutate(subWord))
                    {
                        //temp.Add(word);
                        //yield return new Word(new List<Tile>(word.word));
                        
                        //get permutations of original word
                        word.word.Insert(0, tile);
                        yield return word;
                    }
                }
            }
        }
    }
}

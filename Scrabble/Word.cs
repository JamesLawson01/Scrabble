using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;
using System.Threading.Tasks;

namespace Scrabble
{
    public class Word
    {
        private const string sowpodsLocation = @"Sowpods.txt";

        private static bool previouslyValidated = false;

        private static HashSet<string> sowpods;

        private static readonly HttpClient client = new();

        public List<Tile> word { get; }

        public Bonus wordBonus;
        public Bonus WordBonus
        {
            get => wordBonus;
            set
            {
                if (value != Bonus.None)
                {
                    wordBonus = value;
                }
            }
        }

        public Word(List<Tile> word)
        {
            this.word = word;
        }

        public Word(Tile tile)
        {
            this.word = new() { tile };
        }

        public Word Clone()
        {
            List<Tile> tiles = new List<Tile>();
            foreach (Tile tile in this.word)
            {
                tiles.Add(tile.Clone());
            }
            Word word =  new(tiles);
            word.WordBonus = WordBonus;
            return word;
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

        public void AppendWord(Tile tile)
        {
            word.Add(tile);
        }

        public void PrependWord(Tile tile)
        {
            word.Insert(0, tile);
        }

        public async Task<float> GetPopularity()
        {
            string url = $"https://books.google.com/ngrams/json?content={ToString()}&year_start=2018&year_end=2019&corpus=26&smoothing=0";

            client.DefaultRequestHeaders.Accept.Clear();

            Task<Stream> streamTask = client.GetStreamAsync(url);
            List<Ngram> ngrams;
            try
            {
                ngrams = await JsonSerializer.DeserializeAsync<List<Ngram>>(await streamTask);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            if (ngrams.Count == 0)
            {
                return 0.0F;
            }

            Debug.WriteLine(ngrams[0].Timeseries[1].ToString());
            return ngrams[0].Timeseries[1];
        }

        /*private async Task<Stream> Request(string url)
        {
            try
            {
                Task<Stream> streamTask = client.GetStreamAsync(url);
            }
        }*/

        /*private static async Task<Ngram> RequestApiData(string url)
        {
            Debug.WriteLine(url);
            client.DefaultRequestHeaders.Accept.Clear();

            var streamTask = client.GetStreamAsync(url);
            List<Ngram> ngrams = new();
            try
            {
                ngrams = await JsonSerializer.DeserializeAsync<List<Ngram>>(await streamTask);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            foreach (var ngram in ngrams)
            {
                Debug.WriteLine(ngram.Timeseries[1].ToString());
            }
            return ngrams[0];

        }*/
    }
}

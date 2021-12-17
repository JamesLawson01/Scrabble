using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Scrabble
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly Dictionary<Coord, Bonus> bonusLocations = new()
        {
            { new Coord(0, 0), Bonus.TripleWord },
            { new Coord(7, 0), Bonus.TripleWord },
            { new Coord(14, 0), Bonus.TripleWord },
            { new Coord(0, 7), Bonus.TripleWord },
            { new Coord(14, 7), Bonus.TripleWord },
            { new Coord(0, 14), Bonus.TripleWord },
            { new Coord(7, 14), Bonus.TripleWord },
            { new Coord(14, 14), Bonus.TripleWord },

            { new Coord(1, 1), Bonus.DoubleWord },
            { new Coord(2, 2), Bonus.DoubleWord },
            { new Coord(3, 3), Bonus.DoubleWord },
            { new Coord(4, 4), Bonus.DoubleWord },
            { new Coord(1, 13), Bonus.DoubleWord },
            { new Coord(2, 12), Bonus.DoubleWord },
            { new Coord(3, 11), Bonus.DoubleWord },
            { new Coord(4, 10), Bonus.DoubleWord },
            { new Coord(13, 1), Bonus.DoubleWord },
            { new Coord(12, 2), Bonus.DoubleWord },
            { new Coord(11, 3), Bonus.DoubleWord },
            { new Coord(10, 4), Bonus.DoubleWord },
            { new Coord(13, 13), Bonus.DoubleWord },
            { new Coord(12, 12), Bonus.DoubleWord },
            { new Coord(11, 11), Bonus.DoubleWord },
            { new Coord(10, 10), Bonus.DoubleWord },

            { new Coord(5, 1), Bonus.TripleLetter },
            { new Coord(9, 1), Bonus.TripleLetter },
            { new Coord(1, 5), Bonus.TripleLetter },
            { new Coord(5, 5), Bonus.TripleLetter },
            { new Coord(9, 5), Bonus.TripleLetter },
            { new Coord(13, 5), Bonus.TripleLetter },
            { new Coord(1, 9), Bonus.TripleLetter },
            { new Coord(5, 9), Bonus.TripleLetter },
            { new Coord(9, 9), Bonus.TripleLetter },
            { new Coord(13, 9), Bonus.TripleLetter },
            { new Coord(5, 13), Bonus.TripleLetter },
            { new Coord(9, 13), Bonus.TripleLetter },

            { new Coord(3, 0), Bonus.DoubleLetter },
            { new Coord(11, 0), Bonus.DoubleLetter },
            { new Coord(6, 2), Bonus.DoubleLetter },
            { new Coord(8, 2), Bonus.DoubleLetter },
            { new Coord(0, 3), Bonus.DoubleLetter },
            { new Coord(7, 3), Bonus.DoubleLetter },
            { new Coord(14, 3), Bonus.DoubleLetter },
            { new Coord(2, 6), Bonus.DoubleLetter },
            { new Coord(6, 6), Bonus.DoubleLetter },
            { new Coord(8, 6), Bonus.DoubleLetter },
            { new Coord(12, 6), Bonus.DoubleLetter },
            { new Coord(3, 7), Bonus.DoubleLetter },
            { new Coord(11, 7), Bonus.DoubleLetter },
            { new Coord(2, 8), Bonus.DoubleLetter },
            { new Coord(6, 8), Bonus.DoubleLetter },
            { new Coord(8, 8), Bonus.DoubleLetter },
            { new Coord(12, 8), Bonus.DoubleLetter },
            { new Coord(0, 11), Bonus.DoubleLetter },
            { new Coord(7, 11), Bonus.DoubleLetter },
            { new Coord(14, 11), Bonus.DoubleLetter },
            { new Coord(6, 12), Bonus.DoubleLetter },
            { new Coord(8, 12), Bonus.DoubleLetter },
            { new Coord(3, 14), Bonus.DoubleLetter },
            { new Coord(11, 14), Bonus.DoubleLetter },
        };

        // Used when generating the Tiles. The space is used for the blank tiles
        private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";

        private const string uriPrefix = "pack://application:,,,/";

        private readonly Dictionary<char, int> letterNums = new()
        {
            { 'A', 9 },
            { 'B', 2 },
            { 'C', 2 },
            { 'D', 4 },
            { 'E', 12 },
            { 'F', 2 },
            { 'G', 3 },
            { 'H', 2 },
            { 'I', 9 },
            { 'J', 1 },
            { 'K', 1 },
            { 'L', 4 },
            { 'M', 2 },
            { 'N', 6 },
            { 'O', 8 },
            { 'P', 2 },
            { 'Q', 1 },
            { 'R', 6 },
            { 'S', 4 },
            { 'T', 6 },
            { 'U', 4 },
            { 'V', 2 },
            { 'W', 2 },
            { 'X', 1 },
            { 'Y', 2 },
            { 'Z', 1 },
            { ' ', 2 },
        };

        private List<Tile> letterPool = new();

        private List<Tile> playedTiles = new();

        private readonly List<User> players;

        private User currentPlayer;

        private readonly BitmapImage nullImage = new(new Uri($"{uriPrefix}Blank Image.png", UriKind.Absolute));

        private List<Tile> turnTiles = new();

        public MainWindow()
        {
            InitializeComponent();

            string givenName = "Place Holder";

            //Generate gameboard
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Coord coord = new(i, j);

                    Border border = new();
                    border.BorderBrush = Brushes.White;
                    border.BorderThickness = new Thickness(1, 1, 1, 1);
                    border.CornerRadius = new CornerRadius(5);
                    //border.AllowDrop = true;
                    //border.Drop = new DragEventHandler()

                    Image image = new();
                    image.Margin = new Thickness(0);
                    image.Stretch = Stretch.Uniform;
                    image.AllowDrop = true;
                    image.Drop += DropLetter;
                    

                    Viewbox viewbox = new();
                    viewbox.Stretch = Stretch.Uniform;

                    //check for bonuses
                    image.Source = GetBonusImageFromCoord(coord);
                    border.Background = GetBonusColourFromCoord(coord);

                    viewbox.Child = image;
                    border.Child = viewbox;
                    playGrid.Children.Add(border);
                }
            }

            //generate letters
            for (int i=0; i<27; i++)
            {
                int numToCreate = letterNums[alphabet[i]];
                for (int j=0; j<numToCreate; j++)
                {
                    letterPool.Add(new Tile(alphabet[j]));
                }
            }

            //shuffle
            int length = letterPool.Count;
            for (int i = 0; i < length; i++) ;
            Random rnd = new();
            for (int i = 0; i < length; i++)        // for each letter in the list, swap it with another one
            {
                int dest = rnd.Next(0, length - 1);
                Tile shuffleTile = letterPool[dest];
                letterPool[dest] = letterPool[i];
                letterPool[i] = shuffleTile;
            }

            //Create the player and AI
            players = new();
            players.Add(new User(givenName, letterPool.GetRange(0, 7).ToArray()));
            letterPool.RemoveRange(0, 7);
            players.Add(new AI(givenName, letterPool.GetRange(0, 7).ToArray(), AI.Difficulty.Medium));
            letterPool.RemoveRange(0, 7);
            currentPlayer = players[0];

            //add user's tiles to the tile dock
            foreach (Tile tile in players[0].Tiles)
            {
                char letter = tile.Letter;
                Image image = new();
                image.Source = new BitmapImage(new Uri($"{uriPrefix}letters/{letter}.png", UriKind.Absolute));
                //image.AllowDrop = true;
                image.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragLetter);
                tileDock.Children.Add(image);
            }
        }

        /*private void AddTilesToDock(User user)
        {
            int numNeeded = 7 - user.Tiles.Length;

            for (int i = 0; i < numNeeded; i++)
            {
                
            }
        }*/
        private BitmapImage GetBonusImageFromCoord(Coord coord)
        {
            if (bonusLocations.TryGetValue(coord, out Bonus bonus))
            {
                return bonus switch
                {
                    Bonus.TripleWord => new BitmapImage(new Uri($"{uriPrefix}3x word.png", UriKind.Absolute)),
                    Bonus.DoubleWord => new BitmapImage(new Uri($"{uriPrefix}2x word.png", UriKind.Absolute)),
                    Bonus.TripleLetter => new BitmapImage(new Uri($"{uriPrefix}3x letter.png", UriKind.Absolute)),
                    Bonus.DoubleLetter => new BitmapImage(new Uri($"{uriPrefix}2x letter.png", UriKind.Absolute)),
                    _ => nullImage,
                };
            }
            else if (coord.X == 7 && coord.Y == 7)
            {
                return new BitmapImage(new Uri($"{uriPrefix}Star.png", UriKind.Absolute));
            }
            else
            {
                return nullImage;
            }
        }

        private SolidColorBrush GetBonusColourFromCoord(Coord coord)
        {
            if (bonusLocations.TryGetValue(coord, out Bonus bonus))
            {
                return bonus switch
                {
                    Bonus.TripleWord => Brushes.Red,
                    Bonus.DoubleWord => Brushes.LightPink,
                    Bonus.TripleLetter => Brushes.DarkBlue,
                    Bonus.DoubleLetter => Brushes.LightBlue,
                    _ => null,
                };
            }
            else
            {
                return new SolidColorBrush(Color.FromRgb(0x79, 0x97, 0xB9));
            }
        }

        //start drag
        private void DragLetter(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            DataObject data = new();
            data.SetData(image.Source);
            //data.SetData(false);

            ImageSource originalImage = image.Source;

            //drag from tile dock
            if (image.Parent == tileDock)
            {
                tileDock.Children.Remove(image);
                if (DragDrop.DoDragDrop(image, data, DragDropEffects.Move) == DragDropEffects.None)
                {
                    //drag operation failed
                    tileDock.Children.Add(image);
                }
                else
                {
                    data.SetData(true);
                }
            }
            else    //drag from gameboard
            {
                Tile tile = GetTileFromImage(image);
                image.Source = nullImage;
                Debug.WriteLine($"col: {Grid.GetColumn(image)} row: {Grid.GetRow(image)}");
                if (DragDrop.DoDragDrop(image, data, DragDropEffects.Move) == DragDropEffects.None)
                {
                    //drag operation failed
                    image.Source = originalImage;
                }
                else
                {
                    foreach (Tile loopTile in turnTiles)
                    {
                        if (loopTile.Coord == GetGridCoord(image))
                        {
                            turnTiles.Remove(loopTile);
                            break;
                        }
                    }
                    //turnTiles.Remove(tile);
                }
            }

            bool validArrangement = CheckLetterLocation(turnTiles);
            Debug.WriteLine(validArrangement);

            Word testWord = new Word(turnTiles);
            Debug.WriteLine(testWord.Validate());

        }

        //end drag
        private void DropLetter(object sender, DragEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = (ImageSource)e.Data.GetData("System.Windows.Media.Imaging.BitmapImage");
            image.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragLetter);

            //match dragged letter to tile
            Tile tile = GetTileFromImage(image);
            turnTiles.Add(tile);
        }

        //match an image object to one on the board
        /*private Image FindImage(Image matchImage)
        {
            foreach (Border border in playGrid.Children)
            {
                Viewbox viewbox = (Viewbox)border.Child;
                Image loopImage = (Image)viewbox.Child;
                if (loopImage == matchImage)
                {
                    return loopImage;
                }
            }
            return null;
        }*/

        //Gets the letter represented by an Image
        private char GetLetterFromImage(Image image)
        {
            ImageSource imageSource = image.Source;
            Uri imageUri = ((BitmapImage)imageSource).UriSource;
            string uriString = imageUri.ToString();
            string checkString = $"{uriPrefix}letters/ .png";

            //validate that the image is of a letter
            if (uriString.StartsWith($"{uriPrefix}letters/") && uriString.EndsWith(".png") && uriString.Length == checkString.Length)
            {
                return uriString[uriPrefix.Length + "letters.".Length];
            }
            else
            {
                throw new ArgumentException(message: $"{nameof(image)} does not represent a letter");
            }
        }

        //Gets the location of an image on the gameboard
        private Coord GetGridCoord(Image matchImage)
        {
            int columns = playGrid.Columns;

            int index = playGrid.Children.IndexOf((matchImage.Parent as Viewbox).Parent as Border);
            if (index == -1)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(matchImage), message: "Image not found on the gameboard");
            }

            int row = index / columns;
            int column = index % columns;
            Debug.WriteLine($"Found at ({column}, {row})");
            return new Coord(column, row);
        }

        private Image GetImageFromCoord(Coord coord)
        {
            int index = coord.Y * 15 + coord.X;
            Border border = (Border)playGrid.Children[index];
            Viewbox viewbox = (Viewbox)border.Child;
            return (Image)viewbox.Child;
        }

        //Generates a Tile from an image
        private Tile GetTileFromImage(Image image)
        {
            return new Tile(GetLetterFromImage(image), GetGridCoord(image));
        }

        private Orientation GetOrientation(List<Tile> tiles)
        {
            if (tiles[0].Coord.X == tiles[1].Coord.X)
            {
                //all x values should be the same
                return Orientation.Vertical;
            }
            else if (tiles[0].Coord.Y == tiles[1].Coord.Y)
            {
                //all y values should be the same
                return Orientation.Horizontal;
            }
            else
            {
                throw new ArgumentException(message: $"The contents of {nameof(tiles)} are arranged in an invalid way.", paramName: nameof(tiles));
            }
        }

        private bool CheckLetterLocation(List<Tile> tiles)
        {
            if (players[0].NumTurns == 0)
            {
                bool centre = false;
                foreach (Tile tile in tiles)
                {
                    if (tile.Coord == new Coord(7, 7))
                    {
                        centre = true;
                        break;
                    }
                }
                if (!centre)
                {
                    return false;
                }
            }

            Orientation orientation;
            if (tiles.Count == 1)
            {
                //only one letter placed, so no need to check
                return true;
            }
            else
            {
                int same;
                orientation = GetOrientation(tiles);
                if (orientation == Orientation.Vertical)
                {
                    //all x values should be the same
                    same = tiles[0].Coord.X;
                    foreach (Tile tile in tiles)
                    {
                        if (tile.Coord.X != same)
                        {
                            //Not all in a line, so arrangement not valid
                            return false;
                        }
                    }
                }
                else if (orientation == Orientation.Horizontal)
                {
                    //all y values should be the same
                    same = tiles[0].Coord.Y;
                    foreach (Tile tile in tiles)
                    {
                        if (tile.Coord.Y != same)
                        {
                            //not all in a line, so arrangement not valid.
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                //Check tiles are adjecent
                List<Tile> sortedTiles = SortTiles(tiles, orientation);
                for (int i=1; i<sortedTiles.Count; i++)
                {
                    if (orientation == Orientation.Vertical)
                    {
                        if (sortedTiles[i].Coord.Y != sortedTiles[i - 1].Coord.Y + 1)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (sortedTiles[i].Coord.X != sortedTiles[i - 1].Coord.X + 1)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        private List<Tile> SortTiles(List<Tile> tiles, Orientation orientation)
        {
            List<Tile> sortedTiles;
            if (orientation == Orientation.Horizontal)
            {
                sortedTiles = tiles.OrderBy(tile => tile.Coord.X).ToList();
            }
            else
            {
                sortedTiles = tiles.OrderBy(tile => tile.Coord.Y).ToList(); 
            }
            return sortedTiles;
        }

        private void FinishTurn(object sender, RoutedEventArgs e)
        {
            if (CheckLetterLocation(turnTiles))
            {
                Word word = new Word(SortTiles(turnTiles, GetOrientation(turnTiles)));
                currentPlayer.AddWord(word);    //add created word to the player's list of words
                AddWordToPanel(word);   //add word to side panel

                currentPlayer.IncrementTurns();

                //lock letters in place
                foreach (Tile tile in turnTiles)
                {
                    playedTiles.Add(tile);
                    Image image = GetImageFromCoord(tile.Coord);
                    int index = playGrid.Children.IndexOf((image.Parent as Viewbox).Parent as Border);
                    Debug.WriteLine($"Finish turn index: {index}");
                    image.AllowDrop = false;
                    image.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(DragLetter);
                }

                //To work on
                List<Tile> newTiles = letterPool.Take(7 - turnTiles.Count).ToList();
                currentPlayer.ChangeTiles(turnTiles, newTiles);
                //

                turnTiles.Clear();
            }
        }

        private void AddWordToPanel(Word word)
        {
            StackPanel stackPanel = new();
            stackPanel.Orientation = Orientation.Horizontal;

            Label label = new();
            label.Content = word.ToString();
            label.Style = (Style)FindResource("smallTextStyle");

            Button button = new();
            button.Content = "🔍";
            ToolTip toolTip = new();
            toolTip.Content = "Get definition";

            button.ToolTip = toolTip;
            stackPanel.Children.Add(label);
            stackPanel.Children.Add(button);

            wordList.Children.Add(stackPanel);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        // Used when generating the Tiles. The space is used for the blank tiles
        private const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";// ";

        private const string uriPrefix = "pack://application:,,,/";

        private static readonly Dictionary<char, int> letterNums = new()
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

        private bool isFirstTurn = true;

        private bool gameOver = false;

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
            foreach (char letter in alphabet)
            {
                int numToCreate = letterNums[letter];
                for (int j=0; j<numToCreate; j++)
                {
                    letterPool.Add(new Tile(letter));
                }
            }

            //shuffle
            int length = letterPool.Count;
            Random rnd = new();
            for (int i = 0; i < length; i++)        // for each letter in the list, swap it with another one
            {
                int dest = rnd.Next(0, length);
                Tile shuffleTile = letterPool[dest];
                letterPool[dest] = letterPool[i];
                letterPool[i] = shuffleTile;
            }

            //Create the player and AI
            players = new();
            players.Add(new User(givenName, letterPool.GetRange(0, 7)));
            letterPool.RemoveRange(0, 7);
            players.Add(new AI(givenName, letterPool.GetRange(0, 7), AI.Difficulty.High));
            letterPool.RemoveRange(0, 7);
            currentPlayer = players[0];

            //display the user's tiles on the screen
            AddTilesToDock(currentPlayer);
        }

        //add user's tiles to the tile dock
        private void AddTilesToDock(User user)
        {
            tileDock.Children.Clear();
            foreach (Tile tile in user.Tiles)
            {
                char letter = tile.Letter;
                Image image = new();
                image.Source = tile.Image;
                //image.AllowDrop = true;
                image.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragLetter);
                tileDock.Children.Add(image);
            }
        }

        /*private BitmapImage GetImageSourceFromLetter(char letter)
        {
            return new BitmapImage(new Uri($"{uriPrefix}letters/{letter}.png", UriKind.Absolute));
        }*/

        private BitmapImage GetBonusImageFromCoord(Coord coord)
        {
            if (coord.X == 7 && coord.Y == 7)
            {
                return new BitmapImage(new Uri($"{uriPrefix}Star.png", UriKind.Absolute));
            }

            Bonus bonus = coord.Bonus;
            return bonus switch
            {
                Bonus.TripleWord => new BitmapImage(new Uri($"{uriPrefix}3x word.png", UriKind.Absolute)),
                Bonus.DoubleWord => new BitmapImage(new Uri($"{uriPrefix}2x word.png", UriKind.Absolute)),
                Bonus.TripleLetter => new BitmapImage(new Uri($"{uriPrefix}3x letter.png", UriKind.Absolute)),
                Bonus.DoubleLetter => new BitmapImage(new Uri($"{uriPrefix}2x letter.png", UriKind.Absolute)),
                _ => nullImage,
            };

            /*if (bonusLocations.TryGetValue(coord, out Bonus bonus))
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
            else
            {
                return nullImage;
            }*/
        }

        private SolidColorBrush GetBonusColourFromCoord(Coord coord)
        {
            Bonus bonus = coord.Bonus;

            return bonus switch
            {
                Bonus.TripleWord => Brushes.Red,
                Bonus.DoubleWord => Brushes.LightPink,
                Bonus.TripleLetter => Brushes.DarkBlue,
                Bonus.DoubleLetter => Brushes.LightBlue,
                _ => new SolidColorBrush(Color.FromRgb(0x79, 0x97, 0xB9)),
            };

            /*if (bonusLocations.TryGetValue(coord, out Bonus bonus))
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
            }*/
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

            /*bool validArrangement = Word.CheckLetterLocation(turnTiles, playedTiles, currentPlayer);
            Debug.WriteLine(validArrangement);

            Word testWord = new Word(turnTiles);
            Debug.WriteLine(testWord.Validate());*/

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

        //returns the image at a given coord
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

        //Called when the button to finish a turn is clicked
        private async void FinishTurnAsync(object sender, RoutedEventArgs e)
        {
            await FinishTurnAsync();
        }

        private async Task FinishTurnAsync()
        {
            List<Word> words;
            if (currentPlayer is AI ai)
            {
                turnTiles = await ai.GetTilesToPlaceAsync(playedTiles);
                words = Word.GetInterLinkedWords(turnTiles, playedTiles);
                if (words.Count == 0)
                {
                    gameOver = true;
                }
            }
            else
            {
                words = Word.GetInterLinkedWords(turnTiles, playedTiles);
            }


            if (Word.CheckLetterLocation(turnTiles, playedTiles, isFirstTurn) && words.Count > 0)
            {
                int score = 0;
                foreach (Word word in words)
                {
                    currentPlayer.AddWord(word);    //add created word to the player's list of words
                    AddWordToPanel(word);   //add word to side panel
                    //word.GetPopularity();
                    score += word.Value;
                }

                //lock letters in place
                foreach (Tile tile in turnTiles)
                {
                    playedTiles.Add(tile);
                    Image image = GetImageFromCoord(tile.Coord);
                    image.AllowDrop = false;
                    image.PreviewMouseLeftButtonDown -= new MouseButtonEventHandler(DragLetter);

                    if (currentPlayer is AI)
                    {
                        image.Source = tile.Image;
                    }
                }

                currentPlayer.IncrementTurns();

                List<Tile> newTiles = letterPool.Take(turnTiles.Count).ToList();
                letterPool.RemoveRange(0, newTiles.Count); //to document
                currentPlayer.ChangeTiles(turnTiles, newTiles);

                turnTiles.Clear();

                //update UI
                if (currentPlayer is not AI)
                {
                    AddTilesToDock(currentPlayer);
                    userScoreLabel.Content = currentPlayer.Score;
                }
                else
                {
                    computerScoreLabel.Content = currentPlayer.Score;
                }

                //check if game over
                if (letterPool.Count == 0 && currentPlayer.IsOutOfTiles)
                {
                    gameOver = true;
                }
            }

            if (gameOver)
            {
                GameOverWindow gameOverWindow = new(players);
                gameOverWindow.Owner = this;
                gameOverWindow.ShowDialog();
            }
            else 
            {
                //remove used bonuses
                foreach (Word word in words)
                {
                    foreach (Tile tile in word.word)
                    {
                        tile.Coord.RemoveBonus();
                    }
                }

                isFirstTurn = false;
                SwitchPlayer();
                if (currentPlayer is AI)
                {
                    await FinishTurnAsync();
                }
            }
        }

        private void SwitchPlayer()
        {
            int index = players.IndexOf(currentPlayer);
            index++;
            if (index > players.Count - 1)
            {
                index = 0;
            }
            currentPlayer = players[index];
        }

        //Adds a word to the panel of created words
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private readonly List<User> players;

        private readonly BitmapImage nullImage = new(new Uri("pack://application:,,,/Blank Image.png", UriKind.Absolute));

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
                    border.Background = new SolidColorBrush(Color.FromRgb(0x79, 0x97, 0xB9));
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
                    if (bonusLocations.TryGetValue(coord, out Bonus bonus))
                    {
                        if (bonus == Bonus.TripleWord)
                        {
                            image.Source = new BitmapImage(new Uri("pack://application:,,,/3x word.png", UriKind.Absolute));
                            border.Background = Brushes.Red;
                        }
                        if (bonus == Bonus.DoubleWord)
                        {
                            image.Source = new BitmapImage(new Uri("pack://application:,,,/2x word.png", UriKind.Absolute));
                            border.Background = Brushes.LightPink;
                        }
                        if (bonus == Bonus.TripleLetter)
                        {
                            image.Source = new BitmapImage(new Uri("pack://application:,,,/3x letter.png", UriKind.Absolute));
                            border.Background = Brushes.DarkBlue;
                        }
                        if (bonus == Bonus.DoubleLetter)
                        {
                            image.Source = new BitmapImage(new Uri("pack://application:,,,/2x letter.png", UriKind.Absolute));
                            border.Background = Brushes.LightBlue;
                        }
                    }
                    else if (i == 7 && j == 7)
                    {
                        image.Source = new BitmapImage(new Uri("pack://application:,,,/Star.png", UriKind.Absolute));
                    }
                    else
                    {
                        image.Source = nullImage;
                    }
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
            Random rnd = new Random();
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

            //add user's tiles to the tile dock
            foreach (Tile tile in players[0].Tiles)
            {
                char letter = tile.Letter;
                Image image = new();
                image.Source = new BitmapImage(new Uri($"pack://application:,,,/letters/{letter}.png", UriKind.Absolute));
                //image.AllowDrop = true;
                image.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragLetter);
                tileDock.Children.Add(image);
            }
        }

        //start drag
        private void DragLetter(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            DataObject data = new();
            data.SetData(image.Source);
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

            }
            else    //drag from gameboard
            {
                foreach (Border border in playGrid.Children)
                {
                    Viewbox viewbox = (Viewbox)border.Child;
                    Image loopImage = (Image)viewbox.Child;
                    if (loopImage == image)
                    {
                        loopImage.Source = nullImage;
                        if (DragDrop.DoDragDrop(image, data, DragDropEffects.Move) == DragDropEffects.None)
                        {
                            //drag operation failed
                            Debug.WriteLine(image);
                            Debug.WriteLine(originalImage);
                            loopImage.Source = originalImage;
                            Debug.WriteLine("it gets here");
                        }
                        break;
                    }
                }
            }
        }

        //end drag
        private void DropLetter(object sender, DragEventArgs e)
        {
            Image image = (Image)sender;
            image.Source = (ImageSource)e.Data.GetData("System.Windows.Media.Imaging.BitmapImage");
            image.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragLetter);
        }
    }
}

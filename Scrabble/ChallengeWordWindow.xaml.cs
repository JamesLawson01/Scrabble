using System.Windows;

namespace Scrabble
{
    /// <summary>
    /// Interaction logic for ChallengeWordWindow.xaml
    /// </summary>
    public partial class ChallengeWordWindow : Window
    {
        public ChallengeWordWindow(bool valid, Word word)
        {
            InitializeComponent();

            if (valid)
            {
                outcomeLabel.Content = $"{word} is a valid word";
            }
            else
            {
                outcomeLabel.Content = $"{word} is not a valid word";
            }
        }
    }
}

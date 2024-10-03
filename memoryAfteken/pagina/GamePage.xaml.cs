using memoryAfteken.Business;
using memoryAfteken.Models;
using memoryAfteken.DataAccess;
using memoryAfteken.Controls;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace memoryAfteken.pagina
{
    public partial class GamePage : ContentPage
    {
        private MemoryGameLogic _gameLogic;
        private IGameRepository _repository;
        private FlipCardView _firstButton;
        private FlipCardView _secondButton;
        private int _firstCardIndex;
        private int _secondCardIndex;
        private bool _isProcessing = false;

        public GamePage(int numberOfPairs)
        {
            InitializeComponent();
            _repository = new GameRepository();
            _gameLogic = new MemoryGameLogic(_repository);
            StartGame(numberOfPairs);
        }

        private void StartGame(int numberOfPairs)
        {
            _gameLogic.StartGame(numberOfPairs);
            ScoreLabel.Text = $"Score: {_gameLogic.Score}";
            CreateCardGrid();
        }

        private void CreateCardGrid()
        {
            CardGrid.Children.Clear();
            CardGrid.RowDefinitions.Clear();
            CardGrid.ColumnDefinitions.Clear();

            int totalCards = _gameLogic.GetCards().Count;
            int numColumns = 4;
            int numRows = (int)Math.Ceiling((double)totalCards / numColumns);

            for (int i = 0; i < numRows; i++)
            {
                CardGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
            for (int i = 0; i < numColumns; i++)
            {
                CardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    int cardIndex = row * numColumns + col;
                    if (cardIndex >= totalCards)
                        break;

                    var card = _gameLogic.GetCards()[cardIndex];

                    // Create the FlipCardView control with images
                    var flipCard = new FlipCardView(
                        frontImageSource: card.ImageName,    // Use the card's image name
                        backImageSource: "back_of_card.png"  // Use back-of-card image
                    )
                    {
                        CardIndex = cardIndex,
                        WidthRequest = 80,
                        HeightRequest = 100
                    };

                    // Add tap gesture recognizer
                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += OnCardTapped;
                    flipCard.GestureRecognizers.Add(tapGesture);

                    // Add the FlipCardView to the grid
                    CardGrid.Add(flipCard, col, row);
                }
            }
        }

        private async void OnCardTapped(object sender, EventArgs e)
        {
            if (_isProcessing)
                return;

            var flipCard = (FlipCardView)sender;
            int cardIndex = flipCard.CardIndex;
            var card = _gameLogic.GetCards()[cardIndex];

            // If the card is already flipped or matched, ignore the tap
            if (card.IsFlipped || card.IsMatched)
                return;

            _isProcessing = true;

            // Flip the card and update the score
            _gameLogic.FlipCard(cardIndex);
            ScoreLabel.Text = $"Score: {_gameLogic.Score}";

            // Perform the flip animation to show the front image
            await flipCard.FlipToFront();

            // Check if this is the first or second card being flipped
            if (_firstButton == null)
            {
                _firstButton = flipCard;
                _firstCardIndex = cardIndex;
                _isProcessing = false;
            }
            else
            {
                _secondButton = flipCard;
                _secondCardIndex = cardIndex;

                // Wait for a moment to show the second card
                await Task.Delay(500);

                // Check if the second card matches the first card
                if (!_gameLogic.GetCards()[_firstCardIndex].IsMatched)
                {
                    // If not matched, flip both cards back
                    await _firstButton.FlipToBack();
                    await _secondButton.FlipToBack();

                    // Flip the cards back in the game logic
                    _gameLogic.GetCards()[_firstCardIndex].IsFlipped = false;
                    _gameLogic.GetCards()[_secondCardIndex].IsFlipped = false;
                }

                // Reset for the next round of flipping
                _firstButton = null;
                _secondButton = null;

                // Update the score after processing the second card
                ScoreLabel.Text = $"Score: {_gameLogic.Score}";

                _isProcessing = false;
            }

            // Check if the game is over
            if (_gameLogic.GetCards().All(c => c.IsMatched))
            {
                // Prompt the user for their name
                string playerName = await DisplayPromptAsync("Game Over", "Enter your name:", "OK", "Cancel", "Player", 10, Keyboard.Text);

                if (string.IsNullOrWhiteSpace(playerName))
                {
                    playerName = "Player";
                }

                await _gameLogic.EndGameAsync(playerName);
                await DisplayAlert("Game Over", $"Your final score: {_gameLogic.Score}", "OK");

                // Navigate to high scores page or reset the game
                await Navigation.PopAsync();
            }
        }
    }
}

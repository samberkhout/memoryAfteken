using memoryAfteken.Business;
using memoryAfteken.Models;
using DataAccess.DataAccess;

using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using memoryAfteken.Controls;

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
        private int _numberOfAttempts = 0; // Track number of attempts
        private DateTime _startTime; // Track the start time

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
            _startTime = DateTime.Now; // Record start time
            _numberOfAttempts = 0; // Reset number of attempts
            ScoreLabel.Text = $"Score: {_gameLogic.Score}";
            CreateCardGrid();
        }

        private void CreateCardGrid()
        {
            CardGrid.Children.Clear();
            CardGrid.RowDefinitions.Clear();
            CardGrid.ColumnDefinitions.Clear();

            int totalCards = _gameLogic.GetCards().Count;
            int numColumns = 4; // Adjust columns as needed
            int numRows = (int)Math.Ceiling((double)totalCards / numColumns);

            // Add RowDefinitions and ColumnDefinitions
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
                    
                    var flipCard = new FlipCardView(
                        frontImageSource: card.ImageName,    
                        backImageSource: "back_of_card.png" 
                    )
                    {
                        CardIndex = cardIndex,
                        WidthRequest = 100,
                        HeightRequest = 100,
                        Margin = new Thickness(10)
                    };
                    
                    flipCard.HorizontalOptions = LayoutOptions.Center;
                    flipCard.VerticalOptions = LayoutOptions.Center;
                    
                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += OnCardTapped;
                    flipCard.GestureRecognizers.Add(tapGesture);
                    
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
            
            if (card.IsFlipped || card.IsMatched)
                return;

            _isProcessing = true;
            
            _numberOfAttempts++;
            
            _gameLogic.FlipCard(cardIndex);
            UpdateScore();
            
            await flipCard.FlipToFront();
            
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
                
                await Task.Delay(500);
                
                if (!_gameLogic.GetCards()[_firstCardIndex].IsMatched)
                {
                    await _firstButton.FlipToBack();
                    await _secondButton.FlipToBack();

                  
                    _gameLogic.GetCards()[_firstCardIndex].IsFlipped = false;
                    _gameLogic.GetCards()[_secondCardIndex].IsFlipped = false;
                }
                
                _firstButton = null;
                _secondButton = null;
                
                UpdateScore();

                _isProcessing = false;
            }

            if (_gameLogic.GetCards().All(c => c.IsMatched))
            {
               
                string playerName = await DisplayPromptAsync("Game Over", "Enter your name:", "OK", "Cancel", "Player", 10, Keyboard.Text);

                if (string.IsNullOrWhiteSpace(playerName))
                {
                    playerName = "Player";
                }

                await _gameLogic.EndGameAsync(playerName);
                await DisplayAlert("Game Over", $"Your final score: {_gameLogic.Score}", "OK");

                
                await Navigation.PopAsync();
            }
        }

        private void UpdateScore()
        {
            var timeTaken = (DateTime.Now - _startTime).TotalSeconds;
            double calculatedScore = ((_gameLogic.GetCards().Count * _gameLogic.GetCards().Count) / (timeTaken * _numberOfAttempts)) * 1000;
            _gameLogic.Score = (int)Math.Round(calculatedScore, 2);

            // Update score label
            ScoreLabel.Text = $"Score: {_gameLogic.Score}";
        }
    }
}


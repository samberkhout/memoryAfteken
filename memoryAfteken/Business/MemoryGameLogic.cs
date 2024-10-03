using memoryAfteken.Models;
using memoryAfteken.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace memoryAfteken.Business
{
    public class MemoryGameLogic
    {
        private readonly IGameRepository _repository;
        private List<Card> _cards;
        private DateTime _startTime;
        private int _score;
        private Card _firstFlippedCard;

        public MemoryGameLogic(IGameRepository repository)
        {
            _repository = repository;
            _score = 0;
        }

        public void StartGame(int numberOfPairs)
        {
            _cards = GenerateCards(numberOfPairs);
            _startTime = DateTime.Now;
            _firstFlippedCard = null;
            _score = 0;
        }

        private List<Card> GenerateCards(int numberOfPairs)
        {
            var cards = new List<Card>();

            // List of image file names for card pairs
            var imageNames = new List<string>();
            for (int i = 1; i <= 12; i++) // Assuming there are 12 images
            {
                imageNames.Add($"img{i}.jpg");
            }

            // Select only the number of images required for the given number of pairs
            imageNames = imageNames.Take(numberOfPairs).ToList();

            // Create pairs of cards with the same image name
            for (int i = 0; i < numberOfPairs; i++)
            {
                string imageName = imageNames[i];

                var card1 = new Card
                {
                    Id = i * 2,
                    Value = (i + 1).ToString(),
                    IsFlipped = false,
                    IsMatched = false,
                    ImageName = imageName // Assign image name
                };
                var card2 = new Card
                {
                    Id = i * 2 + 1,
                    Value = (i + 1).ToString(),
                    IsFlipped = false,
                    IsMatched = false,
                    ImageName = imageName
                };
                cards.Add(card1);
                cards.Add(card2);
            }

            // Shuffle the cards
            var rand = new Random();
            for (int i = 0; i < cards.Count; i++)
            {
                int swapIndex = rand.Next(cards.Count);
                var temp = cards[i];
                cards[i] = cards[swapIndex];
                cards[swapIndex] = temp;
            }

            return cards;
        }

        public List<Card> GetCards()
        {
            return _cards;
        }

        public void FlipCard(int cardIndex)
        {
            if (_cards[cardIndex].IsFlipped || _cards[cardIndex].IsMatched)
                return;

            // Flip the card
            _cards[cardIndex].IsFlipped = true;

            // Decrease score for flipping a card
            _score -= 1;

            if (_firstFlippedCard == null)
            {
                _firstFlippedCard = _cards[cardIndex];
            }
            else
            {
                if (_firstFlippedCard.Value == _cards[cardIndex].Value)
                {
                    _firstFlippedCard.IsMatched = true;
                    _cards[cardIndex].IsMatched = true;

                    // Increase score for a match
                    _score += 10;
                }
                else
                {
                    // Decrease score for a mismatch
                    _score -= 5;
                }

                // Prevent negative score
                _score = Math.Max(0, _score);

                _firstFlippedCard = null;
            }
        }

        public async Task EndGameAsync(string playerName)
        {
            var endTime = DateTime.Now;
            var timeTaken = (endTime - _startTime).TotalSeconds;

            // Store high score if it qualifies
            try
            {
                var highScores = await _repository.GetHighScoresAsync();
                if (highScores.Count < 10 || _score > highScores[highScores.Count - 1].Score)
                {
                    var newHighScore = new HighScore
                    {
                        PlayerName = playerName,
                        Score = _score,
                        Date = DateTime.Now
                    };
                    await _repository.AddHighScoreAsync(newHighScore);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine($"Error updating high scores: {ex.Message}");
            }
        }

        public int Score => _score;
    }
}

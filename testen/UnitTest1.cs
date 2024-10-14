namespace testen;
    using memoryAfteken.Business;
using memoryAfteken.Models;
using DataAccess.DataAccess;
using Moq;
using System.Collections.Generic;
using Xunit;
    
    public class UnitTest1
    {
        private readonly Mock<IGameRepository> _mockRepository;

        public UnitTest1()
        {
            _mockRepository = new Mock<IGameRepository>();
        }

        [Fact]
        public void StartGame_ShouldInitializeCardsCorrectly()
        {
            // Arrange
            var logic = new MemoryGameLogic(_mockRepository.Object);
            int numberOfPairs = 4;

            // Act
            logic.StartGame(numberOfPairs);

            // Assert
            Assert.NotNull(logic.GetCards());
            Assert.Equal(numberOfPairs * 2, logic.GetCards().Count);
        }

        [Fact]
        public void FlipCard_ShouldFlipCardCorrectly()
        {
            // Arrange
            var logic = new MemoryGameLogic(_mockRepository.Object);
            logic.StartGame(2); // 2 pairs = 4 cards

            // Act
            logic.FlipCard(0);
            var card = logic.GetCards()[0];

            // Assert
            Assert.True(card.IsFlipped);
        }

        [Fact]
        public void FlipCard_ShouldNotFlipAlreadyMatchedCard()
        {
            // Arrange
            var logic = new MemoryGameLogic(_mockRepository.Object);
            logic.StartGame(2);
            var card = logic.GetCards()[0];
            card.IsMatched = true;

            // Act
            logic.FlipCard(0);

            // Assert
            Assert.False(card.IsFlipped);
        }

        [Fact]
        public void FlipCard_ShouldUpdateNumberOfAttempts()
        {
            // Arrange
            var logic = new MemoryGameLogic(_mockRepository.Object);
            logic.StartGame(2); // 2 pairs = 4 cards

            // Act
            logic.FlipCard(0);
            logic.FlipCard(1);

            // Assert
            Assert.Equal(2, logic.NumberOfAttempts);
        }

        [Fact]
        public void EndGame_ShouldSaveScoreIfInTopTen()
        {
            // Arrange
            var logic = new MemoryGameLogic(_mockRepository.Object);
            logic.StartGame(4);
            logic.FlipCard(0);
            logic.FlipCard(1);

            var highScores = new List<HighScore>
            {
                new HighScore { PlayerName = "Player1", Score = 300, Date = System.DateTime.Now },
                new HighScore { PlayerName = "Player2", Score = 250, Date = System.DateTime.Now }
            };

            _mockRepository.Setup(repo => repo.GetHighScoresAsync()).ReturnsAsync(highScores);

            // Act
            var endGameTask = logic.EndGameAsync("TestPlayer");
            endGameTask.Wait();

            // Assert
            _mockRepository.Verify(repo => repo.AddHighScoreAsync(It.IsAny<HighScore>()), Times.Once);
        }
    }

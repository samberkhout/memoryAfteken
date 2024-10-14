using memoryAfteken.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.DataAccess;
using memoryAfteken.Models;

    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize game repository and logic
            IGameRepository repository = new GameRepository();
            MemoryGameLogic gameLogic = new MemoryGameLogic(repository);

            Console.WriteLine("Welcome to the Memory Game!");
            Console.Write("Enter the number of pairs to play with (e.g., 4 for 8 cards): ");
            
            if (!int.TryParse(Console.ReadLine(), out int numberOfPairs))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }

            gameLogic.StartGame(numberOfPairs);

            int numRows = 4;
            int numColumns = (int)Math.Ceiling((double)(numberOfPairs * 2) / numRows);
            var cards = gameLogic.GetCards();

            bool gameRunning = true;

            // Game loop
            while (gameRunning)
            {
                PrintGrid(cards, numRows, numColumns);

                // Flip the first card
                Console.WriteLine("Enter the index of the first card to flip (e.g., 0 or 1): ");
                if (!int.TryParse(Console.ReadLine(), out int firstIndex) || firstIndex < 0 || firstIndex >= cards.Count)
                {
                    Console.WriteLine("Invalid card index. Try again.");
                    continue;
                }

                gameLogic.FlipCard(firstIndex);
                PrintGrid(cards, numRows, numColumns);

                // Flip the second card
                Console.WriteLine("Enter the index of the second card to flip: ");
                if (!int.TryParse(Console.ReadLine(), out int secondIndex) || secondIndex < 0 || secondIndex >= cards.Count || firstIndex == secondIndex)
                {
                    Console.WriteLine("Invalid card index. Try again.");
                    // Flip the first card back since the second card selection was invalid
                    gameLogic.FlipCard(firstIndex);
                    continue;
                }

                gameLogic.FlipCard(secondIndex);
                PrintGrid(cards, numRows, numColumns);

                // Pause for user to see both cards before proceeding
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();

                // Check if the cards match
                if (cards[firstIndex].Value == cards[secondIndex].Value)
                {
                    Console.WriteLine("It's a match!");
                    cards[firstIndex].IsMatched = true;
                    cards[secondIndex].IsMatched = true;
                }
                else
                {
                    Console.WriteLine("Not a match! Flipping the cards back.");
                    cards[firstIndex].IsFlipped = false;
                    cards[secondIndex].IsFlipped = false;
                }

                Console.WriteLine($"Current Score: {gameLogic.Score}");

                // Check if the game is over
                // After the game is over
                if (cards.TrueForAll(card => card.IsMatched))
                {
                    Console.WriteLine("Congratulations! You've matched all the cards!");
                    Console.WriteLine($"Your final score is being calculated...");

                    Console.Write("Enter your name to save your high score: ");
                    string playerName = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(playerName))
                    {
                        playerName = "Player";
                    }

                    // Calculate the score and save it to the database
                    await gameLogic.EndGameAsync(playerName);

                    Console.WriteLine("Your score has been saved successfully.");
                    gameRunning = false;
                }

            }

            Console.WriteLine("Thanks for playing!");
        }

        static void PrintGrid(List<Card> cards, int numRows, int numColumns)
        {
            Console.Clear();
            Console.WriteLine("Memory Game Board:");
            int cardCounter = 0;

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    if (cardCounter < cards.Count)
                    {
                        var card = cards[cardCounter];
                        if (card.IsFlipped || card.IsMatched)
                        {
                            Console.Write($"[{card.Value}] ");
                        }
                        else
                        {
                            Console.Write("[X] "); // X represents a face-down card
                        }
                    }
                    else
                    {
                        Console.Write("    "); // Empty space if no card is available
                    }

                    cardCounter++;
                }

                Console.WriteLine();
            }
        }
    }

using System.Collections.Generic;
using System;
using memoryAfteken.Models;
using memoryAfteken.Business;
using MySqlConnector;

namespace DataAccess.DataAccess
{
    public class GameRepository : IGameRepository
    {
        private readonly string _connectionString = "server=localhost;user=root;password=2006;database=mauitest";

        public async Task<List<HighScore>> GetHighScoresAsync()
        {
            var highScores = new List<HighScore>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT PlayerName, Score, Date FROM HighScores ORDER BY Score DESC LIMIT 10";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            highScores.Add(new HighScore
                            {
                                PlayerName = reader.GetString("PlayerName"),
                                Score = reader.GetInt32("Score"),
                                Date = reader.GetDateTime("Date")
                            });
                        }
                    }
                }
            }

            return highScores;
        }

        public async Task AddHighScoreAsync(HighScore highScore)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO HighScores (PlayerName, Score, Date) VALUES (@PlayerName, @Score, @Date)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PlayerName", highScore.PlayerName);
                    command.Parameters.AddWithValue("@Score", highScore.Score);
                    command.Parameters.AddWithValue("@Date", highScore.Date);

                    await command.ExecuteNonQueryAsync();
                }
                string deleteQuery = "DELETE FROM HighScores WHERE Id NOT IN (SELECT Id FROM (SELECT Id FROM HighScores ORDER BY Score DESC LIMIT 10) AS TopScores)";

                using (var deleteCommand = new MySqlCommand(deleteQuery, connection))
                {
                    await deleteCommand.ExecuteNonQueryAsync();
                }

            }
        }

    }
}

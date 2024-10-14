using System.Collections.Generic;
using System.Threading.Tasks;
using memoryAfteken.Models;

namespace memoryAfteken.Business
{
    public interface IGameRepository
    {
        Task<List<HighScore>> GetHighScoresAsync();
        Task AddHighScoreAsync(HighScore highScore);
    }
}
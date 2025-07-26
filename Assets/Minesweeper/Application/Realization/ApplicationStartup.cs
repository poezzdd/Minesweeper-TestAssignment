using Minesweeper.GameStarter.Interfaces;

namespace Minesweeper.Application.Realization
{
    public class ApplicationStartup
    {
        public ApplicationStartup(
            IGameStarter gameStarter)
        {
            gameStarter.StartGame();
        }
    }
}
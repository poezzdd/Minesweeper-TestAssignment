using System;

namespace Minesweeper.GameStarter.Interfaces
{
    public interface IGameStarter
    {
        event Action OnGameStartedEvent;
        event Action<bool> OnGameEndedEvent;
        void StartGame();
    }
}
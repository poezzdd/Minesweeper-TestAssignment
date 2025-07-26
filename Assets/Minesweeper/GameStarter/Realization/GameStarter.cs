using System;
using Minesweeper.GameStarter.Interfaces;
using Minesweeper.GameTimer.Interfaces;
using Minesweeper.MineField.Interfaces;

namespace Minesweeper.GameStarter.Realization
{
    public class GameStarter : IGameStarter, IDisposable
    {
        public event Action OnGameStartedEvent;
        public event Action<bool> OnGameEndedEvent;
        
        private IMineFieldController _mineFieldController;
        private IGameTimer _gameTimer;

        public GameStarter(
            IMineFieldController mineFieldController,
            IGameTimer gameTimer)
        {
            _mineFieldController = mineFieldController;
            _gameTimer = gameTimer;

            _mineFieldController.OnEndGameEvent += OnGameEnded;
        }
        
        public void Dispose()
        {
            OnGameStartedEvent = null;
            OnGameEndedEvent = null;
            
            _mineFieldController.OnEndGameEvent -= OnGameEnded;
            _mineFieldController = null;

            _gameTimer = null;
        }
        
        public void StartGame()
        {
            _mineFieldController.Start();
            _gameTimer.Start();
            
            OnGameStartedEvent?.Invoke();
        }

        private void OnGameEnded(bool isWin)
        {
            _gameTimer.Stop();
            
            OnGameEndedEvent?.Invoke(isWin);
        }
    }
}
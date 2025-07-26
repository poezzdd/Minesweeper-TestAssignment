using System;
using Minesweeper.Application;
using Minesweeper.GameStarter.Interfaces;
using Minesweeper.GameTimer.Interfaces;
using Minesweeper.MineField.Interfaces;
using UniRx;

namespace Minesweeper.UI.Realization
{
    public class UIGameWindowController : IDisposable
    {
        private UIGameWindow _uiGameWindow;
        private IGameStarter _gameStarter;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public UIGameWindowController(
            GameSceneData gameSceneData,
            IGameTimer gameTimer,
            IMineFieldController mineFieldController,
            IGameStarter gameStarter)
        {
            _uiGameWindow = gameSceneData.UIGameWindow;
            _gameStarter = gameStarter;

            _compositeDisposable.Add(gameTimer.TotalSeconds.Subscribe(SetTimerText));
            
            _compositeDisposable.Add(
                mineFieldController
                    .MinesLeft
                    .Subscribe(minesLeft => _uiGameWindow.MinesLeftText.text = minesLeft.ToString()));
            
            _uiGameWindow.OnRestartButtonClickedEvent += OnRestartButtonClicked;

            _gameStarter.OnGameStartedEvent += OnStartGame;
            _gameStarter.OnGameEndedEvent += OnEndGame;
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
            _compositeDisposable = null;
            
            _uiGameWindow.OnRestartButtonClickedEvent -= OnRestartButtonClicked;
            _uiGameWindow = null;
            
            _gameStarter.OnGameStartedEvent -= OnStartGame;
            _gameStarter.OnGameEndedEvent -= OnEndGame;
            _gameStarter = null;
        }

        private void SetTimerText(int seconds)
        {
            _uiGameWindow.TimerText.text = $"{seconds / 60 :00}:{seconds % 60 :00}";
        }

        private void OnRestartButtonClicked()
        {
            _gameStarter.StartGame();
        }

        private void OnStartGame()
        {
            _uiGameWindow.SetGameState();
        }

        private void OnEndGame(bool isWin)
        {
            if (isWin)
            {
                _uiGameWindow.SetWinState();
            }
            else
            {
                _uiGameWindow.SetLoseState();
            }
        }
    }
}
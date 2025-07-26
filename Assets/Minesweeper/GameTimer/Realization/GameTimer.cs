using System;
using Minesweeper.GameTimer.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;

namespace Minesweeper.GameTimer.Realization
{
    public class GameTimer : IGameTimer, IDisposable, ITickable
    {
        public ReactiveProperty<int> TotalSeconds { get; private set; } = new ReactiveProperty<int>();

        private float _timer;

        private bool _isRunning;

        public void Dispose()
        {
            TotalSeconds.Dispose();
            TotalSeconds = null;
        }
        
        public void Tick()
        {
            if (!_isRunning)
            {
                return;
            }
            
            _timer += Time.deltaTime;

            if (_timer >= 1)
            {
                _timer -= 1;
                TotalSeconds.Value++;
            }
        }
        
        public void Start()
        {
            _timer = 0;
            TotalSeconds.Value = 0;
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
        }
    }
}
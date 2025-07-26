using System;
using UniRx;

namespace Minesweeper.MineField.Interfaces
{
    public interface IMineFieldController
    {
        event Action<bool> OnEndGameEvent;
        ReactiveProperty<int> MinesLeftFromFlags { get; }
        void Start();
    }
}
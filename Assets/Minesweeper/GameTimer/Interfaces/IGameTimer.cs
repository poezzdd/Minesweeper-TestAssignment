using UniRx;

namespace Minesweeper.GameTimer.Interfaces
{
    public interface IGameTimer
    {
        ReactiveProperty<int> TotalSeconds { get; }
        void Start();
        void Stop();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper.MineField.Realization
{
    [CreateAssetMenu(fileName = "MineFieldConfig", menuName = "Configs/MineFieldConfig")]
    public class MineFieldConfig : ScriptableObject
    {
        public Vector2Int GridSize => gridSize;
        public int MinesCount => minesCount;
        
        [SerializeField] private Vector2Int gridSize; 
        [SerializeField] private int minesCount;
        
        [SerializeField] private NeighborCountColorData[] neighborCountColors;

        private readonly Dictionary<int, Color> _colorsStorage = new Dictionary<int, Color>();

        public Color GetNeighborsCountColor(int neighborsCount)
        {
            if (_colorsStorage.Count == 0)
            {
                Init();
            }

            return _colorsStorage.GetValueOrDefault(neighborsCount);
        }

        private void Init()
        {
            foreach (var data in neighborCountColors)
            {
                _colorsStorage.Add(data.NeighborCount, data.CountTextColor);
            }
        }
    }

    [Serializable]
    public struct NeighborCountColorData
    {
        public int NeighborCount;
        public Color CountTextColor;
    }
}
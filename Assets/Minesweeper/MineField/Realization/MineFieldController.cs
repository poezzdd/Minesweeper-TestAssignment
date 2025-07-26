using System;
using System.Collections.Generic;
using Minesweeper.Application;
using Minesweeper.MineField.Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Minesweeper.MineField.Realization
{
    public class MineFieldController : IMineFieldController, IDisposable
    {
        public event Action<bool> OnEndGameEvent;
        public ReactiveProperty<int> MinesLeftFromFlags { get; private set; } = new ReactiveProperty<int>();

        private MineFieldCellView[,] _grid;

        private Dictionary<MineFieldCellView, MineFieldCellData> _cellsDataStorage;

        private MineFieldConfig _mineFieldConfig;
        private MineFieldCellView.Pool _cellsPool;
        
        private Transform _fieldTransform;

        private bool _minesPlaced;
        private bool _blockInteractions;
        private int _openedCellsCount;

        private readonly int _cellsCount;
        private readonly Vector2Int _gridSize;
        
        public MineFieldController(
            MineFieldConfig mineFieldConfig,
            GameSceneData gameSceneData,
            MineFieldCellView.Pool cellsPool)
        {
            _mineFieldConfig = mineFieldConfig;
            _cellsPool = cellsPool;
            _fieldTransform = gameSceneData.FieldTransform;
            _gridSize = _mineFieldConfig.GridSize;

            _cellsCount = _gridSize.x * _gridSize.y;
            _grid = new MineFieldCellView[_gridSize.x, _gridSize.y];
            _cellsDataStorage = new Dictionary<MineFieldCellView, MineFieldCellData>(_cellsCount);
            
            CreateGrid();
        }

        public void Dispose()
        {
            OnEndGameEvent = null;

            foreach (var dataKVP in _cellsDataStorage)
            {
                dataKVP.Key.OnClickEvent -= OnClickedOnCell;
            }
            
            _cellsDataStorage.Clear();
            _cellsDataStorage = null;
            
            Array.Clear(_grid, 0, _grid.Length);
            _grid = null;
            
            MinesLeftFromFlags.Dispose();
            MinesLeftFromFlags = null;
            
            _cellsPool.Clear();
            _cellsPool = null;

            _mineFieldConfig = null;
            _fieldTransform = null;
        }

        public void Start()
        {
            MinesLeftFromFlags.Value = _mineFieldConfig.MinesCount;

            _openedCellsCount = 0;
            _minesPlaced = false;
            _blockInteractions = false;

            foreach (var cellKVP in _cellsDataStorage)
            {
                cellKVP.Key.SetDefaultState();
                cellKVP.Value.Clear();
            }
        }

        private void CreateGrid()
        {
            var cellSize = Mathf.Min(
                _fieldTransform.localScale.x / _gridSize.x,
                _fieldTransform.localScale.y / _gridSize.y);
            
            var cellScale = new Vector3(cellSize, cellSize, cellSize);
            
            var fieldPos = _fieldTransform.position;
            
            var startPos = fieldPos - new Vector3(
                cellSize * _gridSize.x / 2, 
                cellSize * _gridSize.y / 2, 0f);
            
            var indentVector = Vector3.zero;

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    var cell = _cellsPool.Spawn();

                    indentVector.x = (x + 0.5f) * cellSize;
                    indentVector.y = (y + 0.5f) * cellSize;

                    cell.transform.position = startPos + indentVector;
                    
                    cell.transform.localScale = cellScale;
                    
                    _grid[x, y] = cell;
                    _cellsDataStorage.Add(cell, new MineFieldCellData(new Vector2Int(x, y)));
                    
                    cell.SetDefaultState();

                    cell.OnClickEvent += OnClickedOnCell;
                }
            }
        }

        private void OnClickedOnCell(MineFieldCellView cell, PointerEventData.InputButton button)
        {
            if (_blockInteractions || _cellsDataStorage[cell].IsOpened)
            {
                return;
            }

            switch (button)
            {
                case PointerEventData.InputButton.Left:
                    OpenCell(cell);
                    break;
                case PointerEventData.InputButton.Right:
                    _cellsDataStorage[cell].HasFlag = !_cellsDataStorage[cell].HasFlag;
                    if (_cellsDataStorage[cell].HasFlag)
                    {
                        MinesLeftFromFlags.Value--;
                        cell.SetFlagState();
                    }
                    else
                    {
                        MinesLeftFromFlags.Value++;
                        cell.SetDefaultState();
                    }
                    break;
            }
        }

        private void OpenCell(MineFieldCellView cell)
        {
            if (_cellsDataStorage[cell].HasMine)
            {
                cell.SetMineState(true);

                foreach (var cellKVP in _cellsDataStorage)
                {
                    if (cellKVP.Value.HasMine && cellKVP.Key != cell)
                    {
                        cellKVP.Key.SetMineState(false);
                    }
                }
                
                EndGame(false);
                return;
            }
            
            _cellsDataStorage[cell].IsOpened = true;

            if (!_minesPlaced)
            {
                PlaceMines();
                _minesPlaced = true;
            }
            
            OpenCellsArea(cell);

            if (_openedCellsCount == _cellsCount - _mineFieldConfig.MinesCount)
            {
                EndGame(true);
            }
        }

        private void OpenCellsArea(MineFieldCellView cell)
        {
            var queueToCheck = new Queue<Vector2Int>();
            
            queueToCheck.Enqueue(_cellsDataStorage[cell].GridPosition);

            while (queueToCheck.Count > 0)
            {
                var pos = queueToCheck.Dequeue();

                var nb = _grid[pos.x, pos.y];
                var data = _cellsDataStorage[nb];

                if (pos.x < 0 || pos.x >= _gridSize.x || pos.y < 0 || pos.y >= _gridSize.y)
                {
                    continue;
                }

                if ((data.IsOpened && nb != cell) || data.HasMine)
                {
                    continue;
                }
                
                data.IsOpened = true;
                _openedCellsCount++;

                if (data.NeighborMinesCount > 0)
                {
                    nb.SetNeighboursState(
                        data.NeighborMinesCount,
                        _mineFieldConfig.GetNeighborsCountColor(data.NeighborMinesCount));
                    
                    continue;
                }
                
                nb.SetEmptyState();
                
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }
                        
                        var nx = pos.x + dx;
                        var ny = pos.y + dy;
                        
                        if (nx >= 0 && nx < _gridSize.x && ny >= 0 && ny < _gridSize.y)
                        {
                            if (!_cellsDataStorage[_grid[nx, ny]].IsOpened)
                            {
                                queueToCheck.Enqueue(new Vector2Int(nx, ny));
                            }
                        }
                    }
                }
            }
        }

        private void EndGame(bool isWin)
        {
            _blockInteractions = true;
            OnEndGameEvent?.Invoke(isWin);
        }

        private void PlaceMines()
        {
            if (_mineFieldConfig.MinesCount >= _cellsCount)
            {
                Debug.LogError("Mines count is too big");
                return;
            }

            var cells = new List<Vector2Int>(_cellsCount);

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    if (!_cellsDataStorage[_grid[x, y]].IsOpened)
                    {
                        cells.Add(_cellsDataStorage[_grid[x, y]].GridPosition);
                    }
                }
            }

            for (int i = 0; i < cells.Count; i++)
            {
                var mixIndex = Random.Range(i, cells.Count);
                (cells[i], cells[mixIndex]) = (cells[mixIndex], cells[i]);
            }

            for (int i = 0; i < MinesLeftFromFlags.Value; i++)
            {
                _cellsDataStorage[_grid[cells[i].x, cells[i].y]].HasMine = true;
            }
            
            CalculateNeighborsMinesCount();
        }

        private void CalculateNeighborsMinesCount()
        {
            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    if (_cellsDataStorage[_grid[x, y]].HasMine)
                    {
                        continue;
                    }

                    var minesCount = 0;
                    
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            var nx = x + dx;
                            var ny = y + dy;
                            
                            if (nx >= 0 && nx < _gridSize.x && ny >= 0 && ny < _gridSize.y)
                            {
                                if (_cellsDataStorage[_grid[nx, ny]].HasMine)
                                {
                                    minesCount++;
                                }
                            }
                        }
                    }

                    _cellsDataStorage[_grid[x, y]].NeighborMinesCount = minesCount;
                }
            }
        }
    }

    public class MineFieldCellData
    {
        public bool IsOpened;
        public bool HasMine;
        public bool HasFlag;
        public int NeighborMinesCount;
        
        public readonly Vector2Int GridPosition;

        public MineFieldCellData(
            Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
        }

        public void Clear()
        {
            IsOpened = false;
            HasMine = false;
            HasFlag = false;
            NeighborMinesCount = 0;
        }
    }
}
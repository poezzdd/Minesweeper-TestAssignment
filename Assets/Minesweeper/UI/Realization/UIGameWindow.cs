using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.UI.Realization
{
    public class UIGameWindow : MonoBehaviour
    {
        public event Action OnRestartButtonClickedEvent;
        
        public TextMeshProUGUI TimerText => timerText;
        public TextMeshProUGUI MinesLeftText => minesLeftText;
        
        [SerializeField] private Button restartButton;
        
        [SerializeField] private GameObject winText;
        [SerializeField] private GameObject loseText;
        
        [SerializeField] private GameObject gameSmile;
        [SerializeField] private GameObject winSmile;
        [SerializeField] private GameObject loseSmile;
        
        [SerializeField] private TextMeshProUGUI timerText;
        
        [SerializeField] private TextMeshProUGUI minesLeftText;

        public void SetWinState()
        {
            ClearStates();
            
            winSmile.SetActive(true);
            winText.SetActive(true);
        }
        
        public void SetLoseState()
        {
            ClearStates();
            
            loseSmile.SetActive(true);
            loseText.SetActive(true);
        }

        public void SetGameState()
        {
            ClearStates();
            
            gameSmile.SetActive(true);
        }

        private void ClearStates()
        {
            gameSmile.SetActive(false);
            winSmile.SetActive(false);
            loseSmile.SetActive(false);
            
            winText.SetActive(false);
            loseText.SetActive(false);
        }

        private void Start()
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        private void OnRestartButtonClicked()
        {
            OnRestartButtonClickedEvent?.Invoke();
        }
    }
}
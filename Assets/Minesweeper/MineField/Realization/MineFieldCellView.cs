using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minesweeper.MineField.Realization
{
    public class MineFieldCellView : MonoBehaviour, IPointerClickHandler
    {
        public event Action<MineFieldCellView, PointerEventData.InputButton> OnClickEvent;
        
        [SerializeField] private GameObject mineObject;
        [SerializeField] private GameObject flagObject;
        [SerializeField] private TextMeshPro neighborMinesText;
        [SerializeField] private GameObject closedImageObject;
        [SerializeField] private GameObject loseBackground;

        public void SetMineState(bool showLoseBackground)
        {
            SetEmptyState();
            
            closedImageObject.SetActive(false);
            mineObject.SetActive(true);
            loseBackground.SetActive(showLoseBackground);
        }

        public void SetFlagState()
        {
            SetEmptyState();
            
            closedImageObject.SetActive(true);
            flagObject.SetActive(true);
        }

        public void SetNeighboursState(int neighboursCount, Color textColor)
        {
            SetEmptyState();
            
            closedImageObject.SetActive(false);
            neighborMinesText.gameObject.SetActive(true);
            neighborMinesText.text = neighboursCount.ToString();
            neighborMinesText.color = textColor;
        }

        public void SetDefaultState()
        {
            SetEmptyState();
            
            closedImageObject.SetActive(true);
        }
        
        public void SetEmptyState()
        {
            mineObject.SetActive(false);
            flagObject.SetActive(false);
            closedImageObject.SetActive(false);
            loseBackground.SetActive(false);
            neighborMinesText.gameObject.SetActive(false);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent?.Invoke(this, eventData.button);
        }
        
        public class Pool : MonoMemoryPool<MineFieldCellView> { }
    }
}
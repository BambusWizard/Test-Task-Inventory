using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackpackUI : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private List<Slot> slots;
    
    [Header("References")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemWeight;
    [SerializeField] private BackpackController backpackController;
    private int lastSelectedSlot;
    private Tween tweenUI;

    [Serializable]
    public class Slot
    {
        public int slotID;
        public GameObject slot;
        public CanvasGroup canvasGroup;
    }
    
    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;

    private void Start()
    {
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();
    }
    
    private void Update()
    {
        transform.forward = Camera.main.transform.forward;

        SlotRaycast();
    }

    public void UpdateActiveSlots(List<int> activeSlots)
    {
        foreach (var slot in slots)
        {
            var isSlotActive = activeSlots.Contains(slot.slotID);
            
            slot.canvasGroup.alpha = isSlotActive ? 1f : 0.5f;
        }
    }
    
    public void SetVisibility(bool isActive)
    {
        // Если Canvas и так выключен, то нам не нужно показывать анимацию выключения
        if (!mainCanvas.enabled && !isActive)
            return;
        
        // Поворачиваем Canvas заранее к игроку
        transform.forward = Camera.main.transform.forward;
        
        // Задаем начальные и конечные значения основываясь на том, какая анимация должна быть (Вкл или Выкл)
        canvasGroup.alpha = isActive ? 0f : 1f;
        var alpha = isActive ? 1f : 0f;
        var endActive = isActive;
        
        tweenUI.Kill();
        tweenUI = DOTween.Sequence()
            .AppendCallback(() => SetCanvasVisibility(true))
            .Append(canvasGroup.DOFade(alpha, 0.2f))
            .AppendCallback(() => SetCanvasVisibility(endActive))
            .Play();
    }

    private void SetCanvasVisibility(bool isActive)
    {
        mainCanvas.enabled = isActive;
        enabled = isActive;
    }
    
    private void SlotRaycast()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        var isSlotFound = false;
        foreach (var result in results)
        {
            var currentSlot = slots.Find(x => x.slot == result.gameObject);

            if (currentSlot != null)
            {
                isSlotFound = true;
                UpdateUI(currentSlot.slotID);
            }
        }

        if (!isSlotFound)
        {
            lastSelectedSlot = -1;
            UpdateEmptyUI();
        }
        
        if (Input.GetMouseButtonUp(0) && isSlotFound)
        {
            var item = backpackController.GetItem(lastSelectedSlot);

            if (item != null)
            {
                backpackController.RemoveItem(item.type);
                SetVisibility(false);
            }
        }
    }
    
    private void UpdateUI(int slotIndex)
    {
        if (slotIndex == lastSelectedSlot)
            return;
        
        lastSelectedSlot = slotIndex;
        var itemData = backpackController.GetItem(slotIndex);
        if (itemData == null)
        {
            UpdateEmptyUI();
            return;
        }
        
        itemName.text = $"\"{itemData.itemName}\"";
        itemWeight.text = $"Weight: {itemData.weight}";
    }
    
    private void UpdateEmptyUI()
    {
        itemName.text = "Select Item";
        itemWeight.text = string.Empty;
    }
}

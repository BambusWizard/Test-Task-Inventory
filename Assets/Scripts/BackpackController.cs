using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EventBusSystem;
using UnityEngine;
using UnityEngine.Events;

public class BackpackController : MonoBehaviour
{
    [Header("Main")]    
    [SerializeField] private List<Slot> slots = new ();
    
    [Header("References")]
    [SerializeField] private BackpackUI backpackUI;
    
    [Header("Events")]
    [SerializeField] private UnityEvent<GameObject> OnItemAdd;
    [SerializeField] private UnityEvent<GameObject> OnItemRemove;
    
    [Serializable]
    public class Slot
    {
        public int slotID;
        public ItemType type;
        public Transform attachmentPoint;
        [HideInInspector] public bool isOccupied;
        [HideInInspector] public GameObject item;
        [HideInInspector] public ItemData itemData;
    }

    private void Start()
    {
        backpackUI.SetVisibility(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.CompareTag("Interactable"))
        {
            TryAddItem(other.gameObject);
        }
    }
    
    public void OpenUI(CachedItem item)
    {
        // Если в руке нет предмета - открываем UI рюкзака для взаимодействия с ним
        if (item == null)
        {
            var occupiedSlots = slots.Where(x => x.isOccupied).ToList().ConvertAll(x => x.slotID);
            backpackUI.UpdateActiveSlots(occupiedSlots);
            backpackUI.SetVisibility(true);
            return;
        }
        
        // Либо добавляем предмет в рюкзак
        TryAddItem(item.item);
    }
    
    public void CloseUI(CachedItem item)
    {
        backpackUI.SetVisibility(false);
    }

    private void TryAddItem(GameObject item)
    {
        item.TryGetComponent<ItemController>(out var itemController);
        
        if (itemController == null)
            return;
        
        if (!itemController.IsCanBeAttached())
            return;
        
        var itemType = itemController.GetItemData().type;
        var slot = slots.Find(x => x.type == itemType);
        
        if (slot.isOccupied)
            return;
        
        // Добавляем предмет, если все проверки на возможность добавления предмета в рюкзак пройдены
        item.transform.parent = slot.attachmentPoint;
        slot.isOccupied = true;
        slot.item = item;
        slot.itemData = itemController.GetItemData();
        itemController.BackpackIn();
        
        EventBus.RaiseEvent<IPlayerInventoryHandler>(h => h.OnItemRemoveFromHands(item));
        
        item.transform.DOMove(slot.attachmentPoint.position, 0.5f).SetEase(Ease.InSine);
        item.transform.DORotate(Vector3.up, 0.5f).SetEase(Ease.InSine);
        
        OnItemAdd.Invoke(item);
    }

    public ItemData GetItem(int slotID)
    {
        return slots.Find(x => x.slotID == slotID).itemData;
    }
    
    public void RemoveItem(ItemType itemType)
    {
        var slot = slots.Find(x => x.type == itemType);

        if (!slot.isOccupied)
            return;
        
        slot.item.GetComponent<ItemController>().BackpackOut();
        slot.item.transform.parent = null;
        slot.isOccupied = false;
        slot.itemData = null;
        
        EventBus.RaiseEvent<IPlayerInventoryHandler>(h => h.OnItemAddToHands(slot.item));
        
        OnItemRemove.Invoke(slot.item);
        
        slot.item = null;
    }
}

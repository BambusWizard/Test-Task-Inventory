using EventBusSystem;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private Rigidbody rigidbody;
    
    [Header("Colliders")]
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private Collider physicsCollider;
    private bool isCanBeInteracted = true;
    private bool isCanBeAttached;
    
    public bool IsCanBeAttached() => isCanBeAttached;
    
    public ItemData GetItemData() => itemData;
    
    // Тут описаны вспомогательные методы, помогающие изменить стейт предмета на нужный в определенном контексте
    #region Item State
    public void BackpackIn()
    {
        isCanBeInteracted = false;
        rigidbody.isKinematic = true;
    }

    public void BackpackOut()
    {
        isCanBeInteracted = true;
        rigidbody.isKinematic = false;
    }

    public void HandIn(bool isHoldingInHand)
    {
        isCanBeAttached = true;
        triggerCollider.enabled = !isHoldingInHand;

        if (isHoldingInHand)
        {
            physicsCollider.enabled = false;
        }
    }

    public void HandOut()
    {
        isCanBeAttached = false;
        triggerCollider.enabled = false;
        physicsCollider.enabled = true;
    }
    #endregion

    #region UnityEvents - InteractableObject
    public void OnItemDrag(CachedItem cachedItem)
    {
        if (!isCanBeInteracted)
            return;
        
        EventBus.RaiseEvent<IPlayerInventoryHandler>(h => h.OnItemDrag(gameObject));
    }
    
    public void OnItemUp(CachedItem cachedItem)
    {
        if (!isCanBeInteracted)
            return;
        
        EventBus.RaiseEvent<IPlayerInventoryHandler>(h => h.OnItemButtonUp(gameObject));
    }
    #endregion
}

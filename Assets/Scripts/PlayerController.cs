using System;
using EventBusSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerInventoryHandler
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private float itemFollowSpeed = 0.3f;
    
    [Header("Main")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform dragPoint;
    [SerializeField] private Transform handPoint;
    private HoldingState currentHoldingState;
    private CachedItem itemInHand;
    private bool isPressedOnSomething;
    private bool isDraggingSomething;
    private InteractableObject pressedObject;
    private GameObject pressedGameObject;

    [Serializable]
    public enum HoldingState
    {
        None,
        Holding,
        Dragging
    }

    private void Awake()
    {
        EventBus.Subscribe(this);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(this);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsLookingAtObjectWithTag("Interactable", out var hitInfo))
            {
                pressedGameObject = hitInfo.collider.gameObject;
                pressedObject = hitInfo.collider.GetComponent<InteractableObject>();
                pressedObject.onButtonDown.Invoke(itemInHand);
                isPressedOnSomething = true;
            }
            else
            {
                if (itemInHand != null)
                {
                    ReleaseItem(true);
                }
            }
        }
        
        // Если объект для взаимодействия был найден - ждем пока игрок сдвинет мышку
        // настолько, чтобы не смотреть на этот объект
        if (Input.GetMouseButton(0) && isPressedOnSomething && !isDraggingSomething)
        {
            if (!IsLookingAtObjectWithTag("Interactable", out var hitInfo) || (hitInfo.collider.gameObject && hitInfo.collider.gameObject != pressedGameObject))
            {
                pressedObject.onButtonDrag.Invoke(itemInHand);
                isDraggingSomething = true;
            }
        }
        
        // Когда закончили работу с объектом - вызываем событие
        if (Input.GetMouseButtonUp(0) && isPressedOnSomething)
        {
            isPressedOnSomething = false;
            isDraggingSomething = false;
            pressedObject.onButtonUp.Invoke(itemInHand);
            pressedObject = null;
            pressedGameObject = null;
        }
    }

    private void FixedUpdate()
    {
        if (currentHoldingState == HoldingState.Dragging)
        {
            itemInHand.rigidbody.velocity = (dragPoint.position - itemInHand.rigidbody.position) * (1 / Time.fixedDeltaTime * itemFollowSpeed);
        }

        if (currentHoldingState == HoldingState.Holding)
        {
            itemInHand.rigidbody.velocity = (handPoint.position - itemInHand.rigidbody.position) * (1 / Time.fixedDeltaTime * itemFollowSpeed);
            itemInHand.rigidbody.rotation = handPoint.rotation;
        }
    }
    
    // Raycast из камеры с проверкой тега объекта
    private bool IsLookingAtObjectWithTag(string tagToCompare, out RaycastHit raycastHit)
    {
        raycastHit = new RaycastHit();

        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out var hitInfo, interactionDistance) &&
            hitInfo.collider.CompareTag(tagToCompare))
        {
            raycastHit = hitInfo;
            return true;
        }

        return false;
    }

    #region IPlayerInventoryHandler
    // Событие, когда из рук игрока нужно достать предмет
    public void OnItemRemoveFromHands(GameObject item)
    {
        if (itemInHand != null && itemInHand.item == item)
        {
            ReleaseItem(false);
        }
    }

    // Событие, когда в руки игрока нужно добавить предмет
    public void OnItemAddToHands(GameObject item)
    {
        HoldItem(item, HoldingState.Holding, true);
    }
    
    // Событие когда игрок отпускает кнопку
    public void OnItemButtonUp(GameObject item)
    {
        switch (currentHoldingState)
        {
            case HoldingState.None:
            {
                HoldItem(item, HoldingState.Holding, true);
            }
                break;
            case HoldingState.Holding:
            {
                ReleaseItem(false);
            }
                break;
            case HoldingState.Dragging:
            {
                ReleaseItem(false);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    // Событие перетаскивания предмета
    public void OnItemDrag(GameObject item)
    {
        HoldItem(item, HoldingState.Dragging, false);
    }
    #endregion
    
    // Назначаем предмет в руку и обновляем статус у предмета
    private void HoldItem(GameObject item, HoldingState state, bool isInHands)
    {
        itemInHand = new CachedItem(item);
        currentHoldingState = state;
        itemInHand.itemController.HandIn(isInHands);
    }
    
    // Отпускаем предмет из руки (любое положение: в руке или перетаскивание)
    private void ReleaseItem(bool isWaitForDelay)
    {
        if (itemInHand == null)
            return;

        // Добавляем небольшой delay, чтобы игрок не мог выкинуть предмет из руки сразу же
        if (isWaitForDelay && Time.time < itemInHand.timer + 1f)
            return;
        
        itemInHand.itemController.HandOut();
        currentHoldingState = HoldingState.None;
        itemInHand = null;
    }
}
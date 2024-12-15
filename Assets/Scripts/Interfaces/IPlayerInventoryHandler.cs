using EventBusSystem;
using UnityEngine;

public interface IPlayerInventoryHandler : IGlobalSubscriber
{
    public void OnItemRemoveFromHands(GameObject item);
    public void OnItemAddToHands(GameObject item);
    public void OnItemButtonUp(GameObject item);
    public void OnItemDrag(GameObject item);
}
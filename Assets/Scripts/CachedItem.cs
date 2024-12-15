using UnityEngine;

public class CachedItem
{
    public GameObject item;
    public ItemController itemController;
    public Rigidbody rigidbody;
    public float timer;

    public CachedItem(GameObject obj)
    {
        item = obj;
        itemController = obj.GetComponent<ItemController>();
        rigidbody = obj.GetComponent<Rigidbody>();
        
        // Сохраняем время добавления предмета в руку (последнее взаимодействие)
        timer = Time.time;
    }
}
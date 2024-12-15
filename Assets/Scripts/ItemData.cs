using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "ScriptableObjects/Item Data", order = 1)]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    [Range(0f, 10f)]
    public float weight;
    public ItemType type;
}

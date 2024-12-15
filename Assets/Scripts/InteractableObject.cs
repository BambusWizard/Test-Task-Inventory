using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CachedItemEvent : UnityEvent<CachedItem> { }

public class InteractableObject : MonoBehaviour
{
    public CachedItemEvent onButtonDown;
    public CachedItemEvent onButtonDrag;
    public CachedItemEvent onButtonUp;
}

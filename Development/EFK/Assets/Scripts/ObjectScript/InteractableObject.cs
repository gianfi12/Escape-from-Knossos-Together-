using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected bool _hasBeenActivated;

    public bool HasBeenActivated
    {
        get => _hasBeenActivated;
        set => _hasBeenActivated = value;
    }

    public abstract void Interact(GameObject player);
    
}
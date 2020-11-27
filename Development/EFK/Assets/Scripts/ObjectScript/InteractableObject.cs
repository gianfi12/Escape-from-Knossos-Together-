using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    protected bool _hasBeenActivated;
    [SerializeField] private Transform topRight;

    public bool HasBeenActivated
    {
        get => _hasBeenActivated;
        set => _hasBeenActivated = value;
    }

    public abstract void Interact(GameObject player);

    public Transform GetTopRight()
    {
        return topRight;
    }
    
}
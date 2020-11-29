using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField] private string interactiveText;
    protected bool _hasBeenActivated;

    public bool HasBeenActivated
    {
        get => _hasBeenActivated;
        set => _hasBeenActivated = value;
    }

    public string InteractiveText {
        get => interactiveText;
    }

    public Vector3 GetTextPosition() {
        return GetComponent<SpriteRenderer>().bounds.max;
    }

    public abstract void Interact(GameObject player);

}
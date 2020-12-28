using System;
using UnityEngine;

public class Doors : MonoBehaviour
{
    // Is the direction in which the player has to traverse the door in order for it to become closed
    [SerializeField] private Direction closingDirection;
    [SerializeField] private bool IsOpenOnStart;
    
    private Animator _animator;
    private PolygonCollider2D _polygonCollider2D;
    private SpriteRenderer _spriteRenderer;

    public Direction ClosingDirection {
        get => closingDirection;
        set => closingDirection = value;
    }
    private void Awake() {
        _animator = gameObject.GetComponent<Animator>();
        _polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (IsOpenOnStart) 
            OpenDoors();
        else
            CloseDoors();
    }

    private void OnTriggerExit2D(Collider2D other) {
        bool valueCondition = false;
        switch (closingDirection) {
            case Direction.East:
                valueCondition = other.gameObject.transform.position.x > transform.position.x;
                break;
            case Direction.West:
                valueCondition = other.gameObject.transform.position.x < transform.position.x;
                break;
            case Direction.North:
                valueCondition = other.gameObject.transform.position.y > transform.position.y;
                break;
            case Direction.South:
                valueCondition = other.gameObject.transform.position.y < transform.position.y;
                break;
        }

        if (other.CompareTag("Player") && valueCondition) {
            CloseDoors();
        }
    }

    public void OpenDoors() {
        _animator.SetBool("isOpen",true);
    }

    public void CloseDoors() {
        _animator.SetBool("isOpen",false);
    }

    public void FlipClosingDirection() {
        closingDirection = closingDirection.GetOpposite();
    }
    
    public void changeColliderShape()
    {
        bool isTrigger = _polygonCollider2D.isTrigger;
        Destroy(_polygonCollider2D);
        _polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
        _polygonCollider2D.isTrigger = isTrigger;
    }

    public void setTrigger()
    {
        _polygonCollider2D.isTrigger = _polygonCollider2D.isTrigger ? false : true;
        gameObject.layer = _polygonCollider2D.isTrigger ? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");
    }
}
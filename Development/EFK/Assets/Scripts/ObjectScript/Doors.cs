using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    // Is the direction in which the player has to traverse the door in order for it to become closed
    [SerializeField] private Direction closingDirection;
    [SerializeField] private bool isOpenOnStart;

    public bool IsOpenOnStart { get => isOpenOnStart; }

    private Animator _animator;
    private PolygonCollider2D _polygonCollider2D;
    private SpriteRenderer _spriteRenderer;
    private bool _isOpen;

    public Direction ClosingDirection {
        get => closingDirection;
        set => closingDirection = value;
    }
    
    private void Awake() {
        _animator = gameObject.GetComponent<Animator>();
        _polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (isOpenOnStart) 
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

    public void OpenDoors()
    {
        _isOpen = true;
        GetComponent<Animator>().SetBool("isOpen",true);
        StartCoroutine("OnFinishAnimation");
    }
    
    IEnumerator OnFinishAnimation()
    {
        while(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;

        changeColliderShape();
        setTrigger();
    }

    public void CloseDoors()
    {
        _isOpen = false;
        _animator.SetBool("isOpen",false);
        StartCoroutine("OnFinishAnimation");
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
        _polygonCollider2D.isTrigger = !_isOpen ? false : true;
        gameObject.layer = 
            _isOpen ? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");
    }
}
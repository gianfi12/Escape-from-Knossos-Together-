﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    // Is the direction in which the player has to traverse the door in order for it to become closed
    [SerializeField] private Direction closingDirection;
    [SerializeField] private bool isOpenOnStart;
    [SerializeField] private AudioSource openingSound;
    [SerializeField] private AudioSource closingSound;

    public bool IsOpenOnStart { get => isOpenOnStart; }

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isOpen;

    public Direction ClosingDirection {
        get => closingDirection;
        set => closingDirection = value;
    }
    
    private void Awake() {
        _animator = gameObject.GetComponent<Animator>();
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
        GetComponent<Collider2D>().isTrigger = true;
        _isOpen = true;
        GetComponent<Animator>().SetBool("isOpen",true);
        //openingSound.Play();
        // StartCoroutine("OnFinishAnimation");
    }
    /*
    IEnumerator OnFinishAnimation()
    {
        while(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;

        changeColliderShape();
        setTrigger();
    }*/

    public void CloseDoors()
    {
        GetComponent<Collider2D>().isTrigger = false;
        _isOpen = false;
        _animator.SetBool("isOpen",false);
        //closingSound.Play();
        //StartCoroutine("OnFinishAnimation");
    }

    public void FlipClosingDirection() {
        closingDirection = closingDirection.GetOpposite();
    }
    /*
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
    }*/
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    // Is the direction in which the player has to traverse the door in order for it to become closed
    [SerializeField] private Direction closingDirection;
    [SerializeField] private bool isOpenOnStart;
    private AudioSource[] doorSounds;

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
        doorSounds = GetComponentsInChildren<AudioSource>();
        if (isOpenOnStart)
            OpenDoors(false);
        else
            CloseDoors(false);
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
            
            CloseDoors(true);
            
        }
    }

    public IEnumerator OpenDoorsWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OpenDoors(true);
    }

    public void OpenDoors(bool withSound)
    {
        GetComponent<Collider2D>().isTrigger = true;
        _isOpen = true;
        GetComponent<Animator>().SetBool("isOpen",true);
        if (withSound)
        {
            System.Random random = new System.Random();
            doorSounds[random.Next(0,doorSounds.Length)].Play();
        }
    }

    public void CloseDoors(bool withSound)
    {
        GetComponent<Collider2D>().isTrigger = false;
        _isOpen = false;
        _animator.SetBool("isOpen",false);
        if (withSound)
        {
            System.Random random = new System.Random();
            doorSounds[random.Next(0,doorSounds.Length)].Play();
        }
    }

    public void FlipClosingDirection() {
        closingDirection = closingDirection.GetOpposite();
    }
}
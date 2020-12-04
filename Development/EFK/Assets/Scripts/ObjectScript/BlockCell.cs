﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCell : MonoBehaviour
{
    private bool isWalkable;

    public bool IsWalkable => isWalkable;

    public void setWalkable(bool walkable) { isWalkable = walkable; }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerFeet")) {
            Debug.Log("ENTERED CELL, WALKABLE: " + isWalkable);
        } 
    }
}
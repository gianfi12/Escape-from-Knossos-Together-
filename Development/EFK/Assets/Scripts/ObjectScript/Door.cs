using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Is the direction in which the player has to traverse the door in order for it to become closed
    [SerializeField] private Direction closingDirection;

    private void Awake()
    {
        foreach (Door2Script door2Script in transform.GetComponentsInChildren<Door2Script>())
        {
            door2Script.Direction = closingDirection;
        }
    }
}
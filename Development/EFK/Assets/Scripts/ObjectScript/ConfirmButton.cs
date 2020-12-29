using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmButton : InteractableObject
{
    private MemoryRoomManager memoryRoomManager;

    void Start()
    {
        memoryRoomManager = GetComponentInParent<MemoryRoomManager>();
    }

    public override void Interact(GameObject player)
    {
        memoryRoomManager.VerifyCombination();
    }
}

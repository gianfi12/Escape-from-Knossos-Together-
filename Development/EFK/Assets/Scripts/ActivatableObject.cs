using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Objects that get activated by Room Collider when the player enters the room
public abstract class ActivatableObject : MonoBehaviour
{
    public abstract void ActivateObject();
    public abstract void DeactivateObject();
}

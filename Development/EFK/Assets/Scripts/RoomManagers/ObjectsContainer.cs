using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsContainer : MonoBehaviour
{
    public int Seed;
    [SerializeField] private Doors entranceDoor;
    [SerializeField] private Doors exitDoor;

    public Doors EntranceDoor { get => entranceDoor;}
    public Doors ExitDoor { get => exitDoor; }

    public void FlipDoors() {
        Doors d = entranceDoor;

        entranceDoor = exitDoor;
        exitDoor = d;

        entranceDoor.OpenDoors(false);
        if(!entranceDoor.IsOpenOnStart) exitDoor.CloseDoors(false); // now entranceDoor is the old exitDoor 

        entranceDoor.FlipClosingDirection();
        exitDoor.FlipClosingDirection();
    }
}

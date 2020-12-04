using System;
using UnityEngine;

public class Doors : MonoBehaviour
{
    // Is the direction in which the player has to traverse the door in order for it to become closed
    [SerializeField] private Direction closingDirection;
    [SerializeField] private bool IsOpenOnStart;

    public Direction ClosingDirection {
        get => closingDirection;
        set => closingDirection = value;
    }
    private void Awake() {
        if (IsOpenOnStart) OpenDoors();
        else CloseDoors();
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
        for (int i = 0; i < transform.childCount; i++) {
            SingleDoor door = transform.GetChild(i).GetComponent<SingleDoor>();
            if (door != null) door.Open();
        }
        transform.GetComponent<Collider2D>().isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void CloseDoors() {
        for (int i = 0; i < transform.childCount; i++) {
            SingleDoor door = transform.GetChild(i).GetComponent<SingleDoor>();
            if (door != null) door.Close();
        }
        transform.GetComponent<Collider2D>().isTrigger = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void FlipClosingDirection() {
        closingDirection = closingDirection.GetOpposite();
    }
}
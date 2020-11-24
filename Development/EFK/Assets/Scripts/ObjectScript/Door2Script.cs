using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Door2Script : MonoBehaviour
{
    private Direction _direction;
    private void OnTriggerExit2D(Collider2D other)
    {
        bool valueCondition=false;
        switch (_direction)
        {
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

        if (other.CompareTag("Player") && valueCondition)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SingleDoor>().Close();
            }

            transform.GetComponent<Collider2D>().isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public Direction Direction
    {
        get => _direction;
        set => _direction = value;
    }
}

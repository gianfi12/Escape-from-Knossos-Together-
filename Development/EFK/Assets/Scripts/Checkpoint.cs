using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] ExitTrigger exitTrigger;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            exitTrigger.CheckpointReached(this);
        }
    }

 }

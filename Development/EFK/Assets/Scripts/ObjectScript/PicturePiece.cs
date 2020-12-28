using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicturePiece : MonoBehaviour
{
    private bool active;
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerFeet")) {
            if (!active)
            {
                active = true;
                GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                active = false;
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        } 
    }
}

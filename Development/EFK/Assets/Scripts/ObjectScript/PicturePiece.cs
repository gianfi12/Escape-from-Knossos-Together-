using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PicturePiece : MonoBehaviour
{
    private bool active;
    private int pictureID;

    public int PictureID
    {
        get => pictureID;
        set => pictureID = value;
    }

    public bool Active
    {
        get => active;
        set => active = value;
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
                ResetPiece();
            }
            if (other.GetComponent<PhotonView>().IsMine) FindObjectOfType<AudioManager>().Play("MemoryPressed");
        } 
    }

    public void ResetPiece()
    {
        active = false;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}

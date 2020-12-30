using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCell : MonoBehaviour
{
    [SerializeField] Color walkableColor;
    [SerializeField] Color notWalkableColor;

    private bool isWalkable;

    public bool IsWalkable => isWalkable;


    public void setWalkable(bool walkable) { 
        isWalkable = walkable;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color currColor = walkable ? walkableColor : notWalkableColor;
        spriteRenderer.color = currColor;
        spriteRenderer.material.SetColor("GlowColor", currColor);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerFeet")) {
            GetComponent<SpriteRenderer>().enabled = true;
            if (!isWalkable) other.gameObject.GetComponentInParent<PlayerControllerMap>().Explode();
        } 
    }
}

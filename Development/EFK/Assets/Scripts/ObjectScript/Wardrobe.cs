
using UnityEngine;

public class Wardrobe : InteractableObject
{
    public override void Interact(GameObject player)
    {
        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        if(playerInput.CanMove)
        {
            playerInput.CanMove = false;
            player.SetActive(false);
        }
        else
        {
            playerInput.CanMove = true;
            player.SetActive(true);
        }
    }
}
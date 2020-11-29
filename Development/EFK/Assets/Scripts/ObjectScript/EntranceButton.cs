using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceButton : InteractableObject {

    private EntrancePanel panel;

    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponentInParent<EntrancePanel>();
    }

    public override void Interact(GameObject player) {
        panel.ButtonPressed(transform.GetSiblingIndex());
    }

}

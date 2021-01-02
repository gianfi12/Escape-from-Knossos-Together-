using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeText : ActivatableObject
{
    public override void ActivateObject()
    {
        gameObject.SetActive(true);
    }

    public override void DeactivateObject()
    {
        gameObject.SetActive(false);
    }
}

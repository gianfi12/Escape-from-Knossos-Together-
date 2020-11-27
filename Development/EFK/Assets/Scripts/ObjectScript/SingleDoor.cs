using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDoor : MonoBehaviour {
    [SerializeField] private float rotation;
    private bool isOpen=true;

    public void Close()
    {
        if(isOpen)
        {
            transform.rotation *= Quaternion.Euler(0, 0, rotation);
            isOpen = false;
        }
    }

    public void Open()
    {
        if (!isOpen)
        {
            transform.rotation *= Quaternion.Euler(0, 0, -rotation);
            isOpen = true;
        }
    }
}

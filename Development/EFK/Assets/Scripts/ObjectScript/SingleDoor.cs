using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleDoor : MonoBehaviour
{
    [SerializeField] private float rotation;
    public void Close()
    {
        transform.rotation *= Quaternion.Euler(0,0,rotation);
    }
}

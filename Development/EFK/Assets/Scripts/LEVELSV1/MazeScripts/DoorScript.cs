using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private Transform _hinge;
    private bool _triggered=false;
    private Quaternion _prev;
    private void OnTriggerEnter2D(Collider2D other)
    {
        _prev = _hinge.transform.rotation;
        _hinge.transform.rotation = Quaternion.Euler(0, 0f, -90f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _hinge.transform.rotation = _prev;
    }
}

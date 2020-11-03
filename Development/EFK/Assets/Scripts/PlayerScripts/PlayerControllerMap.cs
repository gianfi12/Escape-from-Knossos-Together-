using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMap : MonoBehaviour
{
    private float _speed = 3f;
    private Vector3 _movement;
    
    public void SetLocation(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void Move(Vector3 movement)
    {
        _movement = movement;
    }

    private void FixedUpdate()
    {
        transform.position += _movement.normalized * Time.fixedDeltaTime * _speed;
        _movement=Vector3.zero;
    }
}

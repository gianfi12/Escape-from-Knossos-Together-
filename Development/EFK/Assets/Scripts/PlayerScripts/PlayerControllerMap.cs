using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMap : MonoBehaviour
{
    private float _speed = 3f;
    private Vector3 _movement;
    private bool _hasChange = false;
    
    public void SetLocation(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void Move(Vector3 movement)
    {
        _movement = movement;
        _hasChange = true;
    }

    private void FixedUpdate()
    {
        if (_hasChange)
        {
            transform.position += _movement.normalized * Time.fixedDeltaTime * _speed;
            _hasChange = false;
        }
    }

    public Vector3 Movement => _movement.normalized;
}

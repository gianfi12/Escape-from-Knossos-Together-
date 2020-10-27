using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController _playerController;
    private float _horizontal;
    private float _vertical;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }
    private void FixedUpdate()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _playerController.Move(new Vector3(_horizontal,_vertical,0f));
    }
}

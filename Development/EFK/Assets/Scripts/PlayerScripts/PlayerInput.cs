using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControllerMap))]
public class PlayerInput : MonoBehaviour
{
    private PlayerControllerMap _playerController;
    private float _horizontal;
    private float _vertical;

    private void Awake()
    {
        _playerController = GetComponent<PlayerControllerMap>();
    }
    private void FixedUpdate()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _playerController.Move(new Vector3(_horizontal,_vertical,0f));
    }
}

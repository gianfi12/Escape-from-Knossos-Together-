using System;
using UnityEngine;


public class InputMenuManagerScript:MonoBehaviour
{
    [SerializeField] private GameObject exitGamePrefab;
    private GameObject _exitGamePrefab;
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (_exitGamePrefab == null)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                _exitGamePrefab = Instantiate(exitGamePrefab);
            }
            else
            {
                Destroy(_exitGamePrefab);
            }
        }
    }
}
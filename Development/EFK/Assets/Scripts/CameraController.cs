using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Transform _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player").transform;
        gameObject.transform.SetParent(_player);
        gameObject.transform.position = _player.transform.position+new Vector3(0f,0f,-1f);
    }
}

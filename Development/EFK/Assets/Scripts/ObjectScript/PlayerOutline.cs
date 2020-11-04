using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerOutline: MonoBehaviour
{
    [SerializeField] private Shader shader;
    [SerializeField] private Texture2D sprite;

    private void Start()
    {
        Renderer render = GetComponent<Renderer>();
        render.material.SetTexture("_MainTex",sprite);
    }
}

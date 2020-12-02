using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile
{
    private TileBase _tileBase;
    private Vector3Int _coordinates;
    private Vector3Int _normalizedCoordinates;
    private Color _color;
    private bool _hasColor;

    public Tile(TileBase tileBase, Vector3Int coordinates)
    {
        _tileBase = tileBase;
        _coordinates = coordinates;
    }

    public Vector3Int NormalizedCoordinates
    {
        get => _normalizedCoordinates;
        set => _normalizedCoordinates = value;
    }

    public TileBase TileBase
    {
        get => _tileBase;
    }

    public Vector3Int Coordinates
    {
        get => _coordinates;
        set => _coordinates = value;
    }
    
    public Color Color
    {
        get => _color;
        set {
            _color = value;
            _hasColor = true;
        }
    }

    public bool HasColor()
    {
        return _hasColor;
    }
}

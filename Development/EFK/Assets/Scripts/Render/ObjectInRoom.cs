using UnityEngine;

public class ObjectInRoom
{
    private Vector3Int _coordinates;
    private Transform _transform;

    public ObjectInRoom(Vector3Int coordinates, Transform gameObject)
    {
        _coordinates = coordinates;
        _transform = gameObject;
    }

    public Vector3Int Coordinates
    {
        get => _coordinates;
        set => _coordinates = value;
    }

    public Transform ObjectTransform => _transform;
}

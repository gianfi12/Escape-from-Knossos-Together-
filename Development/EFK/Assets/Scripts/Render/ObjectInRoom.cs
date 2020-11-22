using UnityEngine;

public class ObjectInRoom
{
    private Vector3 _coordinates;
    private Transform _transform;

    public ObjectInRoom(Vector3 coordinates, Transform gameObject)
    {
        _coordinates = coordinates;
        _transform = gameObject;
    }

    public Vector3 Coordinates
    {
        get => _coordinates;
        set => _coordinates = value;
    }

    public Transform ObjectTransform => _transform;
}

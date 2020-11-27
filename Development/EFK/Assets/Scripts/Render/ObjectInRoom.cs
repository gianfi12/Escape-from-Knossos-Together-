using UnityEngine;

public class ObjectInRoom: MonoBehaviour
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

    public void resetAndPlaceObjectInRoom(Vector3 coordinates)
    {
        GameObject gameObject = Instantiate(_transform.gameObject);
        gameObject.name = _transform.gameObject.name;
        gameObject.transform.position = _coordinates;
        gameObject.SetActive(true);
        _transform = gameObject.transform;
    }
}

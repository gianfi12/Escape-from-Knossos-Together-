// using UnityEngine;
//
// public class ObjectInRoom: MonoBehaviour
// {
//     private Vector3 _coordinates;
//     private Transform _transform;
//
//     public ObjectInRoom(Vector3 coordinates, Transform gameObject)
//     {
//         _coordinates = coordinates;
//         _transform = gameObject;
//     }
//
//     public Vector3 Coordinates
//     {
//         get => _coordinates;
//         set => _coordinates = value;
//     }
//     
//     public void resetAndPlaceObjectInRoom(Vector3 coordinates)
//     {
//         _coordinates = coordinates;
//         GameObject gameObject = Instantiate(_transform.gameObject);
//         gameObject.name = _transform.gameObject.name;
//         gameObject.transform.position = _coordinates;
//         gameObject.SetActive(true);
//         _transform = gameObject.transform;
//     }
//
//     public Transform objectTransform
//     {
//         get => _transform;
//         set => _transform = value;
//     }
// }

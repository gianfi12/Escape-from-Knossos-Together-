using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomCollection",menuName="EFK/RoomCollection",order=1)]
public class RoomCollection : ScriptableObject
{
    
    [SerializeField] private List<Room> rooms = new List<Room>();
    [SerializeField] private Room startingRoom;
    [SerializeField] private Room endingRoom;

    public List<Room> Rooms
    {
        get => rooms;
    }

    public Room StartingRoom => startingRoom;

    public Room EndingRoom => endingRoom;
}

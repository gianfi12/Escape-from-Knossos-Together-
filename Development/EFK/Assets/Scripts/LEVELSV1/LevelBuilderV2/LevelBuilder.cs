using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder : LevelBuilderAbstract
{
    [SerializeField] private RoomList roomList;
    [SerializeField] private int numberOfRoom;
    private List<RoomAbstract> _selectedRooms = new List<RoomAbstract>();
    [SerializeField] private Tilemap floormap;

    public override void Generate()
    {
        if (numberOfRoom > roomList.Rooms.Count)
        {
            throw new InvalidDataException("Not enough Room.");
        }
        for (int i = 0; i < numberOfRoom; i++)
        {
            GameObject room = SelectRoom().Initialize();
            room.transform.parent = this.transform;
            room.transform.localPosition = this.transform.localPosition;
        }
    }

    private RoomAbstract SelectRoom()
    {
        int index = Random.Range(0, roomList.Rooms.Count);
        while (_selectedRooms.Contains(roomList.Rooms[index]))
        {
            index = index++ % roomList.Rooms.Count;
        }
        _selectedRooms.Add(roomList.Rooms[index]);
        return roomList.Rooms[index];
    }

    public override CellAbstract GetCell(IntVector2 coordinates)
    {
        throw new System.NotImplementedException();
    }

    public override IntVector2 RandomCoordinates()
    {
        throw new System.NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LevelMap : MonoBehaviour
{
    private Tilemap _tilemapFloor;
    private Tilemap _tilemapWall;
    private Tilemap _tilemapObject;
    private Grid _grid;
    [SerializeField] private RoomCollection roomCollection;
    [SerializeField] private int numberOfRoom;
    private List<Room> _selectedRooms = new List<Room>();
    private int positionedRoom = 0;
    
    public void CreateMap(){
        InstantiateMapElements();
        
        RoomGeneration();
        RoomPlacement();
        RoomConnect();
    }

    private void InstantiateMapElements()
    {
        var gridObject = new GameObject("Grid");
        _grid = gridObject.AddComponent<Grid>();
        
        var goFloor = new GameObject("TilemapFloor");
        goFloor.transform.SetParent(_grid.gameObject.transform);
        _tilemapFloor = goFloor.AddComponent<Tilemap>();
        goFloor.AddComponent<TilemapRenderer>();
        
        var goWall = new GameObject("TilemapWall");
        goWall.transform.SetParent(_grid.gameObject.transform);
        _tilemapWall = goWall.AddComponent<Tilemap>();
        goWall.AddComponent<TilemapRenderer>();
        goWall.AddComponent<TilemapCollider2D>();
        Rigidbody2D rigidbody2DWall = goWall.AddComponent<Rigidbody2D>();
        rigidbody2DWall.bodyType = RigidbodyType2D.Kinematic;  

        var goObject = new GameObject("TilemapObject");
        goObject.transform.SetParent(_grid.gameObject.transform);
        _tilemapObject = goObject.AddComponent<Tilemap>();
        goObject.AddComponent<TilemapRenderer>();
    }

    private void RoomConnect()
    {
        for (int i = 0; i < _selectedRooms.Count - 1; i++)
        {
            Connect(_selectedRooms[i],_selectedRooms[i+1]);
        }
    }

    private void Connect(Room exitRoom, Room entranceRoom)
    {
        if(exitRoom.Exit.Count!=entranceRoom.Entrance.Count)
            throw new InvalidDataException("The rooms has a different number of tile for entrance/exit!");//TODO if they have a different value, we can maintains a 3 tile structure?
        foreach (Tile tileExitOne in exitRoom.Exit)
        {
            foreach (Tile tileExitTwo in exitRoom.Exit)
            {
                if(tileExitOne!=tileExitTwo && tileExitOne.Coordinates.x!=tileExitTwo.Coordinates.x && tileExitOne.Coordinates.y!=tileExitTwo.Coordinates.y)
                    throw new InvalidDataException("The entrance/exit tile are not linearly disposed.");
            }
        }
        Direction startingDirection = DirectionExtensions.FindDirection(exitRoom.Exit[0], exitRoom.Exit[1],_tilemapFloor);
        Direction arrivalDirection = DirectionExtensions
            .FindDirection(entranceRoom.Entrance[0], entranceRoom.Entrance[1], _tilemapFloor).GetOpposite();
        
    }


    private void RoomPlacement()
    {
        foreach (Room room in _selectedRooms)  
        {
            Vector3Int coordinates;
            while (!FreeSpace(room, coordinates = RandomCoordinates())); //start placing the starting room
            room.PlaceRoom(_tilemapFloor,_tilemapWall,_tilemapObject,coordinates);
            positionedRoom++;
        }
    }

    private bool FreeSpace(Room room,Vector3Int coordinates)
    {
        IEnumerable<Vector3Int> coordinatesFirstRoom = room.TileList.Select(x => x.Coordinates);
        for (int i = 0; _selectedRooms[i] != room; i++)
        {
            IEnumerable<Vector3Int> coordinatesSecondRoom = _selectedRooms[i].TileList.Select(x => x.Coordinates);
            foreach (Vector3Int tileFirst in coordinatesFirstRoom)
            {
                foreach (Vector3Int tileSecond in coordinatesSecondRoom)
                {
                    if (Vector3Int.Distance(tileFirst, tileSecond) == 0) return false;
                }
            }

        }

        return true;
    }

    private Vector3Int RandomCoordinates()
    {
        int randomX = Random.Range(-(30*positionedRoom+15), -(30*(positionedRoom+1)+15));
        int randomY = Random.Range(0,50);
        return new Vector3Int(randomX,randomY,0);
    }


    private void RoomGeneration()
    {
        _selectedRooms.Add(roomCollection.StartingRoom);
        roomCollection.StartingRoom.Generate();
        if (numberOfRoom > roomCollection.Rooms.Count)
        {
            throw new InvalidDataException("Not enough Room.");
        }
        for (int i = 0; i < numberOfRoom; i++)
        {
            Room selectedRoom = SelectRoom();
            selectedRoom.Generate();
            _selectedRooms.Add(selectedRoom); 
            
        }
        _selectedRooms.Add(roomCollection.EndingRoom);
    }


    private Room SelectRoom()
    {
        int index = Random.Range(0, roomCollection.Rooms.Count);
        while (_selectedRooms.Contains(roomCollection.Rooms[index]))
        {
            index = index++ % roomCollection.Rooms.Count;
        }
        return roomCollection.Rooms[index];
    }

    public void PlacePlayer(PlayerControllerMap player)
    {
        player.SetLocation(_tilemapFloor.layoutGrid.CellToWorld(_selectedRooms[0].Spawn[0].Coordinates)+new Vector3Int(1,1,0));
    }
}

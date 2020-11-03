using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public const int PaddingRoom = 3;
    
    
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
        foreach (Tile tileExitOne in exitRoom.Exit)
        {
            foreach (Tile tileExitTwo in exitRoom.Exit)
            {
                if(tileExitOne!=tileExitTwo && tileExitOne.Coordinates.x!=tileExitTwo.Coordinates.x && tileExitOne.Coordinates.y!=tileExitTwo.Coordinates.y)
                    throw new InvalidDataException("The entrance/exit tile are not linearly disposed.");
            }
        }
        Direction startingDirection = DirectionExtensions.FindDirection(exitRoom.Exit[0], exitRoom.Exit[1],_tilemapFloor);
        Direction arrivalDirection = DirectionExtensions.FindDirection(entranceRoom.Entrance[0], entranceRoom.Entrance[1], _tilemapFloor);
        List<Vector3Int> moovingCoordinatesLeft = exitRoom.Exit.Select(x => x.Coordinates).ToList();

        //position to the start of the padding zone from left
        if (startingDirection == Direction.North || startingDirection == Direction.South)
        {
            moovingCoordinatesLeft = ToPaddingRegion(moovingCoordinatesLeft, startingDirection,exitRoom.AssetsCollection);
        }
        
        //Position the moovingCoordinates from left into the central region before moving on the axis
        moovingCoordinatesLeft = ToCenterRegion(moovingCoordinatesLeft, Direction.West, exitRoom,exitRoom.AssetsCollection);
        
        //position to the start of the padding zone from right
        List<Vector3Int> moovingCoordinatesRight = entranceRoom.Entrance.Select(x => x.Coordinates).ToList();
        if (arrivalDirection == Direction.North || arrivalDirection == Direction.South)
        {
            moovingCoordinatesRight = ToPaddingRegion(moovingCoordinatesRight, arrivalDirection, exitRoom.AssetsCollection);
        }
        
        //arrive to the central zone from right
        moovingCoordinatesRight = ToCenterRegion(moovingCoordinatesRight, Direction.East, entranceRoom,exitRoom.AssetsCollection);

        //move on the vertical central axes
        ConnectOnTheCentr(moovingCoordinatesLeft,moovingCoordinatesRight,exitRoom.AssetsCollection);
    }

    private void ConnectOnTheCentr(List<Vector3Int> moovingCoordinatesFromLeft, List<Vector3Int> moovingCoordinatesFromRight, AssetsCollection asset)
    {
        if (moovingCoordinatesFromLeft.Min(x=> x.y) > moovingCoordinatesFromRight.Max(x=>x.y))//means that from the left we are coming more higher
        {
            int minY = moovingCoordinatesFromLeft.Min(x=>x.y);
            //place the walls on the corner of the border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallRight)[0]);
            //place the walls on the top of the border and on the right border
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallsInternal)[0]);
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft[i].y,0),asset.GetTileFromType(AssetType.WallRight)[0]);
            }
            //Rotate the mooving coordinates in order to reach the bottom
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                moovingCoordinatesFromLeft[i] = new Vector3Int(moovingCoordinatesFromLeft[i].x-i,minY,0);
            }
            //Start moving down and place the tile as long as the wall
            for (int i = minY; i > moovingCoordinatesFromRight.Min(pos=>pos.y); i--)
            {
                for (int j = 0; j < moovingCoordinatesFromLeft.Count; j++)
                {
                    _tilemapFloor.SetTile(moovingCoordinatesFromLeft[j]+Direction.South.GetDirection(),asset.GetTileFromType(AssetType.Floors)[0]);
                    moovingCoordinatesFromLeft[j] += Direction.South.GetDirection();
                }     
                if(i>moovingCoordinatesFromRight.Min(pos=>pos.y)+moovingCoordinatesFromRight.Count) _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Max(pos=>pos.x)+1,moovingCoordinatesFromLeft[0].y,0), asset.GetTileFromType(AssetType.WallRight)[0]);
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y,0),asset.GetTileFromType(AssetType.WallLeft)[0]);
            }
            //place the wall now in the bootom corner and bottom border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y-1,0),asset.GetTileFromType(AssetType.WallLeft)[0]);
            //place the walls on the top of the border and on the right border
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[i].x,moovingCoordinatesFromLeft.Max(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallExternal)[0]);
            }
        }else if (moovingCoordinatesFromLeft.Max(pos=> pos.y) < moovingCoordinatesFromRight.Min(pos=>pos.y))//means that we have to go up
        {
            int maxY = moovingCoordinatesFromLeft.Max(pos=>pos.y);
            //place the walls on the corner of the border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallRight)[0]);
            //place the walls on the top of the border and on the right border
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallExternal)[0]);
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft[i].y,0),asset.GetTileFromType(AssetType.WallRight)[0]);
            }
            //Rotate the mooving coordinates in order to reach the top
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                moovingCoordinatesFromLeft[i] = new Vector3Int(moovingCoordinatesFromLeft[i].x-i,maxY,0);
            }
            //Start moving up and place the tile as long as the wall
            for (int i = maxY; i < moovingCoordinatesFromRight.Max(pos=>pos.y); i++)
            {
                for (int j = 0; j < moovingCoordinatesFromLeft.Count; j++)
                {
                    _tilemapFloor.SetTile(moovingCoordinatesFromLeft[j]+Direction.North.GetDirection(),asset.GetTileFromType(AssetType.Floors)[0]);
                    moovingCoordinatesFromLeft[j] += Direction.North.GetDirection();
                }        
                if(i<moovingCoordinatesFromRight.Max(pos=>pos.y)-moovingCoordinatesFromRight.Count) _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Max(pos=>pos.x)+1,moovingCoordinatesFromLeft[0].y,0), asset.GetTileFromType(AssetType.WallRight)[0]);
                 _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y,0),asset.GetTileFromType(AssetType.WallLeft)[0]);
            }
            //place the wall now in top corner and upper border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y+1,0),asset.GetTileFromType(AssetType.WallLeft)[0]);
            //place the walls on the top of the border and on the right border
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[i].x,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallsInternal)[0]);
            }
        }    
    }

    private List<Vector3Int> ToCenterRegion(List<Vector3Int> moovingCoordinates, Direction direction, Room room, AssetsCollection asset)
    {
        int i;
        int topValue;
        if (direction==Direction.West)
        {
            i = moovingCoordinates[0].x;
            topValue = room.DisplacementX + room.RequiredWidthSpace + PaddingRoom + room.Exit.Count - 1;
        }else
        {
            i = 0;
            topValue = moovingCoordinates[0].x - (room.DisplacementX-PaddingRoom);
        }
        //Position the floor and the wall as long as we are reaching the central region
        for (; i < topValue; i++)
        {
            for (int j = 0; j < moovingCoordinates.Count; j++)
            {
                _tilemapFloor.SetTile(moovingCoordinates[j]+direction.GetDirection(),asset.GetTileFromType(AssetType.Floors)[0]);
                moovingCoordinates[j] += direction.GetDirection();
            }
            //here we place the wall only before the ending of the padding zone, the wall for the corner has to be decided when we know if we will go up or down
            if (i < room.DisplacementX + room.RequiredWidthSpace + PaddingRoom-1)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinates[0].x,moovingCoordinates.Max(pos=>pos.y)+1,0), asset.GetTileFromType(AssetType.WallsInternal)[0]);
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinates[0].x,moovingCoordinates.Min(pos=>pos.y)-1,0), asset.GetTileFromType(AssetType.WallExternal)[0]);
            }
        }
        //it returns the updated value of the moving coordinates up to now
        return moovingCoordinates;
    }

    private List<Vector3Int> ToPaddingRegion(List<Vector3Int> moovingCoordinates, Direction startingDirection, AssetsCollection asset)
    {
        //Move down for the size necessary to make a corner plus some padding
        for (int i = 0; i < moovingCoordinates.Count+PaddingRoom; i++)
        {
            //Put the floor as long as your are moving up or down    
            for (int j = 0; j < moovingCoordinates.Count; j++)
            {
                _tilemapFloor.SetTile(moovingCoordinates[j]+startingDirection.GetDirection(),asset.GetTileFromType(AssetType.Floors)[0]);
                moovingCoordinates[j] += startingDirection.GetDirection();
            }
            //Add also the wall, but if you are going:
            //- to South you have to put also the wall after the padding zone before doing the cornet on the left
            //- to North you have to put also the wall after the padding zone before doing the cornet on the right
            if (startingDirection == Direction.South)
            {
                if(i<PaddingRoom) _tilemapWall.SetTile(
                    new Vector3Int(moovingCoordinates.Max(x => x.x) + 1, moovingCoordinates[0].y, 0),
                    asset.GetTileFromType(AssetType.WallRight)[0]);
                _tilemapWall.SetTile(
                    new Vector3Int(moovingCoordinates.Min(x => x.x) - 1, moovingCoordinates[0].y, 0),
                    asset.GetTileFromType(AssetType.WallLeft)[0]);
            }
            else
            {
                if(i<PaddingRoom) _tilemapWall.SetTile(
                    new Vector3Int(moovingCoordinates.Min(x => x.x) - 1, moovingCoordinates[0].y, 0),
                    asset.GetTileFromType(AssetType.WallLeft)[0]);
                _tilemapWall.SetTile(
                    new Vector3Int(moovingCoordinates.Max(x => x.x) + 1, moovingCoordinates[0].y, 0),
                    asset.GetTileFromType(AssetType.WallRight)[0]);
                
            }
        }
        //Put the tile of the wall also on the corner
        if(startingDirection==Direction.South) _tilemapWall.SetTile(
            new Vector3Int(moovingCoordinates.Min(x => x.x)-1, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
            asset.GetTileFromType(AssetType.WallLeft)[0]);
        else _tilemapWall.SetTile(
            new Vector3Int(moovingCoordinates.Max(x => x.x)+1, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
            asset.GetTileFromType(AssetType.WallLeft)[0]);

            //Put the tile of the wall also on the other part of the cornet before changing and rotating the moving direction in the West direction
        foreach (Vector3Int pos in moovingCoordinates)
        {
            _tilemapWall.SetTile(
                new Vector3Int(pos.x, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
                startingDirection==Direction.North? asset.GetTileFromType(AssetType.WallsInternal)[0]:asset.GetTileFromType(AssetType.WallExternal)[0]);
        }
        //Rotate the moving directions in order to continue and go to West in the next function
        for (int i = 0; i < moovingCoordinates.Count; i++)
        {
            if (startingDirection == Direction.North)
                moovingCoordinates[i] = new Vector3Int(moovingCoordinates.Max(x=>x.x), moovingCoordinates[i].y - i, 0);
            else 
                moovingCoordinates[i] = new Vector3Int(moovingCoordinates.Max(x=>x.x), moovingCoordinates[i].y + i, 0);
        }
        //return the updated coordinates
        return moovingCoordinates;
    }

    private void RoomPlacement()
    {
        foreach (Room room in _selectedRooms)  
        {
            Vector3Int coordinates = _selectedRooms.IndexOf(room)==0 ? new Vector3Int(0,0,0) : RandomCoordinates(room,_selectedRooms[_selectedRooms.IndexOf(room)-1]);
            //while (!FreeSpace(room, coordinates = RandomCoordinates())); //start placing the starting room
            room.PlaceRoom(_tilemapFloor,_tilemapWall,_tilemapObject,coordinates); 
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

    private Vector3Int RandomCoordinates(Room nextRoom,Room previousRoom)
    {
        //int randomX = Random.Range(previousRoom.DisplacementX+previousRoom.RequiredWidthSpace+PaddingRoom+previousRoom.Exit.Count+PaddingRoom, PaddingRoom + previousRoom.Exit.Count+ previousRoom.DisplacementX+previousRoom.RequiredWidthSpace +nextRoom.RequiredWidthSpace+PaddingRoom);
        int randomX = previousRoom.DisplacementX + previousRoom.RequiredWidthSpace + PaddingRoom +
                      previousRoom.Exit.Count + PaddingRoom;
        int randomY = Random.Range(-10,10);
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
        roomCollection.EndingRoom.Generate();

    }


    private Room SelectRoom()
    {
        int index = Random.Range(0, roomCollection.Rooms.Count);
        while (_selectedRooms.Contains(roomCollection.Rooms[index]))
        {
            index = Random.Range(0, roomCollection.Rooms.Count);;
        }
        return roomCollection.Rooms[index];
    }

    //TODO use ID of player to select spawn 1 or 2
    public void PlacePlayer(PlayerControllerMap player, int playerID)
    {
        player.SetLocation(_tilemapFloor.layoutGrid.CellToWorld(_selectedRooms[0].Spawn[0].Coordinates)+new Vector3Int(1,1,0));
        //player.SetLocation(_tilemapFloor.layoutGrid.CellToWorld(_selectedRooms[_selectedRooms.Count-1].Spawn[0].Coordinates)+new Vector3Int(1,1,0));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoomMaze : RoomAbstract
{
    private const int _minSetSpace=20;
    private const int _maxSetSpace=100;

    [SerializeField][Range(_minSetSpace,_maxSetSpace)] private int maxSpace;
    [SerializeField] private int minRoomSize;
    
    private int _sizeX, _sizeY;

    private Stack<Cell> _otherRoomCellsStack = new Stack<Cell>();
    private Stack<Cell> _actualRoomCellsStack = new Stack<Cell>();
    private List<Room> _roomList = new List<Room>();
    private Dictionary<int,Cell> _cellMap = new Dictionary<int, Cell>();
    public override void Generate(int seed)
    {
        Random.InitState(seed);
        while ((_sizeX = Random.Range(_minSetSpace, maxSpace)) % 2 != 0);
        while ((_sizeY = Random.Range(_minSetSpace, maxSpace)) % 2 != 0);
        // _sizeX = 6;
        // _sizeY = 6;
        _requiredWidthSpace = _sizeX+2;//due to the walls
        
        GenerateWall();
        GenerateInternal();
        ReshapeRooms();
        GenerateTile();
        _lowestX = -1;//these are due to the presence of the wall
        _lowestY = -1;
    }

    private void ReshapeRooms()
    {
        for (int i = 0; i < _roomList.Count; i++)
        {
            if(_roomList[i].GetCellsList().Count<minRoomSize){}
        }
    }

    private void GenerateTile()
    {
        foreach (Room room in _roomList)
        {
            foreach (Cell cell in room.GetCellsList())
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                       Vector2Int position = new Vector2Int(cell.Position.x*2+i,cell.Position.y*2+j);
                       Tile tile = new Tile(assetsCollection.GetTileFromType(AssetType.Floor)[0],(Vector3Int)position);
                       tile.Color = room.Color;
                       Floor.Add(tile);
                       TileList.Add(tile); 
                    }
                }
            }
        }
    }

    private void GenerateInternal()
    {
        Room startingRoom = new Room();
        _roomList.Add(startingRoom);
        int randX = Random.Range(0,_sizeX/2-1);
        int randY = Random.Range(0, _sizeY / 2 - 1);
        Cell startingCell = new Cell(new Vector2Int(randX,randY), startingRoom);
        startingRoom.AddCell(startingCell);

        _actualRoomCellsStack.Push(startingRoom.GetFirstCell());
        while (_actualRoomCellsStack.Count>0)
        {
            Cell actualCell = _actualRoomCellsStack.Pop();
            VisitNeighbor(actualCell);
            if (_actualRoomCellsStack.Count <= 0)
            {
                Cell nextCell;
                if (_otherRoomCellsStack.Count > 0)
                {
                    for (nextCell = _otherRoomCellsStack.Pop();
                        _otherRoomCellsStack.Count > 0 && !(nextCell.Room is null);
                        nextCell = _otherRoomCellsStack.Pop()) ;
                    if ((nextCell.Room is null)) AddNewRoom(nextCell);
                }
            }
        }
        
    }
    
    private void AddNewRoom(Cell cell)
    {
        Room room = new Room();
        _roomList.Add(room);
        cell.Room = room;
        room.AddCell(cell);
        _actualRoomCellsStack.Push(cell);
    }

    private void VisitNeighbor(Cell cell)
    {
        foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
        {
            if (cell.GetEdge(direction) is null)
            {
                Vector2Int neighborPosition = cell.Position + (Vector2Int) direction.GetDirection();
                if (neighborPosition.x < 0 || neighborPosition.y < 0 || neighborPosition.x>=_sizeX/2 || neighborPosition.y>=_sizeY/2) cell.SetEdge(direction, new Edge(cell, null));
                else
                {
                    Cell otherCell;
                    if (_cellMap.ContainsKey(GetID(neighborPosition)))
                    {
                        otherCell = _cellMap[GetID(neighborPosition)];
                    }
                    else
                    {
                        otherCell = AddCell(null, neighborPosition);
                        _otherRoomCellsStack.Push(otherCell);
                    }
                    
                    if(!(otherCell.Room is null))cell.Room.AddNeighbor(otherCell.Room);
                    
                    SetConnection(direction, cell, otherCell);
                    if (otherCell.Room is null && Random.Range(1, 100) > 60)
                    {
                        cell.Room.AddCell(otherCell);
                        _actualRoomCellsStack.Push(otherCell);
                    }else if(!_otherRoomCellsStack.Contains(otherCell)) 
                        _otherRoomCellsStack.Push(otherCell);
                }
            }
        }
    }

    private void SetConnection(Direction direction, Cell cell, Cell otherCell)
    {
        Edge edge = new Edge(cell,otherCell);
        cell.SetEdge(direction, edge);
        otherCell.SetEdge(direction.GetOpposite(),edge);
    }

    private int GetID(Vector2Int position)
    {
        return position.x + _sizeX / 2 * position.y;
    }

    private void GenerateWall()
    {
        bool hasEntrance = false, hasExit = false;
        Direction[] directions = {Direction.East, Direction.South, Direction.West, Direction.North};
        int directionChange = 0;
        int index = _sizeX;//starts from the upper left side and we go in an anti clockwise round
        int startingIndex = index;
        Vector3Int position = new Vector3Int(_sizeX-1,_sizeY,0);
        while (directionChange < 4)
        {
            if ((directionChange == 1 || directionChange == 0))
            {
                if (!hasEntrance && ((directionChange == 1 && index == 3) || (Random.Range(1, 100) > 90 && index%2==0 && index<startingIndex)))
                {
                    hasEntrance = true;
                    index--;
                    Tile tile = new Tile(assetsCollection.GetTileFromType(AssetType.Entrace)[0],
                        position);
                    position += directions[directionChange].GetDirection();
                    TileList.Add(tile);
                    Entrance.Add(tile);
                    tile = new Tile(assetsCollection.GetTileFromType(AssetType.Entrace)[0], 
                        position);
                    TileList.Add(tile);
                    Entrance.Add(tile);
                }else
                {
                    PutWall(directionChange,position);
                }
            }
            else
            {
                if (!hasExit && ((directionChange == 3 && index == 3) || (Random.Range(1, 100) >90 && index%2==0 && index<startingIndex)))
                {
                    hasExit = true;
                    index--;
                    Tile tile = new Tile(assetsCollection.GetTileFromType(AssetType.Exit)[0],
                        position);
                    position += directions[directionChange].GetDirection();
                    TileList.Add(tile);
                    Exit.Add(tile);
                    tile = new Tile(assetsCollection.GetTileFromType(AssetType.Exit)[0],
                        position);
                    TileList.Add(tile);
                    Exit.Add(tile);
                }else
                {
                    PutWall(directionChange,position);
                }
            }
            
            index--;
            position += directions[directionChange].GetDirection();
            if (index <= 0)
            {
                directionChange++;
                switch (directionChange)
                {
                    case 1:
                        index = _sizeY + 1;
                        break;
                    case 2:
                        index = _sizeX + 1;
                        break;
                    case 3:
                        index = _sizeY + 2;
                        break;
                }

                startingIndex = index;
            }
        }
    }

    private Cell AddCell(Room room, Vector2Int position)
    {
        Cell cell = new Cell(position, room);
        _cellMap.Add(GetID(cell.Position),cell);
        if(!(room is null)) room.AddCell(cell);
        return cell;

    }

    private void PutWall(int directionChange,Vector3Int position)
    {
        Tile tile;
        if(directionChange<2) tile = new Tile(assetsCollection.GetTileFromType(AssetType.WallTopLeft)[0],
            position);
        else tile = new Tile(assetsCollection.GetTileFromType(AssetType.WallBottomRight)[0],
            position);
                
        TileList.Add(tile);
        Wall.Add(tile);
    }
    
    public override void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall, Tilemap tilemapObject, Vector3Int coordinates) 
    {
        _displacementX = coordinates.x;
        _displacementY = coordinates.y;
        foreach (var tile in TileList)
        {
            tile.Coordinates = tile.Coordinates - new Vector3Int(_lowestX, _lowestY, 0) + coordinates;
            if (Wall.Contains(tile))
                tilemapWall.SetTile(tile.Coordinates, tile.TileBase);
            else
            {
                tilemapFloor.SetTile(tile.Coordinates, tile.TileBase);
                if (tile.HasColor())
                {
                    tilemapFloor.SetColor(tile.Coordinates,tile.Color);
                }
            }
        }

        foreach (Transform transform1 in Object)
        {
            GameObject gameObject = Instantiate(transform1.gameObject);
            gameObject.name = transform1.gameObject.name;
            transform1.position = transform1.position - new Vector3Int(_lowestX, _lowestY, 0) + coordinates;
        }
    }
}

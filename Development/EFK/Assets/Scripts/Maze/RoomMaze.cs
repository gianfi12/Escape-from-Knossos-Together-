using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoomMaze : RoomAbstract
{
    private const int _minSetSpace=21;//has to be odd
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
        int random;
        while ((random = Random.Range(_minSetSpace, maxSpace)) % 2 == 0);
        //while ((_sizeY = Random.Range(_minSetSpace, maxSpace)) % 2 == 0);
        _sizeX = random;
        _sizeY = random;
        _requiredWidthSpace = _sizeX+2;//due to the walls
        
        GenerateInternal();
        FindRoomNeighbor();
        ReshapeRooms();
        ConnectNeighbor();
        GenerateTile();
        GenerateWall();

        _lowestX = -1;//these are due to the presence of the wall
        _lowestY = -1;
    }

    private void ConnectNeighbor()
    {
        foreach (Room room in _roomList)
        {
            foreach (Room neighbor in room.GetNeighbor())
            {
                if (!neighbor.IsNeighborAlreadyConnected(room))
                {
                    neighbor.AddConnectedNeighbor(room);
                    room.AddConnectedNeighbor(neighbor);
                    List<Cell> _neighborCell = new List<Cell>();
                    foreach (Cell cell in room.GetCellsList())
                    {
                        if(cell.IsAdjacent(neighbor)) _neighborCell.Add(cell);
                    }
                    Cell selectedCell = _neighborCell[Random.Range(0,_neighborCell.Count-1)];
                    Edge selectedEdge = selectedCell.GetAdjacentEdge(neighbor);
                    selectedEdge.EdgeType = Edge.EdgeTypes.Door;
                }
                else
                {
                    room.AddConnectedNeighbor(neighbor);
                }
            }
        }
    }

    private void FindRoomNeighbor()
    {
        foreach (Room room in _roomList)
        {
            foreach (Cell cell in room.GetCellsList())
            {
                foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
                {
                    Edge edge = cell.GetEdge(direction);
                    if(!(edge is null))
                    {
                        Cell otherCell = cell.GetEdge(direction).GetOther(cell);
                        if(!(otherCell is null)) room.AddNeighbor(otherCell.Room);
                    } 
                }
            }
        }
    }

    private void ReshapeRooms()
    {
        for (int i = 0; i < _roomList.Count; i++)
        {
            if (_roomList[i].GetCellsList().Count < minRoomSize)
            {
                Room removedRoom = _roomList[i];
                _roomList.Remove(removedRoom);
                int index = Random.Range(0, removedRoom.GetNeighbor().Count-1);
                Room mergedRoom = removedRoom.GetNeighbor()[index];
                mergedRoom.RemoveNeighbor(removedRoom);
                foreach (Cell cell in removedRoom.GetCellsList())
                {
                    mergedRoom.AddCell(cell);
                    cell.ReshapeCell(mergedRoom);
                }
            }
        }

        foreach (Room room in _roomList)
        {
            room.ResetNeighbor();
        }
        FindRoomNeighbor();
    }

    private void GenerateTile()
    {
        //first position the floor
        foreach (Cell cell in _cellMap.Values) 
        {
            Vector2Int position = new Vector2Int(cell.Position.x*2+1,cell.Position.y*2+1); 
            PositionTile(position,cell.Room.Color,Floor,assetsCollection.GetTileFromType(AssetType.Floor)[0]);
            foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
            {
                Cell otherCell = cell.GetEdge(direction).GetOther(cell);
                if ((cell.GetEdge(direction).EdgeType==Edge.EdgeTypes.Passage || cell.GetEdge(direction).EdgeType==Edge.EdgeTypes.Door))
                {
                    Vector2Int startingPosition;
                    Direction movementDirection;
                    if(otherCell is null) otherCell = new Cell(cell.Position+(Vector2Int)direction.GetDirection(),null);
                    if (cell.Position.x == otherCell.Position.x)
                    {
                        movementDirection = Direction.West;
                        if (cell.Position.y > otherCell.Position.y)
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2,cell.Position.y*2);
                        }
                        else
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2,cell.Position.y*2+2);
                        }
                    }
                    else
                    {
                        movementDirection = Direction.North;
                        if (cell.Position.x < otherCell.Position.x)
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2+2,cell.Position.y*2);
                        }
                        else
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2,cell.Position.y*2);
                        }
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        PositionTile(startingPosition,cell.Room.Color,Floor,assetsCollection.GetTileFromType(AssetType.Floor)[0]);
                        startingPosition += (Vector2Int)movementDirection.GetDirection();
                    }
                }
            }
        }
        
        // //then the walls
        foreach (Cell cell in _cellMap.Values)
        {
            foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
            {
                Cell otherCell = cell.GetEdge(direction).GetOther(cell);
                if ((cell.GetEdge(direction).EdgeType==Edge.EdgeTypes.Wall))
                {
                    Vector2Int startingPosition;
                    Direction movementDirection;
                    if(otherCell is null) otherCell = new Cell(cell.Position+(Vector2Int)direction.GetDirection(),null);
                    if (cell.Position.x == otherCell.Position.x)
                    {
                        movementDirection = Direction.West;
                        if (cell.Position.y > otherCell.Position.y)
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2,cell.Position.y*2);
                        }
                        else
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2,cell.Position.y*2+2);
                        }
                    }
                    else
                    {
                        movementDirection = Direction.North;
                        if (cell.Position.x < otherCell.Position.x)
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2+2,cell.Position.y*2);
                        }
                        else
                        {
                            startingPosition = new Vector2Int(cell.Position.x*2,cell.Position.y*2);
                        }
                    }
                    TileBase tileBase;
                    if (direction == Direction.East || direction == Direction.North)
                    {
                        tileBase = assetsCollection.GetTileFromType(AssetType.WallTopLeft)[0];
                    }
                    else tileBase = assetsCollection.GetTileFromType(AssetType.WallBottomRight)[0];
                    for (int i = 0; i < 3; i++)
                    {
                        PositionTile(startingPosition,cell.Room.Color,Floor,tileBase);
                        startingPosition += (Vector2Int)movementDirection.GetDirection();
                    }
                }
            }
        }
    }

    private void PositionTile(Vector2Int position, Color color, List<Tile> specificList, TileBase tileBase)
    {
        Tile tile = new Tile(tileBase,(Vector3Int)position);
        tile.Color = color;
        specificList.Add(tile);
        TileList.Add(tile);
    }
    
    private void GenerateInternal()
    {
        Room startingRoom = new Room();
        _roomList.Add(startingRoom);
        int randX = Random.Range(0,(_sizeX-1)/2);
        int randY = Random.Range(0, (_sizeY-1)/2 );
        Cell startingCell = AddCell(startingRoom, new Vector2Int(randX, randY));

        _actualRoomCellsStack.Push(startingCell);
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
                if (neighborPosition.x < 0 || neighborPosition.y < 0 || neighborPosition.x>=(_sizeY - 1)/2  || neighborPosition.y>=(_sizeY - 1)/2)
                {
                    cell.SetEdge(direction, new Edge(cell, null));
                    cell.GetEdge(direction).EdgeType = Edge.EdgeTypes.Passage;
                }
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

                    Edge edge = SetConnection(direction, cell, otherCell);
                    if (otherCell.Room is null && Random.Range(1, 100) > 60)
                    {
                        edge.EdgeType = Edge.EdgeTypes.Passage;
                        cell.Room.AddCell(otherCell);
                        _actualRoomCellsStack.Push(otherCell);
                    }else
                    {
                        edge.EdgeType = Edge.EdgeTypes.Wall;
                        if (!_otherRoomCellsStack.Contains(otherCell))
                        {
                            _otherRoomCellsStack.Push(otherCell);
                        }
                    }
                }
            }
        }
    }

    private Edge SetConnection(Direction direction, Cell cell, Cell otherCell)
    {
        Edge edge = new Edge(cell,otherCell);
        cell.SetEdge(direction, edge);
        otherCell.SetEdge(direction.GetOpposite(),edge);
        return edge;
    }

    private int GetID(Vector2Int position)
    {
        return position.x + (_sizeY - 1)/2  * position.y;
    }

    private void GenerateWall()
    {
        bool hasEntrance = false, hasExit = false;
        Direction[] directions = {Direction.East, Direction.South, Direction.West, Direction.North};
        int directionChange = 0;
        int exitHasToBeInDirectionChange=-1;
        int index = _sizeX;//starts from the upper left side and we go in an anti clockwise round
        int startingIndex = index;
        Vector3Int position = new Vector3Int(_sizeX-1,_sizeY,0);
        while (directionChange < 4)
        {
            if ((directionChange == 1 || directionChange == 0))
            {
                if (!hasEntrance && ((directionChange == 1 && index == 2) || (Random.Range(1, 100) > 90 && index%2==0 && index<startingIndex)))
                {
                    hasEntrance = true;
                    Tile tile = new Tile(assetsCollection.GetTileFromType(AssetType.Entrace)[0],
                        position); 
                    TileList.Add(tile);
                    Entrance.Add(tile);
                    exitHasToBeInDirectionChange = directionChange+2;
                }else
                {
                    PutWall(directionChange,position);
                }
            }
            else
            {
                if (!hasExit && ((directionChange == exitHasToBeInDirectionChange && index == 2) || (Random.Range(1, 100) >90 && index%2==0 && index<startingIndex && exitHasToBeInDirectionChange==directionChange)))
                {
                    hasExit = true;
                    Tile tile = new Tile(assetsCollection.GetTileFromType(AssetType.Exit)[0],
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
                // if (tile.HasColor())
                // {
                //     tilemapFloor.SetColor(tile.Coordinates,tile.Color);
                // }
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

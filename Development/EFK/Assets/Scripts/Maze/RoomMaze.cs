﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RoomMaze : RoomAbstract
{
    private const int _minSetSpace=31;//has to be odd
    private const int _maxSetSpace=31;

    [SerializeField][Range(_minSetSpace,_maxSetSpace)] private int maxSpace;
    [SerializeField] private int minRoomSize;
    [SerializeField] private GameObject doorUpPrefab;
    [SerializeField] private GameObject doorLatePrefab;
    [SerializeField] private GameObject wardrobePrefab;
    [SerializeField] private int numberOfWardrobePerRegion; //TODO use this field
    [SerializeField] private GameObject guardPrefab;
    [SerializeField] private int numberOfGuard;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject resetLever;
    [SerializeField] private GameObject pressedButtonsGUI;
    [SerializeField] private GameObject fadeText;
    private RoomCollider roomCollider;

    private int _sizeX, _sizeY;

    private Stack<Cell> _otherRoomCellsStack = new Stack<Cell>();
    private Stack<Cell> _actualRoomCellsStack = new Stack<Cell>();
    private List<Room> _roomList = new List<Room>();
    private Dictionary<int,Cell> _cellMap = new Dictionary<int, Cell>();
    private Transform _mazeTransform;
    private Transform _doorExitTransform;
    private bool _isPlayer2;
    private List<List<GameObject>> _listCheckpoints = new List<List<GameObject>>();
    private List<AgentController> _listAgent;
    private System.Random rnd;
    private List<Vector3Int> _occupiedTile;
    private GameObject _resetLeverInstance;
    private bool _isVerical;

    private Vector3Int _coordinatesNotEntrance;
    private Vector3Int _coordinatesNotExit;
    public override void Generate(int seed, bool isPlayer2)
    {
        rnd = new System.Random(seed);
        _regions = new List<Region>();
        TileList = new List<Tile>();
        Wall = new List<Tile>();
        Floor = new List<Tile>();
        Spawn = new List<Tile>();
        Decoration = new List<Tile>();
        Entrance = new List<Tile>();
        Exit = new List<Tile>();
        diaryImageList = new List<Image>();
        _occupiedTile = new List<Vector3Int>();
        _otherRoomCellsStack = new Stack<Cell>();
        _actualRoomCellsStack = new Stack<Cell>();
        _cellMap = new Dictionary<int, Cell>();
        _listCheckpoints = new List<List<GameObject>>();
        _roomList = new List<Room>();
        _isPlayer2 = isPlayer2;
        _mazeTransform = new GameObject("RoomMaze").transform;
        ObjectsContainer objectsContainer = _mazeTransform.gameObject.AddComponent<ObjectsContainer>();
        objectsContainer.Seed = seed;
        Random.InitState(seed);
        rnd = new System.Random(seed);
        int random;
        while ((random = Random.Range(_minSetSpace, maxSpace)) % 3 != 0);
        _sizeX = random;
        _sizeY = random;
        _requiredWidthSpace = _sizeX+2;//due to the walls
        
        GenerateInternal();
        FindRoomNeighbor();
        ReshapeRooms();
        ReshapeRooms();
        ConnectNeighbor();
        GenerateTile();
        GenerateWall();
        SpawnAgent();
        removeOverlappingWallsAndFloor();
        SpawnResetLever();

        _lowestX = -1;//these are due to the presence of the wall
        _lowestY = -1;
    }

    private void SpawnResetLever()
    {
        int offsetX = _sizeX / 3;
        int offsetY = _sizeY / 3;
        Tile tile;
        bool found;
        do
        {
            found = false;
            int index = rnd.Next(0, Wall.Count);
            tile = Wall[index];
            Vector3Int searchCoordinates = tile.Coordinates + Direction.South.GetDirection();
            foreach (Tile wall in Wall)
            {
                if (wall.Coordinates == searchCoordinates)
                {
                    found = true;
                    break;
                }
            }
        } while (found || tile.Coordinates.x<=offsetX || tile.Coordinates.x>offsetX*2 || tile.Coordinates.y<=offsetY || tile.Coordinates.y>offsetY*2);

        _resetLeverInstance = Instantiate(resetLever);
        SpawnObjectInRoom(tile,tile.Coordinates+new Vector3(0.5f,0.25f,0f),_resetLeverInstance.transform);
    }

    private void AddCollider()
    {
        GameObject collider = new GameObject("Collider");
        collider.layer = LayerMask.NameToLayer("Ignore Raycast");
        collider.transform.SetParent(_mazeTransform);
        BoxCollider2D boxCollider2D = collider.AddComponent<BoxCollider2D>();
        roomCollider = collider.AddComponent<RoomCollider>();
        roomCollider.Room = this;
        collider.transform.position = new Vector3(_sizeX/2+_displacementX+1f,_sizeY/2+_displacementY+1f,0f);
        boxCollider2D.isTrigger = true;
        if(_isVerical)
        {
            collider.transform.position =
                new Vector3(_sizeX / 2 + _displacementX +1f, _sizeY / 2 + _displacementY+1f, 0f);
            boxCollider2D.size = new Vector2(_sizeX , _sizeY );
        }
        else
        {
            collider.transform.position =
                new Vector3(_sizeX / 2 + _displacementX + 1f, _sizeY / 2 + _displacementY + 1f, 0f);
            boxCollider2D.size = new Vector2(_sizeX + 1f, _sizeY + 1f);
        }
        GameObject textObj = Instantiate(fadeText, _mazeTransform);
        FadeText text = textObj.GetComponentInChildren<FadeText>();
        text.gameObject.SetActive(false);
        roomCollider.AddActivatableObject(text);
    }

    private void SpawnAgent()
    {
        _listAgent = new List<AgentController>();
        for (int i = 0; i < numberOfGuard; i++)
        {
            Tile floor = getRandomFloor();
            
            List<GameObject> localListCheckpoints = new List<GameObject>();
            GameObject go1 = new GameObject();
            go1.name = "Checkpoint1AgentMaze" + i;
            GameObject go2 = new GameObject();
            go2.name = "Checkpoint2AgentMaze" + i;
            localListCheckpoints.Add(go1);
            localListCheckpoints.Add(go2);
            _listCheckpoints.Add(localListCheckpoints);
            Tile tile = getRandomFloor();
            while (checkOccupiedTile(tile.NormalizedCoordinates))tile = getRandomFloor();
            SpawnObjectInRoom(tile ,tile.Coordinates+new Vector3(0.55f,0.5f,0f),go1.transform);
            tile = getRandomFloor();
            while (checkOccupiedTile(tile.NormalizedCoordinates))tile = getRandomFloor();
            SpawnObjectInRoom(tile,tile.Coordinates+new Vector3(0.55f,0.5f,0f),go2.transform);
            Transform agentTransform = Instantiate(guardPrefab).transform;
            agentTransform.name = "AgentMaze"+i;
            AgentController agentController = agentTransform.GetComponent<AgentController>();
            _listAgent.Add(agentController);
            agentController.IsPatroller = false;
            Vector3 agentPosition=floor.Coordinates+new Vector3(0.5f,0.5f,0f);
            SpawnObjectInRoom(null,agentPosition,agentTransform);
        }
    }

    private void SpawnObjectInRoom(Tile tile, Vector3 position,Transform objectTrasform)
    {
        objectTrasform.SetParent(_mazeTransform);
        objectTrasform.position = position;
        if(tile!=null)
            _occupiedTile.Add(tile.NormalizedCoordinates);
    }

    private void InsertWardrobe(Tile tile,Vector3 wardrobePosition)
    {
        Transform wardrobeTransform = Instantiate(wardrobePrefab).transform;
        SpawnObjectInRoom(tile,wardrobePosition,wardrobeTransform);
    }

    Tile getRandomFloor()
    {
        Tile floorTile=null;
        bool isOverlapped=true;
        while (isOverlapped)
        {
            floorTile = Floor[Random.Range(0,Floor.Count-1)];
            isOverlapped = false;
            for (int i = 0; i < Wall.Count && !isOverlapped; i++)
            {
                if (Wall[i].Coordinates == floorTile.Coordinates) isOverlapped = true;
            }
        }
        return floorTile;
    }

    bool thereIsAWall(Direction direction,Vector3Int checkPosition)
    {
        bool found = false;
        for (int i = 0; i < Wall.Count && !found; i++)
        {
            if (Wall[i].Coordinates == checkPosition) found = true;
        }
        return found;
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
                foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(x => x != Direction.None))
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
                if (removedRoom.GetNeighbor().Count > 0) {
                    int index = Random.Range(0, removedRoom.GetNeighbor().Count - 1);
                    Room mergedRoom = removedRoom.GetNeighbor()[index];
                    mergedRoom.RemoveNeighbor(removedRoom);
                    foreach (Cell cell in removedRoom.GetCellsList()) {
                         mergedRoom.AddCell(cell);
                         cell.ReshapeCell(mergedRoom);
                    }
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
            foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(x => x != Direction.None))
            {
                Cell otherCell = cell.GetEdge(direction).GetOther(cell);
                if ((cell.GetEdge(direction).EdgeType==Edge.EdgeTypes.Passage || cell.GetEdge(direction).EdgeType==Edge.EdgeTypes.Door || otherCell.Room==cell.Room))
                {
                    Vector2Int startingPosition;
                    Direction movementDirection;
                    if(otherCell is null) otherCell = new Cell(cell.Position+(Vector2Int)direction.GetDirection(),null);
                    if (cell.Position.x == otherCell.Position.x)
                    {
                        movementDirection = Direction.East;
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
            foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(x => x != Direction.None))
            {
                Cell otherCell = cell.GetEdge(direction).GetOther(cell);
                if ((cell.GetEdge(direction).EdgeType==Edge.EdgeTypes.Wall) && otherCell.Room!=cell.Room)
                {
                    Vector2Int startingPosition;
                    Direction movementDirection;
                    if(otherCell is null) otherCell = new Cell(cell.Position+(Vector2Int)direction.GetDirection(),null);
                    if (cell.Position.x == otherCell.Position.x)
                    {
                        movementDirection = Direction.East;
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
                    tileBase = assetsCollection.GetTileFromType(AssetType.InternalWall)[0];
                    for (int i = 0; i < 3; i++)
                    {
                        PositionTile(startingPosition,cell.Room.Color,Wall,tileBase);
                        startingPosition += (Vector2Int)movementDirection.GetDirection();
                    }
                }
            }
        }
    }

    private void PositionTile(Vector2Int position, Color color, List<Tile> specificList, TileBase tileBase)
    {
        Tile tile = new Tile(tileBase,(Vector3Int)position);
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
        foreach (Direction direction in Enum.GetValues(typeof(Direction)).Cast<Direction>().Where(x => x != Direction.None))
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
                    if (otherCell.Room is null && Random.Range(1, 100) > 65 && cell.Room.GetCellsList().Count<10) // Change value here if you want to increase the complexity
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
        Direction[] directions = {Direction.West, Direction.South, Direction.East, Direction.North};
        int directionChange = 0;
        int exitHasToBeInDirectionChange=-1;
        int index = _sizeX;//starts from the upper left side and we go in an anti clockwise round
        int startingIndex = index;
        Vector3Int position = new Vector3Int(_sizeX-1,_sizeY-1,0);
        while (directionChange < 4)
        {
            if ((directionChange == 1 || directionChange == 0))
            {
                if (!hasEntrance && ((directionChange == 1 && index == 2) || (Random.Range(1, 100) > 95 && index%2==0 && index<startingIndex)))
                {
                    hasEntrance = true;
                    Tile tile = new Tile(assetsCollection.GetTileFromType(AssetType.Entrace)[0],position); 
                    TileList.Add(tile);
                    Entrance.Add(tile);
                    exitHasToBeInDirectionChange = directionChange+2;
                    GameObject selectedGO = directionChange == 0 ? doorUpPrefab : doorLatePrefab;
                    Transform doorTransform = Instantiate(selectedGO).transform;
                    Doors doorScript = doorTransform.GetComponent<Doors>();
                    if(_isPlayer2)
                    {
                        _doorExitTransform = doorTransform;
                    }
                    else
                        doorScript.OpenDoors();
                    Vector3 doorPosition;
                    if (directionChange == 0)
                    {
                        _isVerical = true;
                        // doorTransform.rotation *= Quaternion.Euler(0, 0, 90);
                        doorPosition = position + new Vector3(0.5f, 0.5f, 0);
                        doorTransform.GetComponent<Doors>().ClosingDirection=Direction.South;
                        _coordinatesNotEntrance = position + Direction.South.GetDirection();
                    }
                    else
                    {
                        _isVerical = false;
                        // doorTransform.rotation *= Quaternion.Euler(0, 0, 180);
                        doorPosition = position + new Vector3(0, 0.5f, 0);
                        doorTransform.GetComponent<Doors>().ClosingDirection=Direction.East;
                        _coordinatesNotEntrance = position + Direction.East.GetDirection();
                    }
                    if(_isPlayer2) doorTransform.GetComponent<Doors>().ClosingDirection=doorTransform.GetComponent<Doors>().ClosingDirection.GetOpposite();
                    SpawnObjectInRoom(null,doorPosition,doorTransform);
                    if(directionChange!=0)
                    {
                        index--;
                        position += directions[directionChange].GetDirection();
                        PutWall(3, position);
                    }
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
                    GameObject selectedGO = directionChange == 2 ? doorUpPrefab : doorLatePrefab;
                    Transform doorTransform = Instantiate(selectedGO).transform;
                    Doors doorScript = doorTransform.GetComponent<Doors>();
                    if(!_isPlayer2)
                    {
                        _doorExitTransform = doorTransform;
                    }else
                        doorScript.OpenDoors();
                    Vector3 doorPosition;
                    if (directionChange == 2)
                    {
                        // doorTransform.rotation *= Quaternion.Euler(0, 0, 90);
                        doorPosition = position + new Vector3(0.5f, 0.5f, 0);
                        doorTransform.GetComponent<Doors>().ClosingDirection=Direction.South;
                        _coordinatesNotExit = position + Direction.North.GetDirection();
                    }
                    else
                    {
                        // doorTransform.rotation *= Quaternion.Euler(0, 0, 180);
                        doorPosition = position + new Vector3(1, 0.5f, 0);
                        doorTransform.GetComponent<Doors>().ClosingDirection=Direction.East;
                        _coordinatesNotExit = position + Direction.West.GetDirection();
                    }
                    if(_isPlayer2) doorTransform.GetComponent<Doors>().ClosingDirection=doorTransform.GetComponent<Doors>().ClosingDirection.GetOpposite();
                    SpawnObjectInRoom(null ,doorPosition,doorTransform);
                    if(directionChange!=2)
                    {
                        index--;
                        position += directions[directionChange].GetDirection();
                        PutWall(0, position);
                    }
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
                        index = _sizeY ;
                        break;
                    case 2:
                        index = _sizeX;
                        break;
                    case 3:
                        index = _sizeY + 1;
                        break;
                }
                startingIndex = index;
            }
        }
        PutWall(0,new Vector3Int(_sizeX-1,_sizeY-1,0));
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
    
    public override void PlaceObject(Vector3Int coordinates)
    {
        _displacementX = coordinates.x;
        _displacementY = coordinates.y;
        
                
        _mazeTransform.position = coordinates+ new Vector3Int(1,1,0);
        AddCollider();
        int i = 0;
        foreach (AgentController agentController in _listAgent)
        {
            agentController.SetCheckpoints(_listCheckpoints[i]);
            agentController.gameObject.SetActive(false);
            roomCollider.AddActivatableObject(agentController);
            i++;
        }
    }
    
    
    public override void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall, Tilemap tilemapDecoration)
    {
        Transform buttonsCont = Instantiate(buttonPanel, _mazeTransform).transform;
        ColorButtonPanel entrancePanel = buttonsCont.GetComponent<ColorButtonPanel>();
        _resetLeverInstance.GetComponent<ResetLever>().EntrancePanel = entrancePanel;
        buttonsCont.GetComponent<PolygonCollider2D>().isTrigger = true;
        entrancePanel.ControlledDoors = _doorExitTransform.GetComponent<Doors>();

        GameObject pressedButtonsGUIInstance = Instantiate(pressedButtonsGUI, _mazeTransform);
        PressedButtons pressedButtonsGUIScript = pressedButtonsGUIInstance.GetComponentInChildren<PressedButtons>();
        entrancePanel.PressedButtonsGUI = pressedButtonsGUIScript;
        pressedButtonsGUIScript.gameObject.SetActive(false);
        _mazeTransform.GetComponentInChildren<RoomCollider>().AddActivatableObject(pressedButtonsGUIScript);

        foreach (Image image in entrancePanel.GUIImages)
        {
            AddDiaryImage(image);    
        }

        GenerateRegions(buttonsCont);

        foreach (var tile in TileList)
        {
            Vector3Int normalizedCoordinates = tile.Coordinates - new Vector3Int(_lowestX, _lowestY, 0);
            tile.Coordinates =  normalizedCoordinates + new Vector3Int(_displacementX,_displacementY,0);
            tile.NormalizedCoordinates = normalizedCoordinates;
            if (Wall.Contains(tile))
                tilemapWall.SetTile(tile.Coordinates, tile.TileBase);
            else
            {
                tilemapFloor.SetTile(tile.Coordinates, tile.TileBase);
                Region region = ColorFromCoordinates(normalizedCoordinates);
                if(region!=null)
                {
                    Color color = region.color;
                    color = new Color(color.r, color.g, color.b, 0.5f);
                    tilemapFloor.SetColor(tile.Coordinates, color);
                }
            }
        }

        int index = 0;
        foreach (Region region in _regions)
        {
            Tile floor=getRandomFloor();
            
            while (ColorFromCoordinates(floor.NormalizedCoordinates) != region ||
                   (ColorFromCoordinates(floor.NormalizedCoordinates) == region && checkOccupiedTile(floor.NormalizedCoordinates))) floor=getRandomFloor();
            region.button.position = floor.Coordinates+new Vector3(0.5f,0.5f,0f);
            _occupiedTile.Add(floor.NormalizedCoordinates);
            
            for(int i=0;i<numberOfWardrobePerRegion;i++)
            {
                floor = getRandomFloor();
                while (ColorFromCoordinates(floor.NormalizedCoordinates) != region ||
                       (ColorFromCoordinates(floor.NormalizedCoordinates) == region && checkOccupiedTile(floor.NormalizedCoordinates)))floor = getRandomFloor();
                InsertWardrobe(floor, floor.Coordinates + new Vector3(0.5f, 0.5f, -0.1f));
            }

            index++;
        }
        
    }
    
    protected class Region
    {
        public readonly Color color;
        public readonly Transform button;

        public Region(Color color, Transform button)
        {
            this.color = color;
            this.button = button;
        }
    }

    private List<Region> _regions = new List<Region>();

    private void GenerateRegions(Transform buttonsCont)
    {
        ColorButtonPanel colorButtonPanel = buttonsCont.GetComponent<ColorButtonPanel>();
        Color[] colors = colorButtonPanel.ButtonColors;
        for (int i = 0; i < buttonsCont.childCount; i++)
        {
            _regions.Add(new Region(colors[i],buttonsCont.GetChild(i)));
        }
        _regions = _regions.OrderBy(x => rnd.Next()).ToList();
    }


    private Region ColorFromCoordinates(Vector3Int tileCoordinates)
    {
        if (tileCoordinates.x < 1 || tileCoordinates.x > _sizeX || tileCoordinates.y < 1 || tileCoordinates.y > _sizeY)
            return null;
        int offsetX = _sizeX / 3;
        int offsetY = _sizeY / 3;
        if (tileCoordinates.x <= offsetX)
        {
            if (tileCoordinates.y <= offsetY)
            {
                return _regions[0];
            }
            if (tileCoordinates.y <= offsetY * 2)
            {
                return _regions[1];
            }
            return _regions[2];
        }
        if (tileCoordinates.x <= offsetX * 2)
        {
            if (tileCoordinates.y <= offsetY)
            {
                return _regions[3];
            }if (tileCoordinates.y <= offsetY * 2)
            {
                return _regions[4];
            }
            return _regions[5];
        }

        if (tileCoordinates.y <= offsetY)
        {
            return _regions[6];
        }
        if (tileCoordinates.y <= offsetY * 2)
        {
            return _regions[7];
        }
        return _regions[8];

    }

    private void removeOverlappingWallsAndFloor()
    {
        
        List<Tile> removableWall=new List<Tile>();
        foreach (Tile tile in Wall)
        {
            // if(tile.Coordinates==_coordinatesNotEntrance ||
            //    tile.Coordinates== (_coordinatesNotEntrance+Direction.North.GetDirection()) ||
            //    tile.Coordinates== (_coordinatesNotEntrance+Direction.South.GetDirection()) ||
            //    tile.Coordinates==_coordinatesNotExit ||
            //    tile.Coordinates== (_coordinatesNotExit+Direction.North.GetDirection()) ||
            //    tile.Coordinates== (_coordinatesNotExit+Direction.South.GetDirection())) 
            if(tile.Coordinates==_coordinatesNotEntrance ||
               tile.Coordinates==_coordinatesNotExit) 
                removableWall.Add(tile);
        }
        foreach (Tile tile in removableWall)
        {
            Wall.Remove(tile);
            TileList.Remove(tile);
            Floor.Add(new Tile(assetsCollection.GetTileFromType(AssetType.Floor)[0],tile.Coordinates));
        }
        foreach (Tile wall in Wall)
        { 
            IEnumerable<Tile> enumerationRemovableTile = Floor.Where(e => e.Coordinates == wall.Coordinates);
            List<Tile> removableTile = new List<Tile>();
            foreach (Tile floorTile in enumerationRemovableTile)
            {
                removableTile.Add(floorTile);
            }
            foreach (Tile floorTile in removableTile)
            {
                Floor.Remove(floorTile);
                TileList.Remove(floorTile);
            }
        }
    }

    bool checkOccupiedTile(Vector3Int coordinatesToCheck)
    {
        foreach (Vector3Int coordinates in _occupiedTile)
        {
            if (coordinates == coordinatesToCheck)
                return true;
        }

        return false;
    }

}

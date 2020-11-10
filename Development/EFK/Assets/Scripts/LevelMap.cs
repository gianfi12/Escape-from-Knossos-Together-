using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LevelMap : MonoBehaviourPun
{
    private Tilemap _tilemapFloor;
    private Tilemap _tilemapCorridorFloor;
    private Tilemap _tilemapWall;
    private Tilemap _tilemapObject;
    private Grid _grid;
    [SerializeField] private RoomCollection roomCollection;
    [SerializeField] private int numberOfRoom;
    private List<RoomAbstract> _selectedRooms = new List<RoomAbstract>();
    public const int PaddingRoom = 3;
    private int _seed;

    public int Seed
    {
        get => _seed;
        set => _seed = value;
    }

    [SerializeField] private GameObject playerPrefab;

    public void CreateMapOverNetwork() {
        this.photonView.RPC("SetSeed", RpcTarget.All, Random.Range(0, 10000));
        this.photonView.RPC("CreateMap", RpcTarget.All);
    }

    public void InstantiatePlayersOverNetwork() {
        this.photonView.RPC("InstantiatePlayers", RpcTarget.All);
    }

    [PunRPC]
    public void CreateMap(){
        Random.InitState(_seed);
        InstantiateMapElements();

        RoomGeneration();
        RoomPlacement();
        RoomConnect();


    }

    [PunRPC]
    public void InstantiatePlayers() {
        PlayerControllerMap _playerInstance = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<PlayerControllerMap>();
        int viewID = _playerInstance.GetComponent<PhotonView>().ViewID;
        PlacePlayer(_playerInstance, viewID/1000);
        GameObject.Find("GameManager").GetComponent<GameManager>().SetPlayerInstance(_playerInstance);
    }

    [PunRPC]
    public void SetSeed(int seed) {
        _seed = seed;
    }

    private void InstantiateMapElements()
    {
        var gridObject = new GameObject("Grid");
        _grid = gridObject.AddComponent<Grid>();

        var goFloor = new GameObject("TilemapFloor");
        goFloor.transform.SetParent(_grid.gameObject.transform);
        _tilemapFloor = goFloor.AddComponent<Tilemap>();
        goFloor.AddComponent<TilemapRenderer>();

        NavMeshModifier navMeshModifierFloor = goFloor.AddComponent<NavMeshModifier>();
        navMeshModifierFloor.overrideArea = true;
        navMeshModifierFloor.area = 0; //0 means walkable

        var goWall = new GameObject("TilemapWall");
        goWall.transform.SetParent(_grid.gameObject.transform);
        _tilemapWall = goWall.AddComponent<Tilemap>();
        goWall.AddComponent<TilemapRenderer>();
        goWall.AddComponent<TilemapCollider2D>();
        Rigidbody2D rigidbody2DWall = goWall.AddComponent<Rigidbody2D>();
        rigidbody2DWall.bodyType = RigidbodyType2D.Kinematic;
        goWall.AddComponent<CompositeCollider2D>();
        goWall.GetComponent<TilemapCollider2D>().usedByComposite = true;

        NavMeshModifier navMeshModifierWall = goWall.AddComponent<NavMeshModifier>();
        navMeshModifierWall.overrideArea = true;
        navMeshModifierWall.area = 1; //1 means not walkable

        var goObject = new GameObject("TilemapObject");
        goObject.transform.SetParent(_grid.gameObject.transform);
        _tilemapObject = goObject.AddComponent<Tilemap>();
        TilemapRenderer renderer =  goObject.AddComponent<TilemapRenderer>();
        renderer.sortingLayerName = "Object";

        NavMeshModifier navMeshModifierObject = goWall.AddComponent<NavMeshModifier>();
        navMeshModifierObject.overrideArea = true;
        navMeshModifierObject.area = 1; //1 means not walkable
        
        var goCorridorFloor = new GameObject("TilemapCorridorFloor");
        goCorridorFloor.transform.SetParent(_grid.gameObject.transform);
        _tilemapCorridorFloor = goCorridorFloor.AddComponent<Tilemap>();
        goCorridorFloor.AddComponent<TilemapRenderer>();
    }

    private void RoomConnect()
    {
        for (int i = 0; i < _selectedRooms.Count - 1; i++)
        {
            Connect(_selectedRooms[i],_selectedRooms[i+1]);
        }
    }

    private void Connect(RoomAbstract exitRoom, RoomAbstract entranceRoom)
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
            moovingCoordinatesLeft = ToPaddingRegion(moovingCoordinatesLeft, startingDirection,exitRoom.AssetsCollection,Direction.West);
        }

        //Position the moovingCoordinates from left into the central region before moving on the axis
        moovingCoordinatesLeft = ToCenterRegion(moovingCoordinatesLeft, Direction.West, exitRoom,exitRoom.AssetsCollection);

        //position to the start of the padding zone from right
        List<Vector3Int> moovingCoordinatesRight = entranceRoom.Entrance.Select(x => x.Coordinates).ToList();
        if (arrivalDirection == Direction.North || arrivalDirection == Direction.South)
        {
            moovingCoordinatesRight = ToPaddingRegion(moovingCoordinatesRight, arrivalDirection, exitRoom.AssetsCollection,Direction.East);
        }

        //arrive to the central zone from right
        moovingCoordinatesRight = ToCenterRegion(moovingCoordinatesRight, Direction.East, entranceRoom,exitRoom.AssetsCollection);

        //move on the vertical central axes
        ConnectOnTheCenter(moovingCoordinatesLeft,moovingCoordinatesRight,exitRoom.AssetsCollection);
    }

    private void ConnectOnTheCenter(List<Vector3Int> moovingCoordinatesFromLeft, List<Vector3Int> moovingCoordinatesFromRight, AssetsCollection asset)
    {
        if (moovingCoordinatesFromLeft.Max(x=> x.y) == moovingCoordinatesFromRight.Max(x=>x.y)){
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            }
            if (moovingCoordinatesFromLeft.Count > moovingCoordinatesFromRight.Count)
            {
                //place the walls on the corner of the border
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                for (int i = 0; i < moovingCoordinatesFromLeft.Count-moovingCoordinatesFromRight.Count-1;i++)
                {
                    _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Min(x=>x.y)+i,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                }

            }else if (moovingCoordinatesFromLeft.Count < moovingCoordinatesFromRight.Count)
            {
                for (int i = 0; i < moovingCoordinatesFromRight.Count-moovingCoordinatesFromLeft.Count-1;i++)
                {
                    _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-1,moovingCoordinatesFromLeft.Min(x=>x.y)+i+2,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                }
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Min(x=>x.y)+moovingCoordinatesFromRight.Count-moovingCoordinatesFromLeft.Count,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            }

        }else if (moovingCoordinatesFromLeft.Min(x=> x.y) == moovingCoordinatesFromRight.Min(x=>x.y))
        {
            if (moovingCoordinatesFromLeft.Count > moovingCoordinatesFromRight.Count)
            {
                //place the walls on the corner of the border
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                //place the walls on the top of the border and on the right border
                for (int i = 0; (i < moovingCoordinatesFromLeft.Count); i++)
                {
                    _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                    _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                    if(moovingCoordinatesFromLeft[i].y>moovingCoordinatesFromRight.Max(pos=>pos.y))_tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft[i].y,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                }
            }else if (moovingCoordinatesFromLeft.Count < moovingCoordinatesFromRight.Count)
            {
                //place the walls on the corner of the border
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromRight[0].x-1,moovingCoordinatesFromRight.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                //place the walls on the top of the border and on the right border
                for (int i = 0; (i < moovingCoordinatesFromRight.Count); i++)
                {
                    _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                    _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromRight[0].x+i,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                    if(moovingCoordinatesFromRight[i].y>moovingCoordinatesFromLeft.Max(pos=>pos.y))_tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromRight[0].x-1,moovingCoordinatesFromRight[i].y,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                }
            }

        }else if (moovingCoordinatesFromLeft.Min(x=> x.y) == moovingCoordinatesFromRight.Min(x=>x.y) && moovingCoordinatesFromLeft.Max(x=> x.y) == moovingCoordinatesFromRight.Max(x=>x.y))
        {
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(
                    new Vector3Int(moovingCoordinatesFromLeft[0].x - i, moovingCoordinatesFromLeft.Max(x => x.y) + 1,
                        0), asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                _tilemapWall.SetTile(
                    new Vector3Int(moovingCoordinatesFromLeft[0].x - i, moovingCoordinatesFromLeft.Min(x => x.y) - 1,
                        0), asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            }
        }else if (moovingCoordinatesFromLeft.Max(x=> x.y) > moovingCoordinatesFromRight.Max(x=>x.y))//means that from the left we are coming more higher
        {
            int minY = moovingCoordinatesFromLeft.Min(x=>x.y);
            //place the walls on the corner of the border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
            //place the walls on the top of the border and on the right border
            for (int i = 0; (i < moovingCoordinatesFromLeft.Count); i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                if(moovingCoordinatesFromLeft[i].y>moovingCoordinatesFromRight.Max(pos=>pos.y))_tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft[i].y,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
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
                    _tilemapCorridorFloor.SetTile(moovingCoordinatesFromLeft[j]+Direction.South.GetDirection(),asset.GetTileFromType(AssetType.Floor)[0]);
                    moovingCoordinatesFromLeft[j] += Direction.South.GetDirection();
                }
                if(i>moovingCoordinatesFromRight.Min(pos=>pos.y)+moovingCoordinatesFromRight.Count) _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Max(pos=>pos.x)+1,moovingCoordinatesFromLeft[0].y,0), asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
            }
            //place the wall now in the bootom corner and bottom border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            //place the walls on the top of the border and on the right border
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[i].x,moovingCoordinatesFromLeft.Max(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            }
        }
        else //means that we have to go up
        {
            int maxY = moovingCoordinatesFromLeft.Max(pos=>pos.y);
            //place the walls on the corner of the border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            //place the walls on the bottom of the border and on the right border
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x-i,moovingCoordinatesFromLeft.Min(x=>x.y)-1,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                if(moovingCoordinatesFromLeft[i].y<moovingCoordinatesFromRight.Min(pos=>pos.y))_tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[0].x+1,moovingCoordinatesFromLeft[i].y,0),asset.GetTileFromType(AssetType.WallBottomRight)[0]);
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
                    _tilemapCorridorFloor.SetTile(moovingCoordinatesFromLeft[j]+Direction.North.GetDirection(),asset.GetTileFromType(AssetType.Floor)[0]);
                    moovingCoordinatesFromLeft[j] += Direction.North.GetDirection();
                }
                if(i<moovingCoordinatesFromRight.Max(pos=>pos.y)-moovingCoordinatesFromRight.Count) _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Max(pos=>pos.x)+1,moovingCoordinatesFromLeft[0].y,0), asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                 _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
            }
            //place the wall now in top corner and upper border
            _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft.Min(pos=>pos.x)-1,moovingCoordinatesFromLeft[0].y+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
            //place the walls on the top of the border and on the right border
            for (int i = 0; i < moovingCoordinatesFromLeft.Count; i++)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinatesFromLeft[i].x,moovingCoordinatesFromLeft.Max(x=>x.y)+1,0),asset.GetTileFromType(AssetType.WallTopLeft)[0]);
            }
        }
    }

    private List<Vector3Int> ToCenterRegion(List<Vector3Int> moovingCoordinates, Direction direction, RoomAbstract room, AssetsCollection asset)
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
                _tilemapCorridorFloor.SetTile(moovingCoordinates[j]+direction.GetDirection(),asset.GetTileFromType(AssetType.Floor)[0]);
                moovingCoordinates[j] += direction.GetDirection();
            }
            //here we place the wall only before the ending of the padding zone, the wall for the corner has to be decided when we know if we will go up or down
            if (i < room.DisplacementX + room.RequiredWidthSpace + PaddingRoom-1)
            {
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinates[0].x,moovingCoordinates.Max(pos=>pos.y)+1,0), asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                _tilemapWall.SetTile(new Vector3Int(moovingCoordinates[0].x,moovingCoordinates.Min(pos=>pos.y)-1,0), asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            }
        }
        //it returns the updated value of the moving coordinates up to now
        return moovingCoordinates;
    }

    private List<Vector3Int> ToPaddingRegion(List<Vector3Int> moovingCoordinates, Direction startingDirection, AssetsCollection asset,Direction nextDirection)
    {
        //Move down for the size necessary to make a corner plus some padding
        for (int i = 0; i < moovingCoordinates.Count+PaddingRoom; i++)
        {
            //Put the floor as long as your are moving up or down
            for (int j = 0; j < moovingCoordinates.Count; j++)
            {
                _tilemapCorridorFloor.SetTile(moovingCoordinates[j]+startingDirection.GetDirection(),asset.GetTileFromType(AssetType.Floor)[0]);
                moovingCoordinates[j] += startingDirection.GetDirection();
            }
            //Add also the wall, but if you are going:
            //- to South you have to put also the wall after the padding zone before doing the cornet on the left
            //- to North you have to put also the wall after the padding zone before doing the cornet on the right
            if (startingDirection == Direction.South)
            {
                if (nextDirection == Direction.West)
                {
                    if (i < PaddingRoom)
                        _tilemapWall.SetTile(
                            new Vector3Int(moovingCoordinates.Max(x => x.x) + 1, moovingCoordinates[0].y, 0),
                            asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                    _tilemapWall.SetTile(
                        new Vector3Int(moovingCoordinates.Min(x => x.x) - 1, moovingCoordinates[0].y, 0),
                        asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                }
                else
                {
                    _tilemapWall.SetTile(
                            new Vector3Int(moovingCoordinates.Max(x => x.x) + 1, moovingCoordinates[0].y, 0),
                            asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                    if (i < PaddingRoom) _tilemapWall.SetTile(
                        new Vector3Int(moovingCoordinates.Min(x => x.x) - 1, moovingCoordinates[0].y, 0),
                        asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                }
            }
            else
            {
                if(nextDirection==Direction.East){
                    if (i < PaddingRoom)
                        _tilemapWall.SetTile(
                            new Vector3Int(moovingCoordinates.Min(x => x.x) - 1, moovingCoordinates[0].y, 0),
                            asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                    _tilemapWall.SetTile(
                        new Vector3Int(moovingCoordinates.Max(x => x.x) + 1, moovingCoordinates[0].y, 0),
                        asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                }else
                {
                    _tilemapWall.SetTile(
                            new Vector3Int(moovingCoordinates.Min(x => x.x) - 1, moovingCoordinates[0].y, 0),
                            asset.GetTileFromType(AssetType.WallTopLeft)[0]);
                    if (i < PaddingRoom)_tilemapWall.SetTile(
                        new Vector3Int(moovingCoordinates.Max(x => x.x) + 1, moovingCoordinates[0].y, 0),
                        asset.GetTileFromType(AssetType.WallBottomRight)[0]);
                }

            }
        }
        //Put the tile of the wall also on the corner
        if(startingDirection==Direction.South) {
            if(nextDirection==Direction.West) _tilemapWall.SetTile(
            new Vector3Int(moovingCoordinates.Min(x => x.x)-1, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
            asset.GetTileFromType(AssetType.WallBottomRight)[0]);
            else _tilemapWall.SetTile(
                new Vector3Int(moovingCoordinates.Max(x => x.x)+1, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
                asset.GetTileFromType(AssetType.WallBottomRight)[0]);
        }
        else {
            if(nextDirection==Direction.East) _tilemapWall.SetTile(
            new Vector3Int(moovingCoordinates.Max(x => x.x)+1, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
            asset.GetTileFromType(AssetType.WallTopLeft)[0]);
            else _tilemapWall.SetTile(
                new Vector3Int(moovingCoordinates.Min(x => x.x)-1, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
                asset.GetTileFromType(AssetType.WallTopLeft)[0]);
        }

        //Put the tile of the wall also on the other part of the cornet before changing and rotating the moving direction in the West direction
        foreach (Vector3Int pos in moovingCoordinates)
        {
            _tilemapWall.SetTile(
                new Vector3Int(pos.x, moovingCoordinates[0].y, 0)+startingDirection.GetDirection(),
                startingDirection==Direction.North? asset.GetTileFromType(AssetType.WallTopLeft)[0]:asset.GetTileFromType(AssetType.WallBottomRight)[0]);
        }
        //Rotate the moving directions in order to continue and go to West in the next function
        for (int i = 0; i < moovingCoordinates.Count; i++)
        {
            if (nextDirection == Direction.West)
            {
                if (startingDirection == Direction.South)
                    moovingCoordinates[i] = new Vector3Int(moovingCoordinates.Max(x=>x.x), moovingCoordinates[i].y + i, 0);
                else
                    moovingCoordinates[i] = new Vector3Int(moovingCoordinates.Max(x=>x.x), moovingCoordinates[i].y - i, 0);
            }
            else
            {
                if (startingDirection == Direction.North)
                    moovingCoordinates[i] = new Vector3Int(moovingCoordinates.Min(x=>x.x), moovingCoordinates[i].y - i, 0);
                else moovingCoordinates[i] = new Vector3Int(moovingCoordinates.Min(x=>x.x), moovingCoordinates[i].y + i, 0);
            }
            //if(startingDirection==Direction.North) moovingCoordinates[i] += new Vector3Int(-1,0,0);
        }
        //return the updated coordinates
        return moovingCoordinates;
    }

    private void RoomPlacement()
    {
        foreach (RoomAbstract room in _selectedRooms)
        {
            Vector3Int coordinates = _selectedRooms.IndexOf(room)==0 ? new Vector3Int(0,0,0) : RandomCoordinates(room,_selectedRooms[_selectedRooms.IndexOf(room)-1]);
            //while (!FreeSpace(room, coordinates = RandomCoordinates())); //start placing the starting room
            room.PlaceRoom(_tilemapFloor,_tilemapWall,_tilemapObject,coordinates);
        }
    }

    private Vector3Int RandomCoordinates(RoomAbstract nextRoom,RoomAbstract previousRoom)
    {
        //int randomX = Random.Range(previousRoom.DisplacementX+previousRoom.RequiredWidthSpace+PaddingRoom+previousRoom.Exit.Count+PaddingRoom, PaddingRoom + previousRoom.Exit.Count+ previousRoom.DisplacementX+previousRoom.RequiredWidthSpace +nextRoom.RequiredWidthSpace+PaddingRoom);
        int randomX = previousRoom.DisplacementX + previousRoom.RequiredWidthSpace + PaddingRoom +
                      previousRoom.Exit.Count + PaddingRoom;
        int randomY = Random.Range(-10,10);
        return new Vector3Int(randomX,randomY,0);
    }


    private void RoomGeneration()
    {
        _selectedRooms.Add(roomCollection.StartingRoomPlayer1);
        roomCollection.StartingRoomPlayer1.Generate();
        if (numberOfRoom*2 > roomCollection.Rooms.Count)
        {
            throw new InvalidDataException("Not enough Room.");
        }
        for (int i = 0; i < numberOfRoom; i++)
        {
            RoomAbstract room = SelectRoom();
            room.Generate();
            _selectedRooms.Add(room);

        }
        _selectedRooms.Add(roomCollection.FinalRoom);
        roomCollection.FinalRoom.Generate();
        for (int i = 0; i < numberOfRoom; i++)
        {
            RoomAbstract room = SelectRoom();
            room.Generate();
            _selectedRooms.Add(room);
        }
        _selectedRooms.Add(roomCollection.StartingRoomPlayer2);
        roomCollection.StartingRoomPlayer2.Generate();

    }

    private RoomAbstract SelectRoom()
    {
        int index = Random.Range(0, roomCollection.Rooms.Count);
        while (_selectedRooms.Contains(roomCollection.Rooms[index]))
        {
            index = Random.Range(0, roomCollection.Rooms.Count);;
        }
        return roomCollection.Rooms[index];
    }

    public void PlacePlayer(PlayerControllerMap player, int playerID)
    {
        if (playerID == 1)
            player.SetLocation(_tilemapFloor.layoutGrid.CellToWorld(_selectedRooms[0].Spawn[0].Coordinates) +
                               new Vector3Int(1, 1, 0));
        else
            player.SetLocation(
                _tilemapFloor.layoutGrid.CellToWorld(_selectedRooms[_selectedRooms.Count - 1].Spawn[0].Coordinates) +
                new Vector3Int(1, 1, 0));

    }
}

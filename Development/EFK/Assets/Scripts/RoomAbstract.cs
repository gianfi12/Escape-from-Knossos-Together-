using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class RoomAbstract : MonoBehaviour
{
    [SerializeField] protected AssetsCollection assetsCollection;
    [SerializeField] private bool hasTwin;
    [SerializeField] private RoomAbstract twinRoom;
    [SerializeField] private GameObject doorExit;
    [SerializeField] private GameObject doorEntrance;

    public readonly List<Tile> Entrance = new List<Tile>();
    public readonly List<Tile> Exit = new List<Tile>();
    public readonly List<Tile> Wall = new List<Tile>();
    public readonly List<Tile> Floor = new List<Tile>();
    public readonly List<Transform> Object = new List<Transform>();
    public readonly List<Tile> Spawn = new List<Tile>();
    public readonly List<Tile> Decoration = new List<Tile>();
    
    public readonly List<Tile> TileList = new List<Tile>();
    
    protected int _requiredWidthSpace;
    protected int _displacementX, _displacementY;
    protected int _lowestX;
    protected int _lowestY;
    
    public int RequiredWidthSpace => _requiredWidthSpace;

    public int DisplacementX => _displacementX;

    public int DisplacementY => _displacementY;

    public AssetsCollection AssetsCollection => assetsCollection;

    public abstract void Generate(int seed);

    public abstract void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall, Tilemap tilemapObject,Tilemap tilemapDecoration, Vector3Int coordinates);

    public bool HasTwin => hasTwin;

    public RoomAbstract TwinRoom => twinRoom;
}

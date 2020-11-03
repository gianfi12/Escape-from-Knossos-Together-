using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private Tilemap tilemapFloor;
    [SerializeField] private Tilemap tilemapObject;
    [SerializeField] private Tilemap tilemapExit;
    [SerializeField] private Tilemap tilemapEntrance;
    [SerializeField] private Tilemap tilemapWall;
    [SerializeField] private Tilemap tilemapSpawn;
    [SerializeField] private AssetsCollection assetsCollection;
    
    public readonly List<Tile> Entrance = new List<Tile>();
    public readonly List<Tile> Exit = new List<Tile>();
    public readonly List<Tile> Wall = new List<Tile>();
    public readonly List<Tile> Floor = new List<Tile>();
    public readonly List<Tile> Object = new List<Tile>();
    public readonly List<Tile> Spawn = new List<Tile>();
    public readonly List<Tile> TileList = new List<Tile>();

    private int _requiredWidthSpace;
    private int _displacementX, _displacementY;
    private int _lowestX;
    private int _higherX;
    private int _lowestY;
    private bool _firstTile = true;

    public void Generate()
    {
        IterateTilemap(tilemapFloor,Floor);
        IterateTilemap(tilemapWall,Wall);
        IterateTilemap(tilemapObject,Object);
        IterateTilemap(tilemapExit,Exit);
        IterateTilemap(tilemapEntrance,Entrance);
        IterateTilemap(tilemapSpawn,Spawn);
        _requiredWidthSpace = _higherX - _lowestX;
    }

    private void IterateTilemap(Tilemap tilemap, List<Tile> list)
    {
        for (int n = tilemap.cellBounds.xMin; n < tilemap.cellBounds.xMax; n++)
        {
            for (int p = tilemap.cellBounds.yMin; p < tilemap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)tilemap.transform.position.y));
                if (tilemap.HasTile(localPlace))
                {
                    Tile tile= new Tile(tilemap.GetTile(localPlace), localPlace);
                    if (_firstTile)
                    {
                        _firstTile = false;
                        _lowestX = tile.Coordinates.x;
                        _lowestY = tile.Coordinates.y;
                        _higherX = tile.Coordinates.x;
                    }
                    else if (_lowestX > tile.Coordinates.x)
                    {
                        _lowestX = tile.Coordinates.x;
                        _lowestY = tile.Coordinates.y;
                    }else if (_lowestX == tile.Coordinates.x && _lowestY>tile.Coordinates.y)
                    {
                        _lowestY = tile.Coordinates.y;
                    }

                    if (_higherX < tile.Coordinates.x) _higherX = tile.Coordinates.x;
                    
                    list.Add(tile);
                    TileList.Add(tile);
                }
            }
        }
    }

    public void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall, Tilemap tilemapObject, Vector3Int coordinates)
    {
        _displacementX = coordinates.x;
        _displacementY = coordinates.y;
        foreach (var tile in TileList)
        {
            tile.Coordinates = tile.Coordinates - new Vector3Int(_lowestX,_lowestY,0) + coordinates; 
            if (Wall.Contains(tile)) 
                tilemapWall.SetTile(tile.Coordinates,tile.TileBase);
            else if (Object.Contains(tile))
                tilemapObject.SetTile(tile.Coordinates,tile.TileBase);
            else
                tilemapFloor.SetTile(tile.Coordinates,tile.TileBase);
        }
    }

    public AssetsCollection AssetsCollection => assetsCollection;

    public int RequiredWidthSpace => _requiredWidthSpace;

    public int DisplacementX => _displacementX;

    public int DisplacementY => _displacementY;

    public int LowestX => _lowestX;
}

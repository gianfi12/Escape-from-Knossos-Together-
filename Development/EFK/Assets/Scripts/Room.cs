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
    [SerializeField] private Asset asset;
    [SerializeField] private TileBase _base;
    
    public readonly List<Tile> Entrance = new List<Tile>();
    public readonly List<Tile> Exit = new List<Tile>();
    public readonly List<Tile> Wall = new List<Tile>();
    public readonly List<Tile> Floor = new List<Tile>();
    public readonly List<Tile> Object = new List<Tile>();
    public readonly List<Tile> Spawn = new List<Tile>();
    public readonly List<Tile> TileList = new List<Tile>();

    public void Generate()
    {
        IterateTilemap(tilemapFloor,Floor);
        IterateTilemap(tilemapWall,Wall);
        IterateTilemap(tilemapObject,Object);
        IterateTilemap(tilemapExit,Exit);
        IterateTilemap(tilemapEntrance,Entrance);
        IterateTilemap(tilemapSpawn,Spawn);
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
                    list.Add(tile);
                    TileList.Add(tile);
                }
            }
        }
    }

    public void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall, Tilemap tilemapObject, Vector3Int coordinates)
    {
        foreach (var tile in TileList)
        {
            tile.Coordinates = tile.Coordinates + coordinates;
            if (Wall.Contains(tile)) 
                tilemapWall.SetTile(tile.Coordinates,tile.TileBase);
            else if (Object.Contains(tile))
                tilemapObject.SetTile(tile.Coordinates,tile.TileBase);
            else
                tilemapFloor.SetTile(tile.Coordinates,tile.TileBase);
        }
    }
}

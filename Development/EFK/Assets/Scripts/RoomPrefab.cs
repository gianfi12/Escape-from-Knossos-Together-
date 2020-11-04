using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPrefab : RoomAbstract
{
    [SerializeField] private Tilemap tilemapFloor;
    [SerializeField] private Tilemap tilemapObject;
    [SerializeField] private Tilemap tilemapExit;
    [SerializeField] private Tilemap tilemapEntrance;
    [SerializeField] private Tilemap tilemapWall;
    [SerializeField] private Tilemap tilemapSpawn;

    private int _lowestX;
    private int _higherX;
    private int _lowestY;
    private bool _firstTile = true;
    
    public override void Generate()
    {
        IterateTilemap(tilemapFloor,Floor);
        IterateTilemap(tilemapWall,Wall);
        IterateTilemapObject(tilemapObject,Object);
        IterateTilemap(tilemapExit,Exit);
        IterateTilemap(tilemapEntrance,Entrance);
        IterateTilemap(tilemapSpawn,Spawn);
        _requiredWidthSpace = _higherX - _lowestX;
    }

    private void IterateTilemapObject(Tilemap tilemap, List<Transform> gameObjects)
    {
        for (int i = 0; i < tilemap.transform.childCount; i++)
        {
            gameObjects.Add(tilemap.transform.GetChild(i));
        }
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

    public override void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall, Tilemap tilemapObject, Vector3Int coordinates)
    {
        _displacementX = coordinates.x;
        _displacementY = coordinates.y;
        foreach (var tile in TileList)
        {
            tile.Coordinates = tile.Coordinates - new Vector3Int(_lowestX,_lowestY,0) + coordinates; 
            if (Wall.Contains(tile)) 
                tilemapWall.SetTile(tile.Coordinates,tile.TileBase);
            // else if (Object.Contains(tile))
            //     tilemapObject.SetTile(tile.Coordinates,tile.TileBase);
            else
                tilemapFloor.SetTile(tile.Coordinates,tile.TileBase);
        }

        foreach (Transform transform1 in Object)
        {
            GameObject gameObject = Instantiate(transform1.gameObject);
            transform1.position = transform1.position - new Vector3Int(_lowestX, _lowestY, 0) + coordinates;
            //gameObject.transform.parent = tilemapObject.transform;
            //PrefabBrush prefabBrush = ScriptableObject.CreateInstance<PrefabBrush>();
            //prefabBrush.Paint(tilemapObject,transform1.gameObject, new Vector3Int((int)transform1.position.x,(int)transform1.position.y,(int)transform1.position.z));
        }
    }
}

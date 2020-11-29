using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPrefab : RoomAbstract
{
    [SerializeField] private Tilemap tilemapFloor;
    // [SerializeField] private Tilemap tilemapObject;
    [SerializeField] private Tilemap tilemapExit;
    [SerializeField] private Tilemap tilemapEntrance;
    [SerializeField] private Tilemap tilemapWall;
    [SerializeField] private Tilemap tilemapSpawn;
    [SerializeField] private Tilemap tilemapDecoration;

    private int _higherX;
    private bool _firstTile = true;
    
    public override void Generate(int seed)
    {
        IterateTilemap(tilemapFloor,Floor);
        IterateTilemap(tilemapWall,Wall);
        IterateTilemap(tilemapDecoration,Decoration);
        // IterateTilemapObject(tilemapObject,Object);
        IterateTilemap(tilemapExit,Exit);
        IterateTilemap(tilemapEntrance,Entrance);
        IterateTilemap(tilemapSpawn,Spawn);
        _requiredWidthSpace = _higherX - _lowestX;
    }

    // private void IterateTilemapObject(Tilemap tilemap, List<ObjectInRoom> gameObjects)
    // {
    //     for (int i = 0; i < tilemap.transform.childCount; i++)
    //     {
    //         Transform selectedObjectTransform = tilemap.transform.GetChild(i);
    //         Vector3Int position = new Vector3Int((int)selectedObjectTransform.position.x,(int)selectedObjectTransform.position.y,0);
    //         ObjectInRoom objectInRoom = new ObjectInRoom(position,selectedObjectTransform);
    //         gameObjects.Add(objectInRoom);
    //     }
    // }

    protected void IterateTilemap(Tilemap tilemap, List<Tile> list)
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
    
    public override void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall,Tilemap tilemapDecoration, Vector3Int coordinates) 
    {
        _displacementX = coordinates.x;
        _displacementY = coordinates.y;
        foreach (var tile in TileList)
        {
            tile.Coordinates = tile.Coordinates - new Vector3Int(_lowestX,_lowestY,0) + coordinates; 
            if (Wall.Contains(tile)) 
                tilemapWall.SetTile(tile.Coordinates,tile.TileBase);
            else if(Decoration.Contains(tile))
                tilemapDecoration.SetTile(tile.Coordinates,tile.TileBase);
            else
                tilemapFloor.SetTile(tile.Coordinates,tile.TileBase);
        }
        
        Transform room = Instantiate(gameObject).transform;
        Transform child;
        for (int i = 0; i < room.transform.childCount; i++)
        {
            if ((child = room.GetChild(i)).name == "Grid")
            {
                Destroy(child.gameObject);
            }
        }
        
        room.localPosition = coordinates +(transform.position - new Vector3(_lowestX,_lowestY,0));

    }
}

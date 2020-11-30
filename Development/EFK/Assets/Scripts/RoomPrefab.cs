using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPrefab : RoomAbstract
{
    [SerializeField] private Tilemap tilemapFloor;
    [SerializeField] private Tilemap tilemapExit;
    [SerializeField] private Tilemap tilemapEntrance;
    [SerializeField] private Tilemap tilemapWall;
    [SerializeField] private Tilemap tilemapSpawn;
    [SerializeField] private Tilemap tilemapDecoration;

    private int _higherX;
    private bool _firstTile = true;
    
    public override void Generate(int seed,bool isPlayer2)
    {
        IterateTilemap(tilemapFloor,Floor);
        IterateTilemap(tilemapWall,Wall);
        IterateTilemap(tilemapDecoration,Decoration);
        IterateTilemap(tilemapExit,Exit);
        IterateTilemap(tilemapEntrance,Entrance);
        IterateTilemap(tilemapSpawn,Spawn);
        if (isPlayer2 && useSameEntrance)
        {
            List<Tile> temp = Entrance;
            Entrance = new List<Tile>();
            Entrance.AddRange(Exit);
            Exit = new List<Tile>();
            Exit.AddRange(temp);
        }
        _requiredWidthSpace = _higherX - _lowestX;
        
    }

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

    public override void PlaceObject(Vector3Int coordinates)
    {
        _displacementX = coordinates.x;
        _displacementY = coordinates.y;
        
                
        Transform room = Instantiate(objectsParent).transform;
        // Transform child;
        // for (int i = 0; i < room.transform.childCount; i++)
        // {
        //     if ((child = room.GetChild(i)).name == "Grid")
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
        
        room.localPosition = coordinates +(transform.position - new Vector3(_lowestX,_lowestY,0));
    }

    public override void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall,Tilemap tilemapDecoration) 
    {

        foreach (var tile in TileList)
        {
            tile.Coordinates = tile.Coordinates - new Vector3Int(_lowestX,_lowestY,0) + new Vector3Int(_displacementX,_displacementY,0); 
            if (Wall.Contains(tile)) 
                tilemapWall.SetTile(tile.Coordinates,tile.TileBase);
            else if(Decoration.Contains(tile))
                tilemapDecoration.SetTile(tile.Coordinates,tile.TileBase);
            else
                tilemapFloor.SetTile(tile.Coordinates,tile.TileBase);
        }
        
    }
    
    
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum Direction
{
    North,
    South,
    West,
    East
}

public static class DirectionExtensions
{
    private static Vector3Int[] _direction =
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(1,0,0)
    };
    private static Direction[] _opposite =
    {
        Direction.South,
        Direction.North,
        Direction.East,
        Direction.West
    };

    public static Vector3Int GetDirection(this Direction direction)
    {
        return _direction[(int) direction];

    }
    public static Direction GetOpposite(this Direction direction)
    {
        return _opposite[(int) direction];
    }
    
    public static Direction FindDirection(Tile first,Tile second,Tilemap tilemap)
    {
        if (first.Coordinates.x == second.Coordinates.x)
        {
            TileBase checkTile = tilemap.GetTile(first.Coordinates + new Vector3Int(1, 0, 0));
            if (checkTile != null)
                return Direction.West;
            return Direction.East;
        }
        else
        {
            TileBase checkTile = tilemap.GetTile(first.Coordinates + new Vector3Int(0, 1, 0));
            if (checkTile != null)
                return Direction.South;
            return Direction.North;
        }
    }
    
    public static Direction FindDirection(Tile tile,Tilemap tilemap)
    {
        TileBase checkTile = tilemap.GetTile(tile.Coordinates + Direction.South.GetDirection());
        if (!(checkTile is null)) return Direction.North;
        checkTile = tilemap.GetTile(tile.Coordinates + Direction.North.GetDirection());
        if (!(checkTile is null)) return Direction.South;
        checkTile = tilemap.GetTile(tile.Coordinates + Direction.East.GetDirection());
        if (!(checkTile is null)) return Direction.West;
        return Direction.East;
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum Direction
{
    North,
    South,
    East,
    West
}

public static class DirectionExtensions
{
    private static Direction[] _opposite =
    {
        Direction.South,
        Direction.North,
        Direction.West,
        Direction.East
    };

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
                return Direction.East;
            return Direction.West;
        }
        else
        {
            TileBase checkTile = tilemap.GetTile(first.Coordinates + new Vector3Int(0, 1, 0));
            if (checkTile != null)
                return Direction.South;
            return Direction.North;
        }
    }
}
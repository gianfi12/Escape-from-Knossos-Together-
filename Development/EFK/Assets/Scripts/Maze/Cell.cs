using System.Collections.Generic;
using UnityEngine;

//A cell has a dimension of 2x2 tile
public class Cell
{
    private Vector2Int _position; //is the position of the bottom left tile
    private Room _room;
    private Edge[] _edges={null,null,null,null};

    public Cell(Vector2Int position,Room room)
    {
        _position = position;
        _room = room;
    }
    
    public Vector2Int Position => _position;

    public Room Room
    {
        get => _room;
        set => _room = value;
    }

    public Edge GetEdge(Direction direction)
    {
        return _edges[(int) direction];
    }

    public void SetEdge(Direction direction, Edge edge)
    {
        _edges[(int) direction] = edge;
    }
}

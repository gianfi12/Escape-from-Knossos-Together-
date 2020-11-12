using System;
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

    public bool IsAdjacent(Room neighbor)
    {
        foreach (Edge edge in _edges)
        {
            if (!(edge.GetOther(this) is null) && edge.GetOther(this).Room == neighbor) return true; 
        }
        return false;
    }

    public Edge GetAdjacentEdge(Room neighbor)
    {
        foreach (Edge edge in _edges)
        {
            if (!(edge.GetOther(this) is null) && edge.GetOther(this).Room == neighbor) return edge; 
        }
        return null;
    }

    public void ReshapeCell(Room mergedRoom)
    {
        foreach (Direction direction in (Direction[]) Enum.GetValues(typeof(Direction)))
        {
            Edge edge = this.GetEdge(direction);
            if (edge.GetOther(this)!=null && mergedRoom.GetCellsList().Contains(edge.GetOther(this)))
            {
                edge.EdgeType = Edge.EdgeTypes.Passage;
            }
        }
    }
}

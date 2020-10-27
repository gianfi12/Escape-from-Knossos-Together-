using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellAbstract : MonoBehaviour
{
    protected IntVector2 _coordinates;
    protected CellEdgeAbstract[] edges = new CellEdgeAbstract[MazeDirections.count];
    protected int initializedEdge = 0;
    private float width;
    private float height;
    protected RoomAbstract room;

    public IntVector2 Coordinates
    {
        get => _coordinates;
        set => _coordinates = value;
    }

    public RoomAbstract Room
    {
        get => room;
        set => room = value;
    }

    public float Width
    {
        get => width;
        set => width = value;
    }

    public float Height
    {
        get => height;
        set => height = value;
    }

    private void Awake()
    {
        height = GetComponent<SpriteRenderer>().bounds.size.y;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    public void Initialize(RoomAbstract room)
    {
        this.room = room;
        transform.GetComponent<Renderer>().material = room.Settings.FloorMaterial;
    }

    public CellEdgeAbstract GetEdge(MazeDirections.MazeDirection direction)
    {
        return edges[(int) direction];
    }
    public void SetEdge(MazeDirections.MazeDirection direction, MazeCellEdge edge)
    {
        if(initializedEdge>=MazeDirections.count)
            throw new System.InvalidOperationException("Edges already setted");
        edges[(int) direction] = edge;
        initializedEdge++;

    }
    public MazeDirections.MazeDirection RandomUninitializedDirection() {
        int skips = Random.Range(0, MazeDirections.count - initializedEdge);
        for (int i = 0; i < MazeDirections.count; i++) {
            if (edges[i] == null) {
                if (skips == 0) {
                    return (MazeDirections.MazeDirection)i;
                }
                skips -= 1;
            }
        }

        throw new System.InvalidOperationException("Edges already setted");
    }
    
    public bool IsFullyInitialized (){
        return initializedEdge == MazeDirections.count;
    }
}

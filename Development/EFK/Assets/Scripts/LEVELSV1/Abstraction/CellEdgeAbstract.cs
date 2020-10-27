using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CellEdgeAbstract : MonoBehaviour
{
    public CellAbstract cell, otherCell;
    public MazeDirections.MazeDirection direction;
    protected float tickness;

    virtual protected void Awake()
    {
        tickness = GetComponent<Renderer>().bounds.size.y;
    }

    public abstract void Initialize(CellAbstract cellAbstract, CellAbstract otherCellAbstract,
        MazeDirections.MazeDirection direction);
}

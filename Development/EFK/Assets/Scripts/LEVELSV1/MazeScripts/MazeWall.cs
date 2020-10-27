using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWall : MazeCellEdge
{
    public void Initialize(MazeCell cellAbstract, MazeCell otherCellAbstract, MazeDirections.MazeDirection direction)
    {
        base.Initialize(cellAbstract, otherCellAbstract, direction);
        transform.GetComponent<Renderer>().material = cellAbstract.Room.Settings.WallMaterial;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MazeCellEdge : CellEdgeAbstract
{
    public override void Initialize(CellAbstract cellAbstract, CellAbstract otherCellAbstract, MazeDirections.MazeDirection direction)
    {
        this.cell = cellAbstract;
        this.otherCell = otherCellAbstract;
        this.cell.SetEdge(direction,this);    
        transform.parent = cellAbstract.transform;
        transform.position = cellAbstract.transform.position;
        transform.localRotation = MazeDirections.ToRotation(direction);
        transform.position += Vector3.Scale(new Vector3(this.cell.Width/2-tickness/2,this.cell.Height/2-tickness/2,1f),MazeDirections.ToPosition(direction));
    }
}

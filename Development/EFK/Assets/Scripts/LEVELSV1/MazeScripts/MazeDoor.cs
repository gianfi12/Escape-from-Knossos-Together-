using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeDoor : MazePassage
{
    [SerializeField] private Transform _hinge;
    [SerializeField] private Transform _door;
    [SerializeField] private Transform _doorArea;

    protected override void Awake()
    {
        tickness = transform.GetChild(0).GetComponent<Renderer>().bounds.size.y;
    }

    public void Initialize(MazeCell cellAbstract, MazeCell otherCellAbstract, MazeDirections.MazeDirection direction)
    {
        base.Initialize(cellAbstract, otherCellAbstract, direction);
        if (OtherSideDoor() != null)
        {
            transform.rotation *= Quaternion.Euler(0f, 0f, 180f);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != _hinge && child != _door && child != _doorArea)
            {
                child.GetComponent<Renderer>().material = cellAbstract.Room.Settings.WallMaterial;
            }
        }
    }

    private MazeDoor OtherSideDoor()
    {
        return otherCell.GetEdge(MazeDirections.GetOpposite(direction)) as MazeDoor;
    }
}

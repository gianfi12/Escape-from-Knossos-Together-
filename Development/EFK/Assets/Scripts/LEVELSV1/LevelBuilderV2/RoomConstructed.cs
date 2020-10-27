using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomConstructed",menuName="EFK/RoomConstructed",order=1)]
public class RoomConstructed : RoomAbstract
{
    public int width, height;
    private IntVector2 coordinates;
    public Cell cellPrefab;
    private GameObject room;

    public override GameObject Initialize()
    {
        room = new GameObject();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                CreateCell(new IntVector2(i,j));
            }
            
        }

        return room;
    }
    
    private Cell CreateCell(IntVector2 coordinates)
    {
        Cell newCell = Instantiate(cellPrefab);
        cells.Add(newCell);
        newCell.Coordinates = coordinates;
        newCell.name = "Maze Cell " + newCell.Coordinates.x + ", " + newCell.Coordinates.y;
        Transform newCellTransform = newCell.transform;
        newCellTransform.parent = room.transform; //this makes the cell a child of the maze object
        float realX = (newCell.Coordinates.y - (float)width / 2)*newCell.Width;
        float realY = (-newCell.Coordinates.x + (float)height / 2)*newCell.Height;
        newCellTransform.localPosition = new Vector3(realX,realY,0f);
        return newCell;
    }
}

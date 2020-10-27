using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRoom : RoomAbstract
{
    public void Assimilate(MazeRoom room)
    {
        cells.AddRange(room.cells);
    }

    public override GameObject Initialize()
    {
        throw new System.NotImplementedException();
    }
}

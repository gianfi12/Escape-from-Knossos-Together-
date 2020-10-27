using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelBuilderAbstract : MonoBehaviour
{ 
    public abstract void Generate();

    public abstract CellAbstract GetCell(IntVector2 coordinates);

    public abstract IntVector2 RandomCoordinates();
}

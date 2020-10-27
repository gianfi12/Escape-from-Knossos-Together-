using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomSettingsAbstract
{
    public Material floorMaterial, wallMaterial;

    public Material FloorMaterial
    {
        get => floorMaterial;
        set => floorMaterial = value;
    }

    public Material WallMaterial
    {
        get => wallMaterial;
        set => wallMaterial = value;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSeedInfo : MonoBehaviour
{

    private int _seed;

    public void GenerateMapSeed()
    {
        _seed = Random.Range(0, 10000);
    }

    public int GetSeed()
    {
        return _seed;
    }
}
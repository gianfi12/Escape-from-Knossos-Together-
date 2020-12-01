using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsContainer : MonoBehaviour
{
    private int seed;
    public int Seed => seed;

    public void SetSeed(int seed) {
        this.seed = seed;
    }
}

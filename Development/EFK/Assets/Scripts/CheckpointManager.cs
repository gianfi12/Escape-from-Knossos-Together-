using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions.Must;

public class CheckpointManager : MonoBehaviour
{
    private List<GameObject> checkpointList;
    
    public void builCheckpointList()
    {
        checkpointList = GameObject.FindGameObjectsWithTag("Checkpoint")
            .OrderBy(o => o.name)
            .ToList();
    }

    public List<GameObject> getSelectedCheckpoint(string name)
    {
        List<GameObject> selectedCheckpoints = checkpointList.Where(o => o.name.Contains(name)).ToList();
        return selectedCheckpoints;
    }
}

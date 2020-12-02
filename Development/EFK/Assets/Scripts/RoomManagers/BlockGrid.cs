using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockGrid : MonoBehaviour
{
    [SerializeField] private int numberOfCells;
    private int cellsPerLine;
    AdjacencyList adjacencyList;

    // Start is called before the first frame update
    void Start()
    {
        cellsPerLine = (int) Mathf.Sqrt(numberOfCells);
        InitAdjacencyList();
    }

    void InitAdjacencyList() {
        adjacencyList = new AdjacencyList(numberOfCells);
        
        for(int i=0; i<numberOfCells; i++) {
            /* EdgeCost is already added for both direction in AdjacencyList implementation */
            if(i % cellsPerLine != 0) adjacencyList.AddEdgeCost(i, i - 1, 0);       // edge left to right
            if(i >= cellsPerLine) adjacencyList.AddEdgeCost(i, i-cellsPerLine, 0);  // edge up to down
        }
    }
}

class AdjacencyList {
    private List<Tuple<int, int>>[] adjacencyList;

    public AdjacencyList(int nodes) {
        adjacencyList = new List<Tuple<int, int>>[nodes];

        for (int i = 0; i < adjacencyList.Length; ++i) {
            adjacencyList[i] = new List<Tuple<int, int>>();
        }
    }

    public void AddEdgeCost(int node1, int node2, int cost) {
        adjacencyList[node1].Add(new Tuple<int, int>(node2, cost));
        adjacencyList[node2].Add(new Tuple<int, int>(node1, cost));
    }

    public int GetEdgeCost(int node1, int node2) {
        Tuple<int, int> costTuple = adjacencyList[node1].Where(x => x.Item1 == node2).FirstOrDefault();
        return costTuple==null ? 0 : costTuple.Item2;
    }
}

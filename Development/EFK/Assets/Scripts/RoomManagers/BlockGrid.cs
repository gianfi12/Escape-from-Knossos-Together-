using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockGrid : MonoBehaviour
{
    private const int CostRange = 100;
    private const float ActiveCellProbability = 0.3f;

    private System.Random rnd;
    private bool[] walkableGrid;

    [SerializeField] private int numberOfCells;
    private int cellsPerLine;
    AdjacencyList adjacencyList;

    [SerializeField] private List<Sprite> blockSprites;
    [SerializeField] private List<Transform> GuiSolutions;

    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random(GetComponentInParent<ObjectsContainer>().Seed);
        cellsPerLine = (int) Mathf.Sqrt(numberOfCells);
        walkableGrid = new bool[numberOfCells];

        // AdjacencyList is initalized with random edge costs
        InitAdjacencyList();

        List<int> shortestPath = GenerateSolutionByDijkstra();

        InitGrid(shortestPath);

        AssignBlockImages();
    }

    void InitAdjacencyList() {
        adjacencyList = new AdjacencyList(numberOfCells);

        for (int i = 0; i < numberOfCells; i++) {
            /* EdgeCost is already added for both direction in AdjacencyList implementation */
            if (i % cellsPerLine != 0) adjacencyList.AddEdgeCost(i, i - 1, rnd.Next(0, CostRange));       // edge left to right
            if (i >= cellsPerLine) adjacencyList.AddEdgeCost(i, i - cellsPerLine, rnd.Next(0, CostRange));  // edge up to down
        }
    }

    private List<int> GenerateSolutionByDijkstra() {
        // A path is generated connecting random cells from the bottom and the top row
        int startCell = rnd.Next(0, cellsPerLine);
        int endCell = rnd.Next(0, cellsPerLine) + numberOfCells - cellsPerLine;

        int[] distances = new int[numberOfCells];
        for(int i=0; i<distances.Count(); i++) {
            if (i == startCell) distances[i] = 0;
            else distances[i] = int.MaxValue;
        }

        List<int> visitedNodes = new List<int>();
        visitedNodes.Add(startCell);

        int[] predecessors = new int[numberOfCells];

        while(visitedNodes.Count() < numberOfCells) {
            int currNode = visitedNodes.Last();
            List<Tuple<int, int>> neighboursCosts = adjacencyList.GetAllNeighboursCost(currNode);
            foreach(Tuple<int,int> nc in neighboursCosts) {
                if (distances[currNode] + nc.Item2 < distances[nc.Item1]) {
                    distances[nc.Item1] = distances[currNode] + nc.Item2;
                    predecessors[nc.Item1] = currNode;
                }
            }

            int nextNode = Enumerable.Range(0, numberOfCells).Where(x => !visitedNodes.Contains(x)).OrderBy(x => distances[x]).First();
            visitedNodes.Add(nextNode);
        }

        List<int> shortestPath = new List<int>();
        int backtrackNode = endCell;
        while (backtrackNode != startCell) {
            shortestPath.Insert(0, backtrackNode);
            backtrackNode = predecessors[backtrackNode];
        }
        shortestPath.Insert(0, startCell);
        
        return shortestPath;
    }

    private void InitGrid(List<int> shortestPath) {
        for (int i = 0; i < walkableGrid.Count(); i++) {
            walkableGrid[i] = shortestPath.Contains(i) || (rnd.NextDouble() < ActiveCellProbability);

            int blockIndex = (i % cellsPerLine / 2) + (cellsPerLine / 2) * (i / (2 * cellsPerLine)); // index of block in grid that contains cell i
            int cellIndex = ((i / cellsPerLine) % 2) * 2 + (i % 2); // index of cell i in hierarchy of its containing block
            transform.GetChild(blockIndex).GetChild(cellIndex).GetComponent<BlockCell>().setWalkable(walkableGrid[i]);
        }
    }

    private void AssignBlockImages() {
        List<Sprite> spritesCopy = new List<Sprite>(blockSprites);

        foreach(Transform block in transform) {
            int i = rnd.Next(0, spritesCopy.Count());
            block.GetChild(4).GetComponent<SpriteRenderer>().sprite = spritesCopy[i];
            int originalIndex = blockSprites.IndexOf(spritesCopy[i]);
            spritesCopy.RemoveAt(i);


            for(int j=0; j<GuiSolutions[originalIndex].childCount; j++) {
                GuiSolutions[originalIndex].GetChild(j).gameObject.SetActive(!block.GetChild(j).GetComponent<BlockCell>().IsWalkable);
            }
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

    public List<Tuple<int,int>> GetAllNeighboursCost(int node) {
        return adjacencyList[node];
    }
}

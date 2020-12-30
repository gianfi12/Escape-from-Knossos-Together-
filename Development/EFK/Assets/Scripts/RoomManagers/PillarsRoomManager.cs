﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PillarsRoomManager : MonoBehaviour
{
    [SerializeField] private Doors controlledDoors;
    [SerializeField] private int solutionLength;
    [SerializeField] List<Pillar> pillars;
    [SerializeField] LineRenderer lightRayRenderer;
    private System.Random rnd;

    private List<int>[] connections = new List<int>[] {
        new List<int>{1,2,3,4,6,9},
        new List<int>{0,2,3,4,8},
        new List<int>{0,1,3},
        new List<int>{0,1,2,4,5,6,8},
        new List<int>{0,1,3,5,6},
        new List<int>{3,4,6,8,9},
        new List<int>{0,3,4,5,8,9},
        new List<int>{8,9},
        new List<int>{1,3,5,6,7,9},
        new List<int>{0,5,6,7,8}
    };

    private Tuple<int,int>[] solution;
    private List<Tuple<int, int>> activated = new List<Tuple<int,int>>();
    private int lastActivatedPillar=-1;

    void Start()
    {
        rnd = new System.Random(GetComponentInParent<ObjectsContainer>().Seed);
        foreach (Pillar p in pillars) p.SetPillarsRoomManager(this);
        
        generateSolution();
    }

    private void generateSolution() {
        solution = new Tuple<int, int>[Mathf.Clamp(solutionLength, 0, connections.Length)];

        int sourceNode = rnd.Next(0, connections.Length);
        int destNode;
        Tuple<int, int> path;
        Tuple<int, int> inversePath;
        for (int i=0; i<solution.Length; i++) {
            List<int> validDestinations = connections[sourceNode];
            do {
                destNode = validDestinations[rnd.Next(validDestinations.Count())];

                path = new Tuple<int, int>(sourceNode, destNode);
                inversePath = new Tuple<int, int>(destNode, sourceNode);

            } while(solution.Contains(path) || solution.Contains(inversePath));

            solution[i] = path;
            Debug.Log(solution[i]);
            sourceNode = destNode;
        }
    }

    public void PillarActivated(int pillarIndex) {
        if (lastActivatedPillar != -1) {
            if (connections[lastActivatedPillar].Contains(pillarIndex)) {               
                pillars[pillarIndex].DisableInteraction();
                pillars[lastActivatedPillar].EnableInteraction();

                activated.Add(new Tuple<int, int>(lastActivatedPillar, pillarIndex));
                GenerateLightRay();

                lastActivatedPillar = pillarIndex;

                if (activated.Count() >= solution.Length) VerifySolution();
            }
        }
        else {
            pillars[pillarIndex].DisableInteraction();
            lastActivatedPillar = pillarIndex;
        }     
    }

    private void VerifySolution() {
        bool check = activated.All(path => solution.Contains(path) || solution.Contains(new Tuple<int, int>(path.Item2, path.Item1)));
        Debug.Log(check);

        if (check) {
            controlledDoors.OpenDoors();
        }
        else ResetActivatedPillars();
    }

    private void ResetActivatedPillars() {
        foreach (Pillar p in pillars) p.ResetPillar();
        activated.Clear();
        StartCoroutine("GenerateLightRayWithDelay", 0.5f);
        lastActivatedPillar = -1;
    }

    private void GenerateLightRay() {
        lightRayRenderer.positionCount = activated.Count() + 1;

        for (int i=0; i < activated.Count(); i++) {
            if (i == 0) {
                Vector3 p1 = pillars[activated[i].Item1].transform.position;
                Vector3 p2 = pillars[activated[i].Item2].transform.position;

                lightRayRenderer.SetPosition(0, p1);
                lightRayRenderer.SetPosition(1, p2);
            }
            else {
                Vector3 p = pillars[activated[i].Item2].transform.position;
                lightRayRenderer.SetPosition(i + 1, p);
            }
        }

    }

    IEnumerator GenerateLightRayWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        GenerateLightRay();
    }
}

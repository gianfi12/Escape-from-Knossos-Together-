using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PillarsRoomManager : MonoBehaviour
{
    [SerializeField] private int solutionLength;
    [SerializeField] List<Pillar> pillars;
    [SerializeField] LineRenderer lightRayRenderer;
    [SerializeField] Color baseColor;
    [SerializeField] Color solvedColor;
    [SerializeField] List<Image> pathGUIImages;
    [SerializeField] private AudioSource pillarLoop;
    private System.Random rnd;
    private ObjectsContainer myRoom;

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
        myRoom = GetComponentInParent<ObjectsContainer>();
        rnd = new System.Random(myRoom.Seed);
        foreach (Pillar p in pillars) p.SetPillarsRoomManager(this);
        
        GenerateSolution();
        GenerateGUIPaths();
    }



    private void GenerateSolution() {
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
            sourceNode = destNode;
        }
    }

    public void PillarActivated(int pillarIndex) {
        if (lastActivatedPillar != -1) {
            if (connections[lastActivatedPillar].Contains(pillarIndex)) {               
                pillars[pillarIndex].DisableInteraction();
                pillars[pillarIndex].LightUp();
                pillars[lastActivatedPillar].EnableInteraction();

                Tuple<int, int> path = new Tuple<int, int>(lastActivatedPillar, pillarIndex);
                activated.Add(path);
                GenerateLightRay(false);

                lastActivatedPillar = pillarIndex;

                if (activated.Count() >= solution.Length)
                {
                    bool isRight = VerifySolution();
                    pillars[pillarIndex].PlayLastPillar(isRight);
                }
                else if (activated.Count() == 1)
                {
                    pillars[path.Item1].PlayFirstPillarOn();
                    pillars[path.Item2].PlaySecondPillarOn();
                }
                else
                {
                    pillars[pillarIndex].PlayPillarSound();
                }
            }
        }
        else {
            pillars[pillarIndex].DisableInteraction();
            pillars[pillarIndex].LightUp();
            pillars[pillarIndex].PlayFirstPillar();
            lastActivatedPillar = pillarIndex;
        }     
    }

    private bool VerifySolution() {
        bool check = activated.All(path => solution.Contains(path) || solution.Contains(new Tuple<int, int>(path.Item2, path.Item1)));
        check &= solution.All(path => activated.Contains(path) || activated.Contains(new Tuple<int, int>(path.Item2, path.Item1)));

        if (check)
        {
            StartCoroutine(myRoom.ExitDoor.OpenDoorsWithDelay(0.5f));
            GenerateLightRay(true);
        }
        else ResetActivatedPillars();

        return check;
    }

    public void ResetActivatedPillars() {
        foreach (Pillar p in pillars) p.ResetPillar();
        activated.Clear();
        StartCoroutine("GenerateLightRayWithDelay", 0.5f);
        lastActivatedPillar = -1;
    }

    private void GenerateLightRay(bool solved) {
        Color color = solved ? solvedColor : baseColor;

        lightRayRenderer.positionCount = activated.Count() + 1;
        lightRayRenderer.startColor = color;
        lightRayRenderer.endColor = color;

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

        if (activated.Count > 0)
        {
            if (!pillarLoop.isPlaying) pillarLoop.Play();
        }
        else pillarLoop.Stop(); 

    }

    IEnumerator GenerateLightRayWithDelay(float delay) {
        yield return new WaitForSeconds(delay);
        GenerateLightRay(false);
    }

    /* some hardcoded stuff here */
    private void GenerateGUIPaths() {
        // width and height of a cell in the diary
        const int w = 22;
        const int h = 25;
        // row (0-4) of each pillar in the scheme
        int[] y_lookup = new int[] { 1, 3, 0, 2, 4, 0, 1, 4, 1, 3 };

        Vector2 offset = new Vector2(-99, -50);

        for (int i = 0; i < solution.Length; i++) {
            Tuple<int, int> path = solution[i];
            RectTransform guiPathTrans = pathGUIImages[i].GetComponent<RectTransform>();

            Vector2 pos1 = new Vector2(path.Item1 * w, y_lookup[path.Item1] * h) + offset;
            Vector2 pos2 = new Vector2(path.Item2 * w, y_lookup[path.Item2] * h) + offset;

            guiPathTrans.localPosition = (pos1 + pos2) / 2;
            Vector3 dif = pos1 - pos2;
            guiPathTrans.sizeDelta = new Vector3(dif.magnitude, 5);
            guiPathTrans.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));
        }
    }
}

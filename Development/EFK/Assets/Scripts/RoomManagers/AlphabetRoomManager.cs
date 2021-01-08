using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AlphabetRoomManager : MonoBehaviour
{
    private List<int> selectedNumbers = new List<int>();
    private Mode mode;

    public struct Mode
    {
        public int a;
        public int b;
        public string name;

        public Mode(int a, int b, string name)
        {
            this.a = a;
            this.b = b;
            this.name = name;
        }
    }
    private Mode[] modes =
    {
        new Mode(0,0, "Row top->down from left to right"),new Mode(4,0,  "Row down->top from left to right"),
        new Mode(0,0, "Column left->right from top to down"), new Mode(4,0, "Column right->left from top to down")
    };
    
    private int[,] matrix = new int[5, 5] {
                                            {0,1,2,3,4},
                                            {5,6,7,8,9},
                                            {10,11,12,13,14},
                                            {15,16,17,18,19},
                                            {20,21,22,23,24}
                                            };

    [SerializeField] private Sprite[] runeSprites;
    private Collectable[] runes;
    [SerializeField] private CombinationPanel combinationPanel;
    [SerializeField] private Doors exitDoor;
    [SerializeField] private GameObject positionsContainer;
    private List<Transform> runePositions;

    private void SelectNumbers()
    {
        Random.InitState(GetComponent<ObjectsContainer>().Seed);
        int remaining = 5;
        int chosen;
        selectedNumbers = new List<int>();
        List<int> numbers = new List<int>();


        int countA = 5;
        int modeID = Random.Range(0,4);
        mode = modes[modeID];
        int a = mode.a;
        int b = mode.b;
        Debug.Log(modes[modeID].name);
        while (countA != 0)
        {
            int countB = 5;
            while (countB != 0)
            {
                //se mode riga
                if (modeID <= 1) numbers.Add(matrix[a,b]);
                //se mode colonna
                else numbers.Add(matrix[b,a]);
                b = UpdateAandB(1, modeID % 2, a, b);
                countB--;
            }

            a = UpdateAandB(0, modeID % 2, a, b);
            b = mode.b;
            countA--;
        }
        
        List<int> selectedIndexes = new List<int>();
        while (remaining != 0)
        {
            do
            {
                chosen = Random.Range(0, 25);
            } while (selectedIndexes.Contains(chosen));
            selectedIndexes.Add(chosen);
            remaining--;
        }
        selectedIndexes.Sort();
        Debug.Log(numbers.Count);
        foreach (var index in selectedIndexes)
        {
            selectedNumbers.Add(numbers[index]);
        }
    }

    private int UpdateAandB(int choice, int mode, int a, int b)
    {
        int x = a;
        int y = b;
        
        if (mode == 0)
        {
            x++;
            y++;
        }
        else
        {
            x--;
            y++;
        }

        if (choice == 0) return x;
        return y;
    }
    
    private void Start()
    {
        runePositions = new List<Transform>(positionsContainer.transform.GetComponentsInChildren<Transform>());
        runes = GetComponentsInChildren<Collectable>();
        SelectNumbers();
        //combinationPanel.transform.parent.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().transform.Find("Order").GetComponent<Text>().text += mode.name;
        System.Random rnd = new System.Random(GetComponent<ObjectsContainer>().Seed);
        int [] randomIndex = selectedNumbers.OrderBy(x => rnd.Next()).ToArray();
        for (int i = 0; i < runes.Length; i++)
        {
            runes[i].GetComponent<SpriteRenderer>().sprite = runeSprites[randomIndex[i]];
            runes[i].ID = randomIndex[i];
            int randomPos = rnd.Next(0, runePositions.Count);
            runes[i].transform.position = runePositions[randomPos].position;
            runePositions.RemoveAt(randomPos);
        }
    }

    public void VerifyCombination()
    {
        ItemSlot[] slots = combinationPanel.Slots;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].SlotImage.Image.enabled == false || slots[i].SlotImage.MyCollectable.ID != selectedNumbers[i])
            {
                StartCoroutine(ChangePanelColor(Color.red));
                combinationPanel.TriggerWrongCombination();
                FindObjectOfType<AudioManager>().Play("RuneWrong");
                return;
            }
        }
        StartCoroutine(ChangePanelColor(Color.green));
        FindObjectOfType<AudioManager>().Play("RuneRight");
        StartCoroutine(exitDoor.OpenDoorsWithDelay(0.5f));
        combinationPanel.ClosePanel(0.5f);
    }

    IEnumerator ChangePanelColor(Color newColor)
    {
        Image backgroundImage = combinationPanel.Panel.GetComponent<Image>();
        Color originalColor = backgroundImage.color;
        backgroundImage.color = newColor;
        //combinationPanel.Panel.GetComponent<Image>().material.SetColor("OutlineColor", newColor);
        yield return new WaitForSeconds(0.5f);
        backgroundImage.color = originalColor;
        //combinationPanel.Panel.GetComponent<Image>().material.SetColor("OutlineColor", originalColor);
    }
}

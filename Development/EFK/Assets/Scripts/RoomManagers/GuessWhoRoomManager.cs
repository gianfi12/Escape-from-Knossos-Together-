using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GuessWhoRoomManager : MonoBehaviour
{
    private GameObject lockersContainer;
    private Sprite[] employeePhotos = new Sprite[20];
    private int winnerLockerIndex;
    [SerializeField] private Doors exitDoor;
    [SerializeField] private CombinationPanel combinationPanel;
    [SerializeField] private Image guiImage;
    
    void Start()
    {
        System.Random rnd = new System.Random(GetComponent<ObjectsContainer>().Seed);
        lockersContainer = transform.Find("Lockers").gameObject;
        for (int i = 0; i < lockersContainer.transform.childCount; i++)
        {
            employeePhotos[i] = lockersContainer.transform.GetChild(i).GetComponent<Locker>().EmployeePhoto.sprite;
        }
        
        employeePhotos = employeePhotos.OrderBy(x => rnd.Next()).ToArray();
        
        List<int> selectedNumbers = new List<int>();
        int number;
        for (int i = 0; i < lockersContainer.transform.childCount; i++)
        {
            Locker locker = lockersContainer.transform.GetChild(i).GetComponent<Locker>();
            locker.EmployeePhoto.sprite = employeePhotos[i];
            locker.IDCard.ID = i;
        }

        winnerLockerIndex = rnd.Next(0,lockersContainer.transform.childCount);
        guiImage.sprite = employeePhotos[winnerLockerIndex];
    }

    public void VerifyCombination()
    {
        if (combinationPanel.Slots[0].SlotImage.MyCollectable.ID != winnerLockerIndex)
        {
            combinationPanel.Slots[0].RemoveImage();
            StartCoroutine(ChangePanelColor(Color.red));
            combinationPanel.TriggerWrongCombination();
            return;
        }
        
        StartCoroutine(ChangePanelColor(Color.green));
        exitDoor.OpenDoors();
    }
    
    IEnumerator ChangePanelColor(Color newColor)
    {
        Image backgroundImage = combinationPanel.Panel.GetComponent<Image>();
        Color originalColor = backgroundImage.color;
        backgroundImage.color = newColor;
        yield return new WaitForSeconds(0.5f);
        backgroundImage.color = originalColor;
    }
}

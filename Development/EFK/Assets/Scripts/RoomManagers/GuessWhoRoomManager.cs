using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GuessWhoRoomManager : MonoBehaviour
{
    private GameObject lockersContainer;
    private List<Sprite> activeWithCardSprites;
    private List<Sprite> activeWithoutCardSprites;
    private List<Sprite> guiPhotos;
    private List<int> allIndexes;
    private int winnerLockerIndex;
    [SerializeField] private Doors exitDoor;
    [SerializeField] private CombinationPanel combinationPanel;
    [SerializeField] private Image guiImage;
    
    void Start()
    {
        System.Random rnd = new System.Random(GetComponent<ObjectsContainer>().Seed);
        allIndexes = new List<int>();
        activeWithCardSprites = new List<Sprite>();
        activeWithoutCardSprites = new List<Sprite>();
        guiPhotos = new List<Sprite>();
        lockersContainer = transform.Find("Lockers").gameObject;
        for (int i = 0; i < lockersContainer.transform.childCount; i++)
        {
            allIndexes.Add(i);
            activeWithCardSprites.Add(lockersContainer.transform.GetChild(i).GetComponent<Locker>().ActiveWithCard);
            activeWithoutCardSprites.Add(lockersContainer.transform.GetChild(i).GetComponent<Locker>().ActiveWithoutCard);
            guiPhotos.Add(lockersContainer.transform.GetChild(i).GetComponent<Locker>().EmployeePhoto);
        }
        
        allIndexes = allIndexes.OrderBy(x => rnd.Next()).ToList();
        
        List<int> selectedNumbers = new List<int>();
        int number;
        winnerLockerIndex = rnd.Next(0,lockersContainer.transform.childCount);
        Debug.Log(winnerLockerIndex);
        for (int i = 0; i < lockersContainer.transform.childCount; i++)
        {
            Locker locker = lockersContainer.transform.GetChild(i).GetComponent<Locker>();
            locker.ActiveWithCard = activeWithCardSprites[allIndexes[i]];
            locker.ActiveWithoutCard = activeWithoutCardSprites[allIndexes[i]];
            locker.IDCard.ID = i;
            if (i == winnerLockerIndex) guiImage.sprite = guiPhotos[allIndexes[i]];
        }
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
        FindObjectOfType<AudioManager>().Play("TotemConfirm");
        StartCoroutine(exitDoor.OpenDoorsWithDelay(0.5f));
        combinationPanel.ClosePanel(0.5f);
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

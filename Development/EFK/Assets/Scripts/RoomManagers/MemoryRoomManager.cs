using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MemoryRoomManager : MonoBehaviour
{

    [SerializeField] private SpriteRenderer picture;
    [SerializeField] private SpriteRenderer[] pieces;
    [SerializeField] private GameObject[] diaryPictures;
    private Sprite[] slicedSprites = new Sprite[9];
    private List<int> shuffleOrder;
    private List<int> firstWinnerIndexes;
    private List<int> secondWinnerIndexes;
    private List<int> selectedPictures;
    private System.Random rnd;
    private int currentPicture;
    
    void Start()
    {
        rnd = new System.Random(GetComponent<ObjectsContainer>().Seed);
        shuffleOrder = new List<int> {0,1,2,3,4,5,6,7,8};
        firstWinnerIndexes = new List<int>();
        secondWinnerIndexes = new List<int>();
        selectedPictures = new List<int>();
        currentPicture = 0;
        
        //Select 2 pictures from 6
        while (selectedPictures.Count < 2)
        {
            int selectedPicture;
            do
            {
                selectedPicture = rnd.Next(0, 6);
            } while (selectedPictures.Contains(selectedPicture));
            selectedPictures.Add(selectedPicture); 
        }

        //Select the 3 winning pieces for the first picture
        while (firstWinnerIndexes.Count < 3)
        {
            int selected;
            do
            {
                selected = rnd.Next(0, 9);
            } while (firstWinnerIndexes.Contains(selected));
            firstWinnerIndexes.Add(selected);
        }
        
        //Select the 3 winning pieces for the second picture
        while (secondWinnerIndexes.Count < 3)
        {
            int selected;
            do
            {
                selected = rnd.Next(0, 9);
            } while (secondWinnerIndexes.Contains(selected));
            secondWinnerIndexes.Add(selected);
        }

        //Setup yhe diary
        for (int i = 0; i < diaryPictures.Length; i++)
        {
            Transform diarypicture = diaryPictures[i].transform;
            if (selectedPictures.Contains(i))
            {
                if (selectedPictures.IndexOf(i) == 0)
                {
                    for (int j = 0; j < diarypicture.childCount; j++)
                    {
                        if (firstWinnerIndexes.Contains(j)) diarypicture.GetChild(j).gameObject.SetActive(true);
                        else diarypicture.GetChild(j).gameObject.SetActive(false);
                    }
                }
                else if (selectedPictures.IndexOf(i) == 1)
                {
                    for (int j = 0; j < diarypicture.childCount; j++)
                    {
                        if (secondWinnerIndexes.Contains(j)) diarypicture.GetChild(j).gameObject.SetActive(true);
                        else diarypicture.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                List<int> fakeWinnerIndexes = new List<int>();
                while (fakeWinnerIndexes.Count < 3)
                {
                    int selected;
                    do
                    {
                        selected = rnd.Next(0, 9);
                    } while (fakeWinnerIndexes.Contains(selected));
                    fakeWinnerIndexes.Add(selected);
                }
                for (int j = 0; j < diarypicture.childCount; j++)
                {
                    if (fakeWinnerIndexes.Contains(j)) diarypicture.GetChild(j).gameObject.SetActive(true);
                    else diarypicture.GetChild(j).gameObject.SetActive(false);
                }
            }
        }
        SetUpRoom();
    }

    private void SetUpRoom()
    {
        picture.sprite = diaryPictures[selectedPictures[currentPicture]].GetComponent<Image>().sprite;
        
        //Slice the texture and prepare the 9 pieces
        int current = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                slicedSprites[current] = Sprite.Create(picture.sprite.texture, new Rect(333 * i,333 * j,333, 333), new Vector2(0.5f,0.5f), 333.0f);
                current++;
            }
        }
        
        //Assign and shuffle the pieces
        shuffleOrder = shuffleOrder.OrderBy(x => rnd.Next()).ToList();
        for (int i = 0; i < 9; i++)
        {
            pieces[i].sprite = slicedSprites[shuffleOrder[i]];
            PicturePiece piece = pieces[i].GetComponent<PicturePiece>();
            piece.PictureID = shuffleOrder[i];
            piece.ResetPiece();
        }
        
    }

    public void VerifyCombination()
    {
        int winCount = 0;
        if (currentPicture == 0)
        {
            for (int i = 0; i < 9; i++)
            {
                PicturePiece piece = pieces[i].GetComponent<PicturePiece>();
                if (piece.Active && firstWinnerIndexes.Contains(piece.PictureID)) winCount++;
            }
            if (winCount == 3)
            {
                currentPicture++;
                SetUpRoom();
                Debug.Log("HAI VINTO LA PRIMA PROVA");
            }
            else Debug.Log("HAI PERSO");
        }
        else if (currentPicture == 1)
        {
            for (int i = 0; i < 9; i++)
            {
                PicturePiece piece = pieces[i].GetComponent<PicturePiece>();
                if (piece.Active && secondWinnerIndexes.Contains(piece.PictureID)) winCount++;
            }
            if (winCount == 3)
            {
                //APRI PORTA
                Debug.Log("HAI VINTO");
            }
            else Debug.Log("HAI PERSO");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MemoryRoomManager : MonoBehaviour
{

    [SerializeField] private SpriteRenderer picture;
    [SerializeField] private SpriteRenderer[] pieces;
    [SerializeField] private GameObject diaryPicture;
    private Sprite[] slicedSprites = new Sprite[9];
    private List<int> shuffleOrder = new List<int> {0,1,2,3,4,5,6,7,8};
    private List<int> winnerIndex = new List<int>();
    
    void Start()
    {
        System.Random rnd = new System.Random(GetComponent<ObjectsContainer>().Seed);
        int current = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                slicedSprites[current] = Sprite.Create(picture.sprite.texture, new Rect(333 * i,333 * j,333, 333), new Vector2(0.5f,0.5f), 333.0f);
                current++;
            }
        }
        shuffleOrder = shuffleOrder.OrderBy(x => rnd.Next()).ToList();
        //slicedSprites = slicedSprites.OrderBy(x => rnd.Next()).ToArray();
        for (int i = 0; i < 9; i++)
        {
            pieces[i].sprite = slicedSprites[shuffleOrder[i]];
            pieces[i].GetComponent<PicturePiece>().PictureID = shuffleOrder[i];
        }

        while (winnerIndex.Count < 3)
        {
            int selected;
            do
            {
                selected = rnd.Next(0, 9);
            } while (winnerIndex.Contains(selected));
            winnerIndex.Add(selected);
        }

        for (int i = 0; i < 9; i++)
        {
            if (winnerIndex.Contains(i)) diaryPicture.transform.GetChild(i).gameObject.SetActive(true);
            else diaryPicture.transform.GetChild(i).gameObject.SetActive(false);
            Debug.Log("VINCITORE: "+i);
        }
    }

    public void VerifyCombination()
    {
        int winCount = 0;
        for (int i = 0; i < 9; i++)
        {
            PicturePiece piece = pieces[i].GetComponent<PicturePiece>();
            if (piece.Active && winnerIndex.Contains(piece.PictureID)) winCount++;
        }

        if (winCount == 3) Debug.Log("HAI VINTO");
        else Debug.Log("HAI PERSO");
    }
}

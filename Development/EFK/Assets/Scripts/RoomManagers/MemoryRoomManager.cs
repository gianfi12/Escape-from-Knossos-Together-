using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MemoryRoomManager : MonoBehaviour
{

    [SerializeField] private Sprite picture;
    [SerializeField] private SpriteRenderer[] pieces;
    private Sprite[] slicedSprites = new Sprite[9];
    private List<int> winnerIndex = new List<int>();
    
    void Start()
    {
        System.Random rnd = new System.Random(GetComponent<ObjectsContainer>().Seed);
        int current = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                slicedSprites[current] = Sprite.Create(picture.texture, new Rect(333 * i,333 * j,333, 333), new Vector2(0.5f,0.5f), 333.0f);
                current++;
            }
        }
        slicedSprites = slicedSprites.OrderBy(x => rnd.Next()).ToArray();
        for (int i = 0; i < 9; i++)
        {
            pieces[i].sprite = slicedSprites[i];
        }

        /*while (winnerIndex.Count <=3)
        {
            
            int selected = rnd.Next(0, 10);
        }*/
    }

    public void VerifyCombination()
    {
        
    }
    
}

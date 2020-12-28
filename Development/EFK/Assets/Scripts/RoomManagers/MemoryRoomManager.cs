using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryRoomManager : MonoBehaviour
{

    [SerializeField] private Sprite picture;
    [SerializeField] private SpriteRenderer[] slicedSprite;
    
    void Start()
    {
        int current = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                slicedSprite[current].sprite = Sprite.Create(picture.texture, new Rect(333 * i,333 * j,333, 333), new Vector2(0.5f,0.5f), 333.0f);
                current++;
            }
        }
    }
    
}

﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EntrancePanel : MonoBehaviour
{
    [SerializeField] private Doors controlledDoors;
    [SerializeField] private Image[] guiImages;
    [SerializeField] private Color[] buttonColors;
    private int[] order;
    List<int> pressed = new List<int>();


    private bool disabled=false;

    void Awake()
    {
<<<<<<< HEAD
        order = new int[buttonColors.Length];
        for (int i = 0; i < buttonColors.Length; i++)
        {
            order[i] = i;
        }
        System.Random rnd = new System.Random();
=======
        System.Random rnd = new System.Random(GetComponentInParent<ObjectsContainer>().Seed);

>>>>>>> 2413e079f5c501bc169ee3d3fcda571120d06933
        order = order.OrderBy(x => rnd.Next()).ToArray();

        for (int i = 0; i < order.Count(); i++) {
            transform.GetChild(i).GetComponent<EntranceButton>().SetButtonColor(buttonColors[i]);
            guiImages[i].color = buttonColors[order[i]];
        }

    }


    public void ButtonPressed(int index) {
        if (!disabled) {
            pressed.Add(index);

            if (pressed.Count() >= 3) {
                if (order.SequenceEqual(pressed)) {
                    controlledDoors.OpenDoors();
                    disabled = true;
                }
                else {
                    foreach (EntranceButton button in GetComponentsInChildren<EntranceButton>()) button.ResetLight();
                    pressed.Clear();
                }

            }
        }
    }

}

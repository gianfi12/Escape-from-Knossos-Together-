using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EntrancePanel : MonoBehaviour
{
    [SerializeField] private Doors controlledDoors;
    [SerializeField] private Image[] guiImages;
    [SerializeField] private Color[] buttonColors;
    int[] order = new int[3] {0, 1, 2};
    List<int> pressed = new List<int>();


    private bool disabled=false;

    void Awake()
    {
        System.Random rnd = new System.Random();
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

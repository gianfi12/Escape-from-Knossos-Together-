using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntrancePanel : MonoBehaviour
{
    [SerializeField] private Doors controlledDoors;
    int[] order = new int[3] {0, 1, 2};
    List<int> pressed = new List<int>();

    private bool disabled=false;

    void Start()
    {
        System.Random rnd = new System.Random();
        order = order.OrderBy(x => rnd.Next()).ToArray();

        foreach(int i in order) {
            Debug.Log(i);
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

                pressed.Clear();
            }
        }
    }
}

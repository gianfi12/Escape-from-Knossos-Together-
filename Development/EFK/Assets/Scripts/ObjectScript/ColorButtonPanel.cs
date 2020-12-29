﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ColorButtonPanel : MonoBehaviour {
    [SerializeField] private Doors controlledDoors;
    [SerializeField] private Image[] guiImages;
    [SerializeField] private Color[] buttonColors;
    [SerializeField] private int numberOfActiveButtons;

    private int[] orderedButtons;
    List<int> pressed = new List<int>();

    private bool disabled = false;

    private PressedButtons pressedButtonsGUI;


    void Awake()
    {
        System.Random rnd = new System.Random(GetComponentInParent<ObjectsContainer>().Seed);

        orderedButtons = new int[numberOfActiveButtons];
        for (int i = 0; i < numberOfActiveButtons; i++) orderedButtons[i] = -1;

        int n;
        for (int i = 0; i < numberOfActiveButtons; i++)
        {

            do n = rnd.Next(0, buttonColors.Count());
            while (orderedButtons.Contains(n));
            
            orderedButtons[i] = n;
        }
        
        // Set colors to all buttons
        for (int i = 0; i < buttonColors.Count(); i++) {
            transform.GetChild(i).GetComponentInChildren<ColorButton>().SetButtonColor(buttonColors[i]);
        }

        // Set colors to gui images in order only for active buttons
        for (int i=0; i< numberOfActiveButtons; i++) {
            guiImages[i].color = buttonColors[orderedButtons[i]];
        }

    }


    public void ButtonPressed(int index) {
        if (!disabled) {
            pressed.Add(index);
            if(pressedButtonsGUI != null) pressedButtonsGUI.UpdatePressedColors(buttonColors[index]);

            if (pressed.Count() >= numberOfActiveButtons) {
                if (orderedButtons.SequenceEqual(pressed)) {
                    controlledDoors.OpenDoors();
                    disabled = true;
                }
                else {
                    ResetButtons();
                }

            }
        }
    }

    public void ResetButtons()
    {
        foreach (ColorButton button in GetComponentsInChildren<ColorButton>()) button.ResetLight();
        if (pressedButtonsGUI != null) pressedButtonsGUI.ResetPressedColors();
        pressed.Clear();
    }
    
    public Doors ControlledDoors
    {
        get => controlledDoors;
        set => controlledDoors = value;
    }

    public Image[] GUIImages
    {
        get => guiImages;
        set => guiImages = value;
    }

    public PressedButtons PressedButtonsGUI 
    {
        get => pressedButtonsGUI; 
        set => pressedButtonsGUI = value; 
    }
}
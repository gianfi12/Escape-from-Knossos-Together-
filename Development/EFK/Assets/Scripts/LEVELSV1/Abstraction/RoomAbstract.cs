using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomAbstract : ScriptableObject
{
    [SerializeField] protected int settingIndex;
    [SerializeField] protected RoomSettingsAbstract settings;
    protected List<CellAbstract> cells = new List<CellAbstract>();

    public void Add(CellAbstract cellAbstract)
    {
        cellAbstract.Room = this;
        cells.Add(cellAbstract);
    }

    public RoomSettingsAbstract Settings
    {
        get => settings;
        set => settings = value;
    }
    
    public int SettingIndex
    {
        get => settingIndex;
        set => settingIndex = value;
    }

    public abstract GameObject Initialize();
}

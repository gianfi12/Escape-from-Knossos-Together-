using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomList",menuName="EFK/RoomList",order=1)]
public class RoomList : ScriptableObject
{
    [SerializeField] private List<RoomAbstract> rooms= new List<RoomAbstract>();

    public List<RoomAbstract> Rooms
    {
        get => rooms;
        set => rooms = value;
    }
}

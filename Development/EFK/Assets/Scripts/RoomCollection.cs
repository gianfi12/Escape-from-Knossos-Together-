using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="RoomCollection",menuName="EFK/RoomCollection",order=1)]
public class RoomCollection : ScriptableObject
{
    
    [SerializeField] private List<RoomAbstract> rooms = new List<RoomAbstract>();
    [SerializeField] private RoomAbstract startingRoomPlayer1;
    [SerializeField] private RoomAbstract startingRoomPlayer2;
    [SerializeField] private RoomAbstract finalRoom;

    public List<RoomAbstract> Rooms
    {
        get => rooms;
    }

    public RoomAbstract StartingRoomPlayer1 => startingRoomPlayer1;

    public RoomAbstract FinalRoom => finalRoom;

    public RoomAbstract StartingRoomPlayer2 => startingRoomPlayer2;
}

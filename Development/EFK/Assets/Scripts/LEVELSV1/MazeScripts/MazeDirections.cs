using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public static class MazeDirections
{
    private static IntVector2[] _vectorDirections = {
        new IntVector2(-1,0),
        new IntVector2(0,-1),
        new IntVector2(1,0),
        new IntVector2(0,1), 
    };

    private static MazeDirection[] _opposite =
    {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East
    };
    
    private static Quaternion[] _rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f,0f,90f), 
        Quaternion.Euler(0f,0f,180f),
        Quaternion.Euler(0f,0f,270f)
    };
    
    private static Vector3[] _position =
    {
        new Vector3(0f,1f,-1f),
        new Vector3(-1f,0f,-1f),
        new Vector3(0f,-1f,-1f),
        new Vector3(1f,0f,-1f),
    };

    public static Vector3 ToPosition(MazeDirection direction)
    {
        return _position[(int) direction];
    }

    public static Quaternion ToRotation(MazeDirection direction)
    {
        return _rotations[(int) direction];
    }
    
    public static MazeDirection GetOpposite(MazeDirection direction)
    {
        return _opposite[(int) direction];
    }
    
    
    public static int count= Enum.GetValues( typeof( MazeDirection ) ).Length;
    
    public static MazeDirection RandomValue()
    {
        return (MazeDirection) Random.Range(0, count);
    }

    public static IntVector2 IntVector2FromDirection(MazeDirection mazeDirection)
    {
        return _vectorDirections[(int) mazeDirection];
    }
    
    public enum MazeDirection
    {
        North,
        East,
        South,
        West,
    }

}

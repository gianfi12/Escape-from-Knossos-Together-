using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoomMaze : RoomAbstract
{
    private const int _minSpace=20;

    [SerializeField][Range(_minSpace,Single.PositiveInfinity)] private int maxSpace;
    
    private int _sizeX, _sizeY;
    public override void Generate()
    {
        _sizeX = Random.Range(_minSpace, maxSpace);
        _sizeY = Random.Range(_minSpace, maxSpace);
        _requiredWidthSpace = _sizeX;
        
        GenerateFloor();
        GenerateWall();
    }

    private void GenerateWall()
    {
        bool hasEntrance = false, hasExit = false;
        Direction[] directions = {Direction.East, Direction.South, Direction.West, Direction.North};
        int directionChange = 0;
        int index = _sizeX;//starts from the upper left side and we go in an anti clockwise round
        Vector3Int position = new Vector3Int(_sizeX-1,_sizeY-1,0);
        while (directionChange < 4)
        {
            if (true)
            {
                Tile tile;
                if(directionChange<2) tile = new Tile(assetsCollection.GetTileFromType(AssetType.WallTop)[0],
                    position);
                else tile = new Tile(assetsCollection.GetTileFromType(AssetType.WallBottom)[0],
                    position);
                
                TileList.Add(tile);
                Floor.Add(tile);
            }
            else
            {
                //if we have to put the entrance or the exit
            }

            index--;
            if (index <= 0)
            {
                directionChange++;
                
            }
        }
    }

    private void GenerateFloor()
    {
        for (int i = 0; i < _sizeX-2; i++)
        {
            for (int j = 0; j < _sizeY-2; j++)
            {
                Tile tile =  new Tile(assetsCollection.GetTileFromType(AssetType.Floor)[0],new Vector3Int(i+1,j+1,0));
                TileList.Add(tile);
                Floor.Add(tile);
            }
        }
    }

    public override void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall, Tilemap tilemapObject, Vector3Int coordinates)
    {
        
    }
}

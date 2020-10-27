using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Maze : LevelBuilderAbstract
{
    [SerializeField] private IntVector2 _size;
    [SerializeField] private CellAbstract cellAbstractPrefab;
    [SerializeField] private MazePassage _passagePrefab;
    [SerializeField] private MazeWall _wallPrefab;
    [SerializeField] private MazeDoor _doorPrefab;
    [Range(0f, 1f)] [SerializeField] private float _doorProbability;
    private CellAbstract[,] _cells;
    public MazeRoomSettings[] mazeRoomSettings;
    private List<MazeRoom> _mazeRooms = new List<MazeRoom>();

    
    public override void Generate()
    {
        _cells = new MazeCell[_size.x,_size.y];
        List<MazeCell> activeCells = new List<MazeCell>();
        IntVector2 actualCoordinates = RandomCoordinates();
        MazeCell firstCellAbstract = CreateCell(actualCoordinates);
        firstCellAbstract.Initialize(CreateRoom(-1));
        activeCells.Add(firstCellAbstract);
        while (activeCells.Count>0)
        {
            int currentIndex = activeCells.Count - 1;
            MazeCell currentCellAbstract = activeCells[currentIndex];
            if (currentCellAbstract.IsFullyInitialized())
            {
                activeCells.RemoveAt(currentIndex);
            }
            else
            {
                MazeDirections.MazeDirection actualDirection = currentCellAbstract.RandomUninitializedDirection();
                actualCoordinates = currentCellAbstract.Coordinates +
                                    MazeDirections.IntVector2FromDirection(actualDirection);
                if (ContainsCoordinates(actualCoordinates))
                {
                    MazeCell neighbor = GetCell(actualCoordinates) as MazeCell;
                    if (neighbor == null)
                    {
                        MazeCell newCellAbstract = CreateCell(actualCoordinates);
                        activeCells.Add(newCellAbstract);
                        CreatePassage(currentCellAbstract,newCellAbstract,actualDirection);
                    }else if(neighbor.Room.SettingIndex==currentCellAbstract.Room.SettingIndex)
                    {
                        CreatePassageInSameRoom(currentCellAbstract,neighbor,actualDirection);
                    }
                    else
                    {
                        CreateWall(currentCellAbstract,neighbor,actualDirection);
                    }
                }
                else
                {
                    CreateWall(currentCellAbstract,null,actualDirection);
                }
            }
        }
    }
    
    private MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCellAbstract = Instantiate(cellAbstractPrefab) as MazeCell;
        _cells[coordinates.x,coordinates.y] = newCellAbstract;
        newCellAbstract.Coordinates = coordinates;
        newCellAbstract.name = "Maze Cell " + newCellAbstract.Coordinates.x + ", " + newCellAbstract.Coordinates.y;
        Transform newCellTransform = newCellAbstract.transform;
        newCellTransform.parent = transform; //this makes the cell a child of the maze object
        //newCellTransform.localPosition = new Vector3(newCell.Coordinates.x- _size.x*1f+2f,newCell.Coordinates.y-_size.y*1f+2f,0f);
        float realX = (newCellAbstract.Coordinates.y - (float)_size.x / 2)*newCellAbstract.Width;
        float realY = (-newCellAbstract.Coordinates.x + (float)_size.y / 2)*newCellAbstract.Height;
        newCellTransform.localPosition = new Vector3(realX,realY,0f);
        return newCellAbstract;
    }

    public override IntVector2 RandomCoordinates()
    {
        return new IntVector2(Random.Range(0,_size.x),Random.Range(0,_size.y));
    }

    public override CellAbstract GetCell(IntVector2 coordinates)
    {
        return _cells[coordinates.x, coordinates.y];
    }

    public bool ContainsCoordinates(IntVector2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < _size.x && coordinate.y >= 0 && coordinate.y < _size.y;
    }

    private void CreatePassage(MazeCell cellAbstract,MazeCell otherCellAbstract,MazeDirections.MazeDirection direction)
    {
        MazePassage prefab = Random.value < _doorProbability ? _doorPrefab : _passagePrefab;
        MazePassage newPassage = Instantiate(prefab);
        newPassage.Initialize(cellAbstract,otherCellAbstract,direction);
        newPassage = Instantiate(prefab);
        if (newPassage is MazeDoor)
        {
            otherCellAbstract.Initialize(CreateRoom(cellAbstract.Room.SettingIndex));
        }
        else
        {
            otherCellAbstract.Initialize(cellAbstract.Room);
        }
        newPassage.Initialize(otherCellAbstract,cellAbstract,MazeDirections.GetOpposite(direction));
    }

    private void CreatePassageInSameRoom(MazeCell cellAbstract,MazeCell otherCellAbstract,MazeDirections.MazeDirection direction)
    {
        MazePassage passage = Instantiate(_passagePrefab);
        passage.Initialize(cellAbstract,otherCellAbstract,direction);
        passage = Instantiate(_passagePrefab);
        passage.Initialize(otherCellAbstract,cellAbstract,MazeDirections.GetOpposite(direction));
        if (cellAbstract.Room != otherCellAbstract.Room)
        {
            MazeRoom roomToAssimilate = otherCellAbstract.Room as MazeRoom;
            (cellAbstract.Room as MazeRoom).Assimilate(roomToAssimilate);
            _mazeRooms.Remove(roomToAssimilate);
            Destroy(roomToAssimilate);
        }
    }

    private void CreateWall(MazeCell cellAbstract, MazeCell otherCellAbstract, MazeDirections.MazeDirection direction)
    {
        MazeWall newWall = Instantiate(_wallPrefab);
        newWall.Initialize(cellAbstract,otherCellAbstract,direction);
        if (otherCellAbstract != null)
        {
            newWall = Instantiate(_wallPrefab);
            newWall.Initialize(otherCellAbstract,cellAbstract,MazeDirections.GetOpposite(direction));
        }
    }

    private MazeRoom CreateRoom(int indexToExclude)
    {
        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        newRoom.SettingIndex = Random.Range(0, mazeRoomSettings.Length);
        if (newRoom.SettingIndex == indexToExclude)
        {
            newRoom.SettingIndex = (newRoom.SettingIndex + 1) % mazeRoomSettings.Length;
        }
        newRoom.Settings = mazeRoomSettings[newRoom.SettingIndex];
        _mazeRooms.Add(newRoom);
        return newRoom;
    }
}

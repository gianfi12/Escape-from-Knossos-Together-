using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class RoomAbstract : MonoBehaviour
{
    [SerializeField] protected AssetsCollection assetsCollection;
    [SerializeField] private bool hasTwin;
    [SerializeField] private RoomAbstract twinRoom;
    [SerializeField] private GameObject doorExit;
    [SerializeField] private GameObject doorEntrance;
    [SerializeField] protected GameObject objectsParent;
    [SerializeField] protected bool useSameEntrance;

    public List<Tile> Entrance = new List<Tile>();
    public List<Tile> Exit = new List<Tile>();
    public readonly List<Tile> Wall = new List<Tile>();
    public readonly List<Tile> Floor = new List<Tile>();
    public readonly List<Tile> Spawn = new List<Tile>();
    public readonly List<Tile> Decoration = new List<Tile>();
    
    public readonly List<Tile> TileList = new List<Tile>();
    
    protected int _requiredWidthSpace;
    protected int _displacementX, _displacementY;
    protected int _lowestX;
    protected int _lowestY;
    
    protected List<GameObject> diaryComponents;
    protected PlayerControllerMap player;
    [SerializeField] private List<GameObject> diaryText;
    [SerializeField] private List<GameObject> diaryImage;
    
    public int RequiredWidthSpace => _requiredWidthSpace;

    public int DisplacementX => _displacementX;

    public int DisplacementY => _displacementY;

    public AssetsCollection AssetsCollection => assetsCollection;

    public abstract void Generate(int seed,bool isPlayer2);

    public abstract void PlaceRoom(Tilemap tilemapFloor, Tilemap tilemapWall,Tilemap tilemapDecoration);
    public abstract void PlaceObject(Vector3Int coordinates);

    public bool HasTwin => hasTwin;

    public RoomAbstract TwinRoom => twinRoom;

    public virtual List<GameObject> GetDiaryComponents()
    {
            diaryComponents.AddRange(diaryText);
            diaryComponents.AddRange(diaryImage);
            return diaryComponents;
    }

    public PlayerControllerMap Player
    {
        get => player;
        set => player = value;
    }
    
    
}

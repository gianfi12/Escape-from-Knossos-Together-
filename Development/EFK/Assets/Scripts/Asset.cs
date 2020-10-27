using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = UnityEngine.Tilemaps.Tile;

[CreateAssetMenu(fileName ="Asset",menuName="EFK/Asset",order=1)]
public class Asset : ScriptableObject
{
    [SerializeField] private TileBase entrance, exit, floor, wall;
    [SerializeField] private List<TileBase> tileCenter = new List<TileBase>();
    [SerializeField] private List<TileBase> tileBorder = new List<TileBase>();
    [SerializeField] private List<TileBase> tileCornerExternal = new List<TileBase>();
    [SerializeField] private List<TileBase> tileCornerInternal = new List<TileBase>();
    [SerializeField] private List<TileBase> tileWall = new List<TileBase>();
    public Texture2D tileTexture;

    public List<TileBase> TileCenter => tileCenter;

    public List<TileBase> TileBorder => tileBorder;

    public List<TileBase> TileCornerExternal => tileCornerExternal;

    public List<TileBase> TileCornerInternal => tileCornerInternal;

    public List<TileBase> TileWall => tileWall;
    
    public TileBase Entrance => entrance;

    public TileBase Exit => exit;

    public TileBase Floor => floor;

    public TileBase Wall => wall;

    public List<List<Tile>> GetTile(VectorAssetType assetType)
    {
        int SliceHeight = 16;
        int SliceWidth = 16;
                
        for (int i = 0; i < tileTexture.width; i += SliceWidth)
        {
            for (int j = tileTexture.height; j > 0; j -= SliceHeight)
            {
                
            }
        }
        TileBase
    }
    
}

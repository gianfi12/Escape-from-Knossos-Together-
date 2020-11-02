using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Tile = UnityEngine.Tilemaps.Tile;

[CreateAssetMenu(fileName ="AssetsCollection",menuName="EFK/Asset",order=1)]
public class AssetsCollection : ScriptableObject
{
    [SerializeField] private List<Asset> Assets;
    
    [Serializable]
    public class Asset
    {
        public AssetType AssetType;
        public List<TileBase> tileBases;
    }

    public List<TileBase> GetTileFromType(AssetType assetType)
    {
        return Assets.Where(x => x.AssetType == assetType).Select(x=> x.tileBases).ToList()[0];
    }

}

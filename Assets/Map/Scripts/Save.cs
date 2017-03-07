using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Save
{
    public Tiles[] tiles;

    [Serializable]
    public struct Tiles
    {
        public GridTile tile;
        public WorldPos pos;
    }

    public string saveToString(bool savePacked = false)
    {
        return JsonUtility.ToJson(this, savePacked);
    }

    public void ToArray(List<Tiles> t)
    {
        tiles = t.ToArray();
    }

    public Dictionary<WorldPos, GridTile> ToDictionary()
    {
        Dictionary<WorldPos, GridTile> t = new Dictionary<WorldPos, GridTile>();
        foreach (var item in tiles)
        {
            t.Add(item.pos, item.tile);
        }
        return t;
    }
}

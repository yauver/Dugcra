using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class GridTile
{
    public enum TileTypes { Ground, Wall, Empty, Fog }
    public struct Tile { public int x, y; }
    const float tileSize = 0.125f;
    public TileTypes type;
    [NonSerialized]
    public TileTypes[] adjTiles = new TileTypes[4];
    [NonSerialized]
    public TileTypes[] diagTiles = new TileTypes[4];
    //  0 /0 1
    // /1   /2
    //  2 /3 3

    [NonSerialized]
    public bool changed = false;

    public GridTile()
    {

    }

    public GridTile(TileTypes type)
    {
        this.type = type;
    }

    public virtual MeshData TileData(Grid grid, int x, int y, MeshData meshData)
    {
        if (type == TileTypes.Empty)
        {
            return meshData;
        }
        else if (type == TileTypes.Wall || type == TileTypes.Fog)
        {
            meshData.useRenderDataForCol = true;
        }
        else
        {
            meshData.useRenderDataForCol = false;
        }
        meshData.AddVertex(new Vector3(x + 1f, y));
        meshData.AddVertex(new Vector3(x + 1f, y + 1f));
        meshData.AddVertex(new Vector3(x, y + 1f));
        meshData.AddVertex(new Vector3(x, y));

        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs());

        return meshData;
    }

    public virtual Tile TexturePosition()
    {
        Tile tile = new Tile();
        if (type == TileTypes.Ground)
        {
            tile.x = UnityEngine.Random.Range(5, 7);
            tile.y = UnityEngine.Random.Range(5, 7);
        }
        else if (type == TileTypes.Wall)
        {
            #region if checks
            //  / 0 \
            //  1   2
            //  \ 3 /
            //adj tiles, if 1 >= ground
            if (adjTiles[0] == TileTypes.Ground)
            {
                if (adjTiles[1] == TileTypes.Ground)
                {
                    if (adjTiles[2] == TileTypes.Ground)
                    {
                        if (adjTiles[3] == TileTypes.Ground)
                        {
                            tile.x = 0;
                            tile.y = 3;
                            return tile;
                        }
                        tile.x = 2;
                        tile.y = 7;
                        return tile;
                    }
                    else if (adjTiles[3] == TileTypes.Ground)
                    {
                        tile.x = 0;
                        tile.y = 4;
                        return tile;
                    }
                    tile.x = 1;
                    tile.y = 6;
                    return tile;
                }
                else if (adjTiles[2] == TileTypes.Ground)
                {
                    if (adjTiles[3] == TileTypes.Ground)
                    {
                        tile.x = 6;
                        tile.y = 4;
                        return tile;
                    }
                    tile.x = 4;
                    tile.y = 6;
                    return tile;
                }
                else if (adjTiles[3] == TileTypes.Ground)
                {
                    tile.x = 5;
                    tile.y = 4;
                    return tile;
                }
                tile.x = 3;
                tile.y = 6;
                return tile;
            }


            if (adjTiles[1] == TileTypes.Ground)
            {
                if (adjTiles[2] == TileTypes.Ground)
                {
                    if (adjTiles[3] == TileTypes.Ground)
                    {
                        tile.x = 2;
                        tile.y = 1;
                        return tile;
                    }
                    tile.x = 2;
                    tile.y = 2;
                    return tile;
                }
                if (adjTiles[3] == TileTypes.Ground)
                {
                    tile.x = 1;
                    tile.y = 3;
                    return tile;
                }
                tile.x = 1;
                tile.y = 5;
                return tile;
            }


            if (adjTiles[2] == TileTypes.Ground)
            {
                if (adjTiles[3] == TileTypes.Ground)
                {
                    tile.x = 4;
                    tile.y = 3;
                    return tile;
                }
                tile.x = 4;
                tile.y = 5;
                return tile;
            }

            if (adjTiles[3] == TileTypes.Ground)
            {
                tile.x = 3;
                tile.y = 3;
                return tile;
            }

            //if no ground adj, check diagonals.
            //  0 - 1
            //  |   |
            //  2 - 3

            if (diagTiles[0] == TileTypes.Ground)
            {
                if (diagTiles[1] == TileTypes.Ground)
                {
                    if (diagTiles[2] == TileTypes.Ground)
                    {
                        if (diagTiles[3] == TileTypes.Ground)
                        {
                            tile.x = 4;
                            tile.y = 0;
                            return tile;
                        }
                        tile.x = 4;
                        tile.y = 1;
                        return tile;
                    }
                    if (diagTiles[3] == TileTypes.Ground)
                    {
                        tile.x = 3;
                        tile.y = 1;
                        return tile;
                    }
                    tile.x = 2;
                    tile.y = 6;
                    return tile;
                }
                if (diagTiles[2] == TileTypes.Ground)
                {
                    if (diagTiles[3] == TileTypes.Ground)
                    {
                        tile.x = 4;
                        tile.y = 2;
                        return tile;
                    }
                    tile.x = 1;
                    tile.y = 4;
                    return tile;
                }
                if (diagTiles[3] == TileTypes.Ground)
                {
                    tile.x = 0;
                    tile.y = 0;
                    return tile;
                }
                tile.x = 3;
                tile.y = 4;
                return tile;
            }

            //      1
            //      |
            //  2 - 3
            if (diagTiles[1] == TileTypes.Ground)
            {
                if (diagTiles[2] == TileTypes.Ground)
                {
                    if (diagTiles[3] == TileTypes.Ground)
                    {
                        tile.x = 3;
                        tile.y = 2;
                        return tile;
                    }
                    tile.x = 0;
                    tile.y = 1;
                    return tile;
                }
                if (diagTiles[3] == TileTypes.Ground)
                {
                    tile.x = 4;
                    tile.y = 4;
                    return tile;
                }
                tile.x = 2;
                tile.y = 4;
                return tile;
            }

            //  2 - 3

            if (diagTiles[2] == TileTypes.Ground)
            {
                if (diagTiles[3] == TileTypes.Ground)
                {
                    tile.x = 2;
                    tile.y = 3;
                    return tile;
                }
                tile.x = 3;
                tile.y = 5;
                return tile;
            }


            if (diagTiles[3] == TileTypes.Ground)
            {
                tile.x = 2;
                tile.y = 5;
                return tile;
            }

            tile.x = 6;
            tile.y = 1;
            #endregion
        }
        else if (type == TileTypes.Fog)
        {
            tile.x = 6;
            tile.y = 1;
        }
        else if (type == TileTypes.Empty)
        {
            return tile;
        }
        return tile;
    }

    public virtual Vector2[] FaceUVs()
    {
        Vector2[] UVs = new Vector2[4];
        Tile tilePos = TexturePosition();

        UVs[0] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y);
        UVs[1] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y + tileSize);
        UVs[2] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y + tileSize);
        UVs[3] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y);

        return UVs;
    }


}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridGenerator
{
    public Grid GridGen(Grid grid, bool isFogGenerator = false)
    {
        if (isFogGenerator)
        {
            for (int x = grid.pos.x; x < grid.pos.x + Grid.gridSize; x++)
            {
                for (int y = grid.pos.y; y < grid.pos.y + Grid.gridSize; y++)
                {
                    grid = GridFogGen(grid, x, y);
                }
            }
        }
        else
        {
            for (int x = grid.pos.x; x < grid.pos.x + Grid.gridSize; x++)
            {
                for (int y = grid.pos.y; y < grid.pos.y + Grid.gridSize; y++)
                {
                    grid = GridTileGen(grid, x, y);
                }
            }
        }
        return grid;
    }

    private Grid GridTileGen(Grid grid, int x, int y)
    {
        if ((
            (x == grid.pos.x || x == grid.pos.x + Grid.gridSize - 1) && (y >= grid.pos.y && y <= grid.pos.y + Grid.gridSize)
            ) || (
            (y == grid.pos.y || y == grid.pos.y + Grid.gridSize - 1) && x >= grid.pos.x && x <= grid.pos.x + Grid.gridSize
            ))
        {
            GridTile tile = new GridTile(GridTile.TileTypes.Wall);
            grid.SetTile(x - grid.pos.x, y - grid.pos.y, tile);
        }
        else
        {
            GridTile tile = new GridTile(GridTile.TileTypes.Ground);
            grid.SetTile(x - grid.pos.x, y - grid.pos.y, tile);
        }
        return grid;
    }

    private Grid GridFogGen(Grid grid, int x, int y)
    {
        GridTile tile = new GridTile(GridTile.TileTypes.Fog);
        grid.SetTile(x - grid.pos.x, y - grid.pos.y, tile);
        return grid;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class World : MonoBehaviour {

    public Dictionary<WorldPos, Grid> grids = new Dictionary<WorldPos, Grid>();
    public GameObject gridPrefab;
    public bool isFogGenerator = false;

    public string worldName = "World";

    public void CreateGrid(int x, int y)
    {
        WorldPos worldPos = new WorldPos(x, y);

        GameObject newGridObject = Instantiate(gridPrefab, new Vector3(x, y), Quaternion.Euler(Vector3.zero)) as GameObject;

        Grid newGrid = newGridObject.GetComponent<Grid>();

        GridGenerator gen = new GridGenerator();

        newGrid.pos = worldPos;
        newGrid.world = this;
        if (isFogGenerator || !SaveAndLoadManager.LoadGrid(newGrid))
        {
            newGrid = gen.GridGen(newGrid, isFogGenerator);
        }

        grids.Add(worldPos, newGrid);

        newGridObject.layer = 9;

    }

    public Grid GetGrid(int x, int y)
    {
        WorldPos pos = new WorldPos();
        float multiple = Grid.gridSize;
        pos.x = Mathf.FloorToInt(x / multiple) * Grid.gridSize;
        pos.y = Mathf.FloorToInt(y / multiple) * Grid.gridSize;

        Grid containerGrid= null;

        grids.TryGetValue(pos, out containerGrid);

        return containerGrid;
    }

    public void SetTile(int x, int y, GridTile tile)
    {
        Grid grid = GetGrid(x, y);

        if (grid != null)
        {
            grid.SetTile(x - grid.pos.x, y - grid.pos.y, tile);
            grid.update = true;
        }
    }

    public GridTile GetTile(int x, int y)
    {
        Grid containerGrid = GetGrid(x, y);

        if (containerGrid != null)
        {
            GridTile block = containerGrid.GetTile(
                x - containerGrid.pos.x,
                y - containerGrid.pos.y);

            return block;
        }
        else
        {
            return new GridTile(GridTile.TileTypes.Empty);
        }

    }

    internal void DestroyGrid(int x, int y)
    {
        Grid grid = null;
        if (grids.TryGetValue(new WorldPos(x, y), out grid))
        {
            Destroy(grid.gameObject);
            grids.Remove(new WorldPos(x, y));
        }
    }
}

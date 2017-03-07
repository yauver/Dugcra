using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LoadGrids : MonoBehaviour
{
    public World world;

    List<WorldPos> gridPositions = new List<WorldPos>();
    List<WorldPos> updateList = new List<WorldPos>();
    List<WorldPos> buildList = new List<WorldPos>();

    int timer = 0;

    void Start()
    {
        for (int x = 0; x <= 0; x++)
        {
            for (int y = 0; y <= 0; y++)
            {
                gridPositions.Add(new WorldPos(x, y));
            }
        }
    }

    void Update()
    {
        DeleteGrids();
        FindGridsToLoad();
        LoadAndRenderGrids();
    }
    private void FindGridsToLoad()
    {
        WorldPos playerPos = new WorldPos(
            Mathf.FloorToInt(transform.position.x / Grid.gridSize) * Grid.gridSize,
            Mathf.FloorToInt(transform.position.y / Grid.gridSize) * Grid.gridSize
            );
        if (buildList.Count == 0)
        {
            for (int i = 0; i < gridPositions.Count; i++)
            {
                WorldPos newChunkPos = new WorldPos(
                   gridPositions[i].x * Grid.gridSize + playerPos.x,
                   gridPositions[i].y * Grid.gridSize + playerPos.y
                   );

                Grid newGrid = world.GetGrid(newChunkPos.x, newChunkPos.y);

                if (newGrid != null
                    && (newGrid.rendered || updateList.Contains(newChunkPos)))
                    continue;

                buildList.Add(new WorldPos(newChunkPos.x, newChunkPos.y));
                return;
            }
        }
    }

    private void LoadAndRenderGrids()
    {

        for (int i = 0; i < 2; i++)
        {
            if (buildList.Count != 0)
            {
                BuildGrid(buildList[0]);
                buildList.RemoveAt(0);
            }
        }

        for (int i = 0; i < updateList.Count; i++)
        {
            Grid grid = world.GetGrid(updateList[0].x, updateList[0].y);
            if (grid != null)
                grid.update = true;
            updateList.RemoveAt(0);
        }
    }

    private void BuildGrid(WorldPos pos)
    {

        //for (int y = pos.y - Grid.gridSize; y <= pos.y + Grid.gridSize; y += Grid.gridSize)
        //{
        //    for (int x = pos.x - Grid.gridSize; x <= pos.x + Grid.gridSize; x += Grid.gridSize)
        //    {
                if (world.GetGrid(pos.x, pos.y) == null)
                    world.CreateGrid(pos.x, pos.y);
        //    }
        //}
        updateList.Add(pos);

    }

    void DeleteGrids()
    {
        if (timer == 10)
        {
            var gridsToDelete = new List<WorldPos>();
            foreach (var grid in world.grids)
            {
                float distance = Vector3.Distance(new Vector3(grid.Value.pos.x, grid.Value.pos.y),
                    new Vector3(transform.position.x, transform.position.y));

                if (distance > 256)
                    gridsToDelete.Add(grid.Key);
            }

            foreach (var grid in gridsToDelete)
                world.DestroyGrid(grid.x, grid.y);

            timer = 0;
        }
        timer++;
    }
}

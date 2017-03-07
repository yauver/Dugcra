using UnityEngine;
using System.Collections;

public static class EditTerrain
{
    public static WorldPos GetBlockPos(Vector2 pos)
    {
        WorldPos blockPos = new WorldPos(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.y)
            );

        return blockPos;
    }

    public static WorldPos GetBlockPos(RaycastHit2D hit, bool adjacent = false)
    {
        Vector2 pos = new Vector2(
            MoveWithinBlock(hit.point.x, hit.normal.x, adjacent),
            MoveWithinBlock(hit.point.y, hit.normal.y, adjacent)
            );

        return GetBlockPos(pos);
    }

    static float MoveWithinBlock(float pos, float norm, bool adjacent = false)
    {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f)
        {
            if (adjacent)
            {
                pos += (norm / 2);
            }
            else
            {
                pos -= (norm / 2);
            }
        }

        return pos;
    }     
    
    public static GridTile GetBlock(RaycastHit2D hit, bool adjacent = false)
    {
        Grid grid = hit.collider.GetComponent<Grid>();
        if (grid == null)
            return null;

        WorldPos pos = GetBlockPos(hit, adjacent);

        GridTile block = grid.world.GetTile(pos.x, pos.y);

        return block;
    }
}
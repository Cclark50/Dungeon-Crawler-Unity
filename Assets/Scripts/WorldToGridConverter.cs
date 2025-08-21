using UnityEngine;

public static class WorldToGridConverter
{

    private const float GRID_UNIT = 1.5f;
    
    public static Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / GRID_UNIT);
        int y = Mathf.RoundToInt(worldPosition.z / GRID_UNIT);
        return new Vector2Int(x,y);
    }

    public static Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float x = gridPosition.x * GRID_UNIT;
        float y = gridPosition.y * GRID_UNIT;
        return new Vector3(x, 0, y);
    }
}

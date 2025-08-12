using UnityEngine;

public static class WorldToGridConverter
{
    public static Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / 1.5f);
        int y = Mathf.RoundToInt(worldPosition.z / 1.5f);
        return new Vector2Int(x,y);
    }

    public static Vector3 GridToWorld(Vector2Int gridPosition)
    {
        float x = gridPosition.x * 1.5f;
        float y = gridPosition.y * 1.5f;
        return new Vector3(x, 0, y);
    }
}

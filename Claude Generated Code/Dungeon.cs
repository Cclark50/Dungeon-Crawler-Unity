// Central data structure holds the truth
[System.Serializable]
public class DungeonData : ScriptableObject {
    public int width;
    public int height;
    public WallData[,] horizontalWalls;
    public WallData[,] verticalWalls;
    public CellData[,] cells;
}

[System.Serializable]
public class WallData {
    public bool exists;
    public WallType type;
    public string prefabId;
    public List<WallBehaviorData> behaviors;
    // Serializable data only - no GameObjects here
}

// Wall renderer manages GameObject pool
public class WallRenderer : MonoBehaviour {
    [SerializeField] private Dictionary<string, GameObject> wallPrefabs;
    [SerializeField] private Transform wallContainer;

    private Dictionary<Vector2Int, GameObject> activeWalls;
    private Queue<GameObject> wallPool;
    private DungeonData dungeonData;

    public void BuildDungeon(DungeonData data) {
        dungeonData = data;
        ClearExistingWalls();

        // Only create GameObjects for walls near player
        UpdateVisibleWalls(playerPosition, viewDistance);
    }

    void UpdateVisibleWalls(Vector3 playerPos, float distance) {
        Vector2Int gridPos = WorldToGrid(playerPos);

        // Calculate visible range
        int minX = gridPos.x - Mathf.CeilToInt(distance);
        int maxX = gridPos.x + Mathf.CeilToInt(distance);

        // Spawn/despawn walls as needed
        HashSet<Vector2Int> neededWalls = GetWallsInRange(minX, maxX, minY, maxY);

        // Spawn new walls
        foreach (var wallPos in neededWalls) {
            if (!activeWalls.ContainsKey(wallPos)) {
                SpawnWall(wallPos);
            }
        }

        // Return distant walls to pool
        var toRemove = new List<Vector2Int>();
        foreach (var kvp in activeWalls) {
            if (!neededWalls.Contains(kvp.Key)) {
                ReturnToPool(kvp.Value);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove) {
            activeWalls.Remove(key);
        }
    }

    GameObject SpawnWall(Vector2Int dataPos) {
        WallData data = GetWallData(dataPos);

        GameObject wall = GetFromPool(data.prefabId);
        wall.transform.position = GridToWorld(dataPos);
        wall.transform.rotation = GetWallRotation(dataPos);

        // Apply behaviors
        var wallComponent = wall.GetComponent<WallController>();
        wallComponent.Initialize(data, dataPos);

        activeWalls[dataPos] = wall;
        return wall;
    }
}

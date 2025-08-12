public class Dungeon3DGenerator : MonoBehaviour {
    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject ceilingPrefab;
    public WallPrefabSet wallPrefabs;
    public PropPrefabLibrary propLibrary;

    [Header("Settings")]
    public float cellSize = 3f;
    public float wallHeight = 3f;
    public bool generateLighting = true;
    public bool generateNavMesh = true;

    public GameObject Generate3DDungeon(DungeonData data) {
        GameObject dungeonRoot = new GameObject("Generated Dungeon");

        // Generate floors and ceilings
        GenerateFloors(data, dungeonRoot.transform);

        // Generate walls
        GenerateWalls(data, dungeonRoot.transform);

        // Place props
        PlaceProps(data, dungeonRoot.transform);

        // Setup encounter triggers
        SetupEncounterZones(data, dungeonRoot.transform);

        // Post-processing
        if (generateLighting) {
            GenerateLighting(dungeonRoot);
        }

        if (generateNavMesh) {
            GenerateNavMesh(dungeonRoot);
        }

        // Optimize
        OptimizeDungeon(dungeonRoot);

        return dungeonRoot;
    }

    void GenerateWalls(DungeonData data, Transform parent) {
        GameObject wallContainer = new GameObject("Walls");
        wallContainer.transform.parent = parent;

        // Generate horizontal walls
        for (int x = 0; x < data.width; x++) {
            for (int y = 0; y <= data.height; y++) {
                if (data.horizontalWalls[x, y].exists) {
                    Vector3 position = new Vector3(
                        x * cellSize + cellSize/2,
                        wallHeight/2,
                        y * cellSize
                    );

                    var wallPrefab = GetWallPrefab(data.horizontalWalls[x, y].type);
                    var wall = Instantiate(wallPrefab, position, Quaternion.identity, wallContainer.transform);
                    wall.name = $"Wall_H_{x}_{y}";

                    ConfigureWall(wall, data.horizontalWalls[x, y]);
                }
            }
        }

        // Similar for vertical walls...
    }

    void PlaceProps(DungeonData data, Transform parent) {
        GameObject propContainer = new GameObject("Props");
        propContainer.transform.parent = parent;

        foreach (var prop in data.props) {
            var prefab = propLibrary.GetPrefab(prop.propId);
            if (prefab == null) continue;

            Vector3 position = GridToWorld(prop.gridPosition);

            // Adjust based on anchor
            switch (prop.anchor) {
                case PropAnchor.Wall:
                    position += GetWallOffset(prop.gridPosition, prop.rotation);
                    break;
                case PropAnchor.Ceiling:
                    position.y = wallHeight;
                    break;
                case PropAnchor.Corner:
                    position += GetCornerOffset(prop.gridPosition);
                    break;
            }

            var instance = Instantiate(prefab, position, Quaternion.Euler(0, prop.rotation, 0), propContainer.transform);

            // Apply metadata
            if (prop.metadata.blockMovement) {
                instance.AddComponent<NavMeshObstacle>();
            }

            if (prop.metadata.isInteractable) {
                var interactable = instance.AddComponent<Interactable>();
                interactable.scriptName = prop.metadata.interactionScript;
            }
        }
    }
}

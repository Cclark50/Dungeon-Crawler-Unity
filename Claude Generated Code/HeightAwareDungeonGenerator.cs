public class HeightAwareDungeonGenerator : MonoBehaviour {
    [Header("Height Settings")]
    public float unitHeight = 1f;  // World units per height unit
    public bool generateRamps = true;
    public float maxWalkableSlope = 30f;  // Degrees

    public GameObject Generate3DDungeon(DungeonData data) {
        GameObject dungeonRoot = new GameObject("Multi-Height Dungeon");

        // Generate floors at different heights
        GenerateVariableFloors(data, dungeonRoot.transform);

        // Generate walls with proper heights
        GenerateHeightAwareWalls(data, dungeonRoot.transform);

        // Generate vertical connectors
        GenerateVerticalConnectors(data, dungeonRoot.transform);

        return dungeonRoot;
    }

    void GenerateVariableFloors(DungeonData data, Transform parent) {
        GameObject floorContainer = new GameObject("Floors");
        floorContainer.transform.parent = parent;

        // Group adjacent cells of same height for optimization
        var heightGroups = GroupCellsByHeight(data);

        foreach (var group in heightGroups) {
            float height = group.Key;
            List<Vector2Int> cells = group.Value;

            // Merge adjacent cells into larger floor pieces
            var floorRegions = MergeAdjacentCells(cells);

            foreach (var region in floorRegions) {
                CreateFloorMesh(region, height, floorContainer.transform);
            }
        }

        // Generate transition pieces
        GenerateFloorTransitions(data, floorContainer.transform);
    }

    void GenerateFloorTransitions(DungeonData data, Transform parent) {
        for (int x = 0; x < data.width; x++) {
            for (int y = 0; y < data.height; y++) {
                float currentHeight = data.cells[x, y].floorHeight;

                // Check east neighbor
                if (x < data.width - 1) {
                    float eastHeight = data.cells[x + 1, y].floorHeight;
                    if (Mathf.Abs(eastHeight - currentHeight) > 0.1f) {
                        CreateTransition(
                            new Vector3(x + 1, currentHeight, y) * cellSize,
                            new Vector3(x + 1, eastHeight, y + 1) * cellSize,
                            parent
                        );
                    }
                }

                // Check north neighbor
                if (y < data.height - 1) {
                    float northHeight = data.cells[x, y + 1].floorHeight;
                    if (Mathf.Abs(northHeight - currentHeight) > 0.1f) {
                        CreateTransition(
                            new Vector3(x, currentHeight, y + 1) * cellSize,
                            new Vector3(x + 1, northHeight, y + 1) * cellSize,
                            parent
                        );
                    }
                }
            }
        }
    }

    void CreateTransition(Vector3 lowPoint, Vector3 highPoint, Transform parent) {
        float heightDiff = Mathf.Abs(highPoint.y - lowPoint.y);

        if (heightDiff <= 0.5f * unitHeight) {
            // Small step - create a sloped transition
            CreateRamp(lowPoint, highPoint, parent);
        } else if (heightDiff <= 1.5f * unitHeight) {
            // Medium height - create stairs
            CreateStairMesh(lowPoint, highPoint, parent);
        } else {
            // Large height difference - just a wall
            CreateWallTransition(lowPoint, highPoint, parent);
        }
    }

    void GenerateHeightAwareWalls(DungeonData data, Transform parent) {
        GameObject wallContainer = new GameObject("Walls");
        wallContainer.transform.parent = parent;

        // Generate walls that adapt to floor heights
        for (int x = 0; x < data.width; x++) {
            for (int y = 0; y <= data.height; y++) {
                var wallData = data.horizontalWalls[x, y];
                if (!wallData.exists) continue;

                // Get floor heights on both sides of wall
                float southFloor = (y > 0) ? data.cells[x, y - 1].floorHeight : 0f;
                float northFloor = (y < data.height) ? data.cells[x, y].floorHeight : 0f;

                float bottomHeight = Mathf.Min(southFloor, northFloor);
                float topHeight = Mathf.Max(southFloor, northFloor) + wallHeight;

                if (wallData.isHalfWall) {
                    topHeight = bottomHeight + wallData.halfWallHeight;
                }

                Vector3 position = new Vector3(
                    x * cellSize + cellSize/2,
                    bottomHeight * unitHeight,
                    y * cellSize
                );

                CreateAdaptiveWall(position, bottomHeight, topHeight, wallData, wallContainer.transform);
            }
        }
    }

    void CreateAdaptiveWall(Vector3 position, float bottomHeight, float topHeight,
                           WallData wallData, Transform parent) {
        GameObject wall = new GameObject("Adaptive Wall");
        wall.transform.parent = parent;
        wall.transform.position = position;

        MeshFilter meshFilter = wall.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = wall.AddComponent<MeshRenderer>();

        // Generate wall mesh that fits between heights
        Mesh wallMesh = GenerateWallMesh(bottomHeight, topHeight, cellSize);
        meshFilter.mesh = wallMesh;

        // Add ladder if needed
        if (wallData.hasLadder) {
            GameObject ladder = Instantiate(ladderPrefab, position, Quaternion.identity, wall.transform);
            ladder.transform.localScale = new Vector3(1, topHeight - bottomHeight, 1);

            // Add climbing trigger
            var climbZone = ladder.AddComponent<ClimbingZone>();
            climbZone.climbSpeed = wallData.climbSpeed;
        }
    }
}

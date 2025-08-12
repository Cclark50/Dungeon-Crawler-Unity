public class SmartDrawingTools {
    // Auto-complete rooms
    public static void AutoCompleteRoom(DungeonData data, Vector2Int start, Vector2Int end) {
        // Draw rectangle
        for (int x = start.x; x <= end.x; x++) {
            PlaceWall(data, x, start.y, WallOrientation.Horizontal);
            PlaceWall(data, x, end.y + 1, WallOrientation.Horizontal);
        }

        for (int y = start.y; y <= end.y; y++) {
            PlaceWall(data, start.x, y, WallOrientation.Vertical);
            PlaceWall(data, end.x + 1, y, WallOrientation.Vertical);
        }
    }

    // Intelligent door placement
    public static void SmartDoor(DungeonData data, Vector2Int pos) {
        // Find walls adjacent to this position
        List<WallPlacement> adjacentWalls = GetAdjacentWalls(data, pos);

        if (adjacentWalls.Count == 2) {
            // Corner - place door on one wall
            var wall = adjacentWalls[0];
            SetWallType(data, wall, WallType.Door);
        } else if (adjacentWalls.Count == 1) {
            // Against single wall
            var wall = adjacentWalls[0];
            SetWallType(data, wall, WallType.Door);
        }
    }

    // Pattern fill
    public static void PatternFill(DungeonData data, Vector2Int start, Vector2Int end, PatternType pattern) {
        switch (pattern) {
            case PatternType.Maze:
                GenerateMaze(data, start, end);
                break;
            case PatternType.Pillars:
                GeneratePillars(data, start, end, spacing: 2);
                break;
            case PatternType.Cells:
                GenerateCells(data, start, end, cellSize: 3);
                break;
        }
    }
}

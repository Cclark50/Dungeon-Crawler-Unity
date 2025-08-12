public class VerticalConnectorPlacer {
    public static void AutoPlaceConnectors(DungeonData data) {
        for (int x = 0; x < data.width; x++) {
            for (int y = 0; y < data.height; y++) {
                CheckAndPlaceConnector(x, y, data);
            }
        }
    }

    static void CheckAndPlaceConnector(int x, int y, DungeonData data) {
        float currentHeight = data.cells[x, y].floorHeight;

        // Check all four directions
        foreach (Direction dir in System.Enum.GetValues(typeof(Direction))) {
            Vector2Int neighbor = GetNeighbor(x, y, dir);
            if (!IsValid(neighbor, data)) continue;

            float neighborHeight = data.cells[neighbor.x, neighbor.y].floorHeight;
            float heightDiff = neighborHeight - currentHeight;

            if (Mathf.Abs(heightDiff) < 0.1f) continue;  // Same height

            // Determine connector type based on height difference
            if (Mathf.Abs(heightDiff) <= 1f) {
                // Small difference - use stairs
                PlaceStairs(x, y, dir, data);
            } else if (Mathf.Abs(heightDiff) <= 3f) {
                // Medium difference - use ladder
                PlaceLadder(x, y, dir, data);
            } else {
                // Large difference - maybe a lift or teleporter
                PlaceSpecialConnector(x, y, dir, data);
            }
        }
    }

    static void PlaceStairs(int x, int y, Direction dir, DungeonData data) {
        // Check if there's room for stairs
        if (HasRoomForStairs(x, y, dir, data)) {
            data.cells[x, y].hasStairs = true;
            data.cells[x, y].stairDirection = dir;

            // Stairs take up space, mark adjacent cell
            Vector2Int stairEnd = GetNeighbor(x, y, dir);
            data.cells[stairEnd.x, stairEnd.y].partiallyOccupied = true;
        }
    }
}

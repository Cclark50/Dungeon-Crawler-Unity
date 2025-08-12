public class HeightAwareDungeonEditor : EditorWindow {
    // Height painting mode
    private float currentPaintHeight = 0f;
    private float heightStep = 0.5f;  // Snap to half-unit increments
    private bool showHeightOverlay = true;
    private bool autoGenerateStairs = true;

    // Visualization
    private Dictionary<float, Color> heightColors = new Dictionary<float, Color> {
        { -2f, new Color(0.2f, 0.2f, 0.5f) },  // Deep pit
        { -1f, new Color(0.3f, 0.3f, 0.6f) },  // Shallow pit
        { 0f, new Color(0.5f, 0.5f, 0.5f) },   // Ground level
        { 1f, new Color(0.6f, 0.5f, 0.4f) },   // Raised
        { 2f, new Color(0.7f, 0.6f, 0.3f) },   // Platform
        { 3f, new Color(0.8f, 0.7f, 0.2f) }    // High platform
    };

    void DrawHeightTools() {
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Height Tools", EditorStyles.boldLabel);

        // Height slider
        currentPaintHeight = EditorGUILayout.Slider("Paint Height", currentPaintHeight, -3f, 5f);

        // Snap to grid
        if (GUILayout.Button($"Snap to {heightStep}m grid")) {
            currentPaintHeight = Mathf.Round(currentPaintHeight / heightStep) * heightStep;
        }

        // Quick height buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Pit (-2)")) currentPaintHeight = -2f;
        if (GUILayout.Button("Lower (-1)")) currentPaintHeight = -1f;
        if (GUILayout.Button("Ground (0)")) currentPaintHeight = 0f;
        if (GUILayout.Button("Raised (1)")) currentPaintHeight = 1f;
        if (GUILayout.Button("Platform (2)")) currentPaintHeight = 2f;
        GUILayout.EndHorizontal();

        // Height tools
        autoGenerateStairs = EditorGUILayout.Toggle("Auto-add Stairs", autoGenerateStairs);
        showHeightOverlay = EditorGUILayout.Toggle("Show Heights", showHeightOverlay);

        // Vertical connectors
        GUILayout.Label("Vertical Connectors", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Ladder")) currentMode = DrawMode.Ladder;
        if (GUILayout.Button("Stairs")) currentMode = DrawMode.Stairs;
        if (GUILayout.Button("Ramp")) currentMode = DrawMode.Ramp;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    void DrawHeightOverlay() {
        if (!showHeightOverlay) return;

        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                float height = dungeonData.cells[x, y].floorHeight;

                // Color based on height
                Color heightColor = GetHeightColor(height);
                heightColor.a = 0.4f;

                Rect cellRect = GetCellRect(x, y);
                EditorGUI.DrawRect(cellRect, heightColor);

                // Draw height text
                if (height != 0) {
                    var style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontSize = 10;
                    style.normal.textColor = Color.white;

                    GUI.Label(cellRect, height.ToString("F1"), style);
                }

                // Draw height transitions
                DrawHeightTransitions(x, y);
            }
        }
    }

    void DrawHeightTransitions(int x, int y) {
        CellData cell = dungeonData.cells[x, y];

        // Check adjacent cells for height differences
        if (x < gridWidth - 1) {
            float heightDiff = dungeonData.cells[x + 1, y].floorHeight - cell.floorHeight;
            if (Mathf.Abs(heightDiff) > 0.1f) {
                DrawHeightTransitionLine(x, y, x + 1, y, heightDiff);
            }
        }

        if (y < gridHeight - 1) {
            float heightDiff = dungeonData.cells[x, y + 1].floorHeight - cell.floorHeight;
            if (Mathf.Abs(heightDiff) > 0.1f) {
                DrawHeightTransitionLine(x, y, x, y + 1, heightDiff);
            }
        }
    }

    void DrawHeightTransitionLine(int x1, int y1, int x2, int y2, float heightDiff) {
        Vector2 start = GridToScreen(x1, y1);
        Vector2 end = GridToScreen(x2, y2);
        Vector2 mid = (start + end) / 2;

        // Different line styles for different height changes
        if (Mathf.Abs(heightDiff) <= 0.5f) {
            // Small step - can walk
            Handles.color = Color.green;
            Handles.DrawLine(new Vector3(start.x, start.y), new Vector3(end.x, end.y), 2f);
        } else if (Mathf.Abs(heightDiff) <= 1.5f) {
            // Need stairs/ladder
            Handles.color = Color.yellow;
            Handles.DrawDottedLine(new Vector3(start.x, start.y), new Vector3(end.x, end.y), 3f);

            // Draw stair/ladder icon
            if (HasVerticalConnector(x1, y1, x2, y2)) {
                DrawIcon(mid, "ladder_icon");
            }
        } else {
            // Too high - need special connection
            Handles.color = Color.red;
            Handles.DrawDottedLine(new Vector3(start.x, start.y), new Vector3(end.x, end.y), 5f);
        }
    }

    void PaintHeight(Vector2Int gridPos) {
        if (!IsValidCell(gridPos)) return;

        float oldHeight = dungeonData.cells[gridPos.x, gridPos.y].floorHeight;
        dungeonData.cells[gridPos.x, gridPos.y].floorHeight = currentPaintHeight;

        // Auto-generate vertical connectors if needed
        if (autoGenerateStairs) {
            CheckAndAddVerticalConnectors(gridPos, oldHeight, currentPaintHeight);
        }

        // Adjust walls to match new height
        AdjustWallHeights(gridPos);
    }

    void CheckAndAddVerticalConnectors(Vector2Int pos, float oldHeight, float newHeight) {
        // Check all adjacent cells
        for (int dir = 0; dir < 4; dir++) {
            Vector2Int adjacent = pos + GetDirectionVector(dir);
            if (!IsValidCell(adjacent)) continue;

            float adjacentHeight = dungeonData.cells[adjacent.x, adjacent.y].floorHeight;
            float heightDiff = Mathf.Abs(adjacentHeight - newHeight);

            if (heightDiff > 0.5f && heightDiff <= 2f) {
                // Need connector
                if (heightDiff <= 1f) {
                    // Add stairs
                    AddStairs(pos, adjacent);
                } else {
                    // Add ladder
                    AddLadder(pos, adjacent);
                }
            }
        }
    }
}

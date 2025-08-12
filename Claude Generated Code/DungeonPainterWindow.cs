using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DungeonPainterWindow : EditorWindow {
    // Drawing modes
    private enum DrawMode {
        Wall,
        Erase,
        Door,
        Props,
        Encounters,
        Floors,
        Regions
    }

    private DrawMode currentMode = DrawMode.Wall;
    private WallType selectedWallType = WallType.Solid;
    private GameObject selectedProp;
    private FloorType selectedFloor = FloorType.Stone;
    private EncounterType selectedEncounter = EncounterType.Random;

    // Grid settings
    private int gridWidth = 50;
    private int gridHeight = 50;
    private float cellSize = 20f; // pixels per cell
    private Vector2 scrollPosition;
    private Vector2 gridOffset = new Vector2(10, 10);

    // Dungeon data
    private DungeonData dungeonData;
    private Dictionary<Vector2Int, List<PropPlacement>> props = new();

    // Drawing state
    private bool isDrawing;
    private Vector2Int lastGridPos;
    private List<Vector2Int> currentStroke = new();

    [MenuItem("Tools/Dungeon Painter")]
    public static void OpenWindow() {
        var window = GetWindow<DungeonPainterWindow>("Dungeon Painter");
        window.minSize = new Vector2(800, 600);
    }

    void OnEnable() {
        if (dungeonData == null) {
            dungeonData = CreateInstance<DungeonData>();
            dungeonData.Initialize(gridWidth, gridHeight);
        }
    }

    void OnGUI() {
        DrawToolbar();
        DrawCanvas();
        DrawSidebar();

        HandleInput();
    }

    void DrawToolbar() {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Mode buttons
        if (GUILayout.Toggle(currentMode == DrawMode.Wall, "Wall", EditorStyles.toolbarButton))
            currentMode = DrawMode.Wall;
        if (GUILayout.Toggle(currentMode == DrawMode.Door, "Door", EditorStyles.toolbarButton))
            currentMode = DrawMode.Door;
        if (GUILayout.Toggle(currentMode == DrawMode.Props, "Props", EditorStyles.toolbarButton))
            currentMode = DrawMode.Props;
        if (GUILayout.Toggle(currentMode == DrawMode.Floors, "Floors", EditorStyles.toolbarButton))
            currentMode = DrawMode.Floors;
        if (GUILayout.Toggle(currentMode == DrawMode.Encounters, "Encounters", EditorStyles.toolbarButton))
            currentMode = DrawMode.Encounters;
        if (GUILayout.Toggle(currentMode == DrawMode.Erase, "Erase", EditorStyles.toolbarButton))
            currentMode = DrawMode.Erase;

        GUILayout.FlexibleSpace();

        // Actions
        if (GUILayout.Button("Clear", EditorStyles.toolbarButton)) {
            if (EditorUtility.DisplayDialog("Clear Dungeon", "Clear all data?", "Clear", "Cancel")) {
                dungeonData.Clear();
            }
        }

        if (GUILayout.Button("Generate 3D", EditorStyles.toolbarButton)) {
            Generate3DDungeon();
        }

        EditorGUILayout.EndHorizontal();
    }

    void DrawCanvas() {
        Rect canvasRect = new Rect(200, 20, position.width - 400, position.height - 40);
        GUI.Box(canvasRect, GUIContent.none, EditorStyles.helpBox);

        // Scrollable grid area
        Rect viewRect = new Rect(0, 0, gridWidth * cellSize, gridHeight * cellSize);
        scrollPosition = GUI.BeginScrollView(canvasRect, scrollPosition, viewRect);

        // Draw grid
        DrawGrid();

        // Draw walls
        DrawWalls();

        // Draw props
        DrawProps();

        // Draw floor types
        DrawFloorTypes();

        // Draw encounter zones
        DrawEncounterZones();

        // Draw current stroke preview
        if (isDrawing && currentStroke.Count > 0) {
            DrawStrokePreview();
        }

        GUI.EndScrollView();
    }

    void DrawGrid() {
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);

        // Vertical lines
        for (int x = 0; x <= gridWidth; x++) {
            Vector2 start = GridToScreen(x, 0);
            Vector2 end = GridToScreen(x, gridHeight);
            Handles.DrawLine(new Vector3(start.x, start.y), new Vector3(end.x, end.y));
        }

        // Horizontal lines
        for (int y = 0; y <= gridHeight; y++) {
            Vector2 start = GridToScreen(0, y);
            Vector2 end = GridToScreen(gridWidth, y);
            Handles.DrawLine(new Vector3(start.x, start.y), new Vector3(end.x, end.y));
        }
    }

    void DrawWalls() {
        Handles.color = Color.white;

        // Draw horizontal walls
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y <= gridHeight; y++) {
                if (dungeonData.horizontalWalls[x, y].exists) {
                    DrawWallLine(x, y, x + 1, y, dungeonData.horizontalWalls[x, y].type);
                }
            }
        }

        // Draw vertical walls
        for (int x = 0; x <= gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                if (dungeonData.verticalWalls[x, y].exists) {
                    DrawWallLine(x, y, x, y + 1, dungeonData.verticalWalls[x, y].type);
                }
            }
        }
    }

    void DrawWallLine(float x1, float y1, float x2, float y2, WallType type) {
        Vector2 start = GridToScreen(x1, y1);
        Vector2 end = GridToScreen(x2, y2);

        // Different colors/styles for different wall types
        switch (type) {
            case WallType.Solid:
                Handles.color = Color.white;
                Handles.DrawLine(new Vector3(start.x, start.y), new Vector3(end.x, end.y), 3f);
                break;
            case WallType.Door:
                Handles.color = Color.yellow;
                DrawDottedLine(start, end, 5f);
                break;
            case WallType.Secret:
                Handles.color = Color.magenta;
                DrawDottedLine(start, end, 3f);
                break;
            case WallType.Bars:
                Handles.color = Color.cyan;
                DrawDottedLine(start, end, 2f);
                break;
        }
    }

    void HandleInput() {
        Event e = Event.current;
        Rect canvasRect = new Rect(200, 20, position.width - 400, position.height - 40);

        if (!canvasRect.Contains(e.mousePosition)) return;

        Vector2 mouseInCanvas = e.mousePosition - new Vector2(200, 20) + scrollPosition;
        Vector2Int gridPos = ScreenToGrid(mouseInCanvas);

        switch (e.type) {
            case EventType.MouseDown:
                if (e.button == 0) {
                    StartDrawing(gridPos);
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (isDrawing) {
                    ContinueDrawing(gridPos);
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                if (isDrawing) {
                    FinishDrawing();
                    e.Use();
                }
                break;
        }

        // Force repaint while drawing
        if (isDrawing) {
            Repaint();
        }
    }

    void StartDrawing(Vector2Int gridPos) {
        isDrawing = true;
        lastGridPos = gridPos;
        currentStroke.Clear();
        currentStroke.Add(gridPos);

        if (currentMode == DrawMode.Props) {
            PlaceProp(gridPos);
        } else if (currentMode == DrawMode.Floors) {
            PaintFloor(gridPos);
        }
    }

    void ContinueDrawing(Vector2Int gridPos) {
        if (gridPos == lastGridPos) return;

        if (currentMode == DrawMode.Wall || currentMode == DrawMode.Door || currentMode == DrawMode.Erase) {
            // Draw line from last position to current
            DrawLineBetween(lastGridPos, gridPos);
        } else if (currentMode == DrawMode.Floors) {
            PaintFloor(gridPos);
        } else if (currentMode == DrawMode.Encounters) {
            // Add to region
            currentStroke.Add(gridPos);
        }

        lastGridPos = gridPos;
    }

    void DrawLineBetween(Vector2Int start, Vector2Int end) {
        // Determine if this is a horizontal or vertical wall
        Vector2Int diff = end - start;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y)) {
            // Horizontal movement - place vertical walls
            int y = start.y;
            int startX = Mathf.Min(start.x, end.x);
            int endX = Mathf.Max(start.x, end.x);

            for (int x = startX; x <= endX; x++) {
                PlaceWall(new Vector2Int(x, y), WallOrientation.Vertical);
            }
        } else {
            // Vertical movement - place horizontal walls
            int x = start.x;
            int startY = Mathf.Min(start.y, end.y);
            int endY = Mathf.Max(start.y, end.y);

            for (int y = startY; y <= endY; y++) {
                PlaceWall(new Vector2Int(x, y), WallOrientation.Horizontal);
            }
        }
    }

    void PlaceWall(Vector2Int pos, WallOrientation orientation) {
        WallData wallData = null;

        if (orientation == WallOrientation.Horizontal) {
            if (pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y <= gridHeight) {
                wallData = dungeonData.horizontalWalls[pos.x, pos.y];
            }
        } else {
            if (pos.x >= 0 && pos.x <= gridWidth && pos.y >= 0 && pos.y < gridHeight) {
                wallData = dungeonData.verticalWalls[pos.x, pos.y];
            }
        }

        if (wallData != null) {
            if (currentMode == DrawMode.Erase) {
                wallData.exists = false;
            } else {
                wallData.exists = true;
                wallData.type = currentMode == DrawMode.Door ? WallType.Door : selectedWallType;
            }
        }
    }
}

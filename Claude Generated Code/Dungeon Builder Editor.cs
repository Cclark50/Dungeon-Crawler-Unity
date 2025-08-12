[CustomEditor(typeof(DungeonBuilder))]
public class DungeonBuilderEditor : Editor {
    private DungeonBuilder builder;
    private WallType selectedWallType = WallType.Solid;
    private bool editingMode = false;

    void OnSceneGUI() {
        if (!editingMode) return;

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("GridPlane"))) {
            Vector2Int gridPos = WorldToGrid(hit.point);
            WallPlacement placement = GetNearestWallPlacement(hit.point, gridPos);

            // Preview wall
            DrawWallPreview(placement);

            // Place wall on click
            if (e.type == EventType.MouseDown && e.button == 0) {
                PlaceWall(placement);
                e.Use();
            }
        }
    }

    void PlaceWall(WallPlacement placement) {
        // Update data
        builder.dungeonData.SetWall(placement.gridPos, placement.orientation, new WallData {
            exists = true,
            type = selectedWallType,
            prefabId = GetPrefabId(selectedWallType),
            behaviors = GetDefaultBehaviors(selectedWallType)
        });

        // Create preview GameObject (for editor only)
        if (builder.showPreview) {
            builder.wallRenderer.SpawnWall(placement.gridPos);
        }

        EditorUtility.SetDirty(builder.dungeonData);
    }
}

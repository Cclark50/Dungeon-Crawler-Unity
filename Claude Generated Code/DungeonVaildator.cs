public class DungeonValidator {
    public class ValidationResult {
        public bool isValid;
        public List<string> warnings = new();
        public List<string> errors = new();
    }

    public static ValidationResult Validate(DungeonData data) {
        var result = new ValidationResult { isValid = true };

        // Check for unreachable areas
        var reachableAreas = FindReachableAreas(data);
        if (reachableAreas.Count > 1) {
            result.warnings.Add($"Found {reachableAreas.Count} disconnected areas");
        }

        // Check for missing walls (open boundaries)
        var openBoundaries = FindOpenBoundaries(data);
        if (openBoundaries.Count > 0) {
            result.warnings.Add($"Found {openBoundaries.Count} open boundaries");
        }

        // Check door placement
        foreach (var door in FindDoors(data)) {
            if (!HasAdjacentWalls(door)) {
                result.errors.Add($"Door at {door.position} has no adjacent walls");
                result.isValid = false;
            }
        }

        // Check encounter zones
        foreach (var zone in data.encounterZones) {
            if (zone.cells.Count == 0) {
                result.errors.Add($"Encounter zone {zone.id} has no cells");
                result.isValid = false;
            }
        }

        return result;
    }

    // Quick play test from editor
    public static void QuickTest(DungeonData data) {
        // Save current scene
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        // Create test scene
        var testScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

        // Generate dungeon
        var generator = new GameObject().AddComponent<Dungeon3DGenerator>();
        var dungeon = generator.Generate3DDungeon(data);

        // Add player
        var playerPrefab = Resources.Load<GameObject>("Player/PlayerPrefab");
        var player = Instantiate(playerPrefab);
        player.transform.position = data.playerStartPosition * generator.cellSize;

        // Enter play mode
        EditorApplication.EnterPlaymode();
    }
}

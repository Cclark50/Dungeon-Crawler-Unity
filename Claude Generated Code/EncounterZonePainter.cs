[System.Serializable]
public class EncounterZone {
    public string id;
    public List<Vector2Int> cells = new();
    public EncounterType type;
    public int difficulty;
    public Color editorColor;

    [System.Serializable]
    public class EncounterData {
        public List<string> possibleEnemies;
        public int minEnemies;
        public int maxEnemies;
        public float triggerChance;
        public bool oneTimeOnly;
        public string specialScript;
    }

    public EncounterData data;
}

public class EncounterZonePainter {
    private Color currentColor = Color.red;
    private EncounterType currentType = EncounterType.Random;

    public void PaintEncounterZone(List<Vector2Int> cells) {
        var zone = new EncounterZone {
            id = System.Guid.NewGuid().ToString(),
            cells = new List<Vector2Int>(cells),
            type = currentType,
            editorColor = currentColor
        };

        dungeonData.encounterZones.Add(zone);
    }

    public void DrawEncounterOverlay() {
        foreach (var zone in dungeonData.encounterZones) {
            Color zoneColor = zone.editorColor;
            zoneColor.a = 0.3f;

            foreach (var cell in zone.cells) {
                Rect cellRect = GetCellRect(cell);
                EditorGUI.DrawRect(cellRect, zoneColor);
            }

            // Draw zone label
            Vector2 center = GetZoneCenter(zone);
            GUI.Label(GetCellRect(center), zone.type.ToString(), centeredStyle);
        }
    }
}

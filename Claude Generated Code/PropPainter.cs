[System.Serializable]
public class PropPlacement {
    public string propId;
    public Vector2Int gridPosition;
    public float rotation;
    public PropAnchor anchor; // Floor, Wall, Ceiling, Corner

    [System.Serializable]
    public class PropMetadata {
        public bool blockMovement;
        public bool isInteractable;
        public string interactionScript;
        public float lightRadius;
        public Color lightColor;
    }

    public PropMetadata metadata;
}

public class PropPainter {
    private PropLibrary propLibrary;

    public void DrawPropPalette(Rect rect) {
        GUILayout.BeginArea(rect);
        GUILayout.Label("Props", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var category in propLibrary.categories) {
            if (foldouts[category.name] = EditorGUILayout.Foldout(foldouts[category.name], category.name)) {
                foreach (var prop in category.props) {
                    Rect buttonRect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64));

                    if (GUI.Button(buttonRect, prop.icon)) {
                        selectedProp = prop;
                    }

                    // Highlight if selected
                    if (prop == selectedProp) {
                        EditorGUI.DrawRect(buttonRect, new Color(0, 1, 0, 0.3f));
                    }
                }
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    public void PlaceProp(Vector2Int gridPos, PropData prop) {
        var placement = new PropPlacement {
            propId = prop.id,
            gridPosition = gridPos,
            rotation = currentRotation,
            anchor = DetermineAnchor(prop, gridPos)
        };

        // Smart placement based on prop type
        if (prop.type == PropType.WallTorch) {
            // Snap to nearest wall
            placement.anchor = PropAnchor.Wall;
            placement.rotation = GetWallRotation(gridPos);
        } else if (prop.type == PropType.Chest) {
            // Face away from nearest wall
            placement.rotation = GetFacingRotation(gridPos);
        }

        dungeonData.AddProp(placement);
    }
}

public class HeightVisualization : MonoBehaviour {
    [Header("Height Indicators")]
    public bool showHeightGrid = true;
    public bool showDropShadows = true;
    public bool showHeightFog = true;

    void GenerateHeightIndicators(DungeonData data) {
        if (showHeightGrid) {
            // Create grid lines at different heights
            for (float h = -3f; h <= 5f; h += 1f) {
                CreateHeightGrid(h, GetColorForHeight(h));
            }
        }

        if (showDropShadows) {
            // Add shadows below elevated platforms
            foreach (var platform in FindElevatedAreas(data)) {
                CreateDropShadow(platform);
            }
        }

        if (showHeightFog) {
            // Add fog in pits
            foreach (var pit in FindPits(data)) {
                CreatePitFog(pit);
            }
        }
    }

    void CreateDropShadow(PlatformArea platform) {
        GameObject shadow = GameObject.CreatePrimitive(PrimitiveType.Quad);
        shadow.transform.rotation = Quaternion.Euler(90, 0, 0);
        shadow.transform.position = new Vector3(
            platform.center.x,
            0.01f,  // Just above ground
            platform.center.z
        );

        // Scale based on platform size
        shadow.transform.localScale = new Vector3(
            platform.width * cellSize,
            platform.length * cellSize,
            1
        );

        // Semi-transparent black material
        var mat = shadow.GetComponent<MeshRenderer>().material;
        mat.color = new Color(0, 0, 0, 0.3f);
        mat.renderQueue = 3000;  // Render on top
    }
}

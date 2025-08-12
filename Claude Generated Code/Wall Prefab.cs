// Wall prefab hierarchy example:
/*
WallPrefab
├── Mesh (with LOD group)
│   ├── LOD0_Detailed
│   ├── LOD1_Medium
│   └── LOD2_Simple
├── Collider
├── InteractionTrigger
├── Effects
│   ├── Torches
│   ├── Decorations
│   └── ParticleEffects
└── DoorComponents (disabled by default)
    ├── DoorMesh
    ├── DoorPivot
    └── DoorAnimator
*/

public class WallPrefabSetup : MonoBehaviour {
    [Header("Wall Variations")]
    public GameObject solidWallPrefab;
    public GameObject doorWallPrefab;
    public GameObject barsWallPrefab;
    public GameObject windowWallPrefab;

    [Header("Optimization")]
    public bool useLOD = true;
    public float[] lodDistances = { 10f, 20f, 50f };

    [Header("Batching")]
    public bool enableBatching = true;
    public Material[] sharedMaterials; // Use same materials for batching
}

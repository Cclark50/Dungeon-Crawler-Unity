public class WallController : MonoBehaviour {
    private WallData data;
    private Vector2Int gridPosition;
    private List<IWallBehavior> behaviors = new List<IWallBehavior>();

    // Components for different wall features
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider wallCollider;
    [SerializeField] private GameObject doorMesh;
    [SerializeField] private Transform pivotPoint;

    public void Initialize(WallData data, Vector2Int gridPos) {
        this.data = data;
        this.gridPosition = gridPos;

        // Clear old behaviors
        foreach (var behavior in behaviors) {
            behavior.Cleanup();
        }
        behaviors.Clear();

        // Add behaviors based on data
        foreach (var behaviorData in data.behaviors) {
            var behavior = WallBehaviorFactory.Create(behaviorData);
            behavior.Initialize(this, behaviorData);
            behaviors.Add(behavior);
        }

        UpdateVisuals();
    }

    void UpdateVisuals() {
        // Set material based on wall type
        meshRenderer.material = WallMaterialLibrary.Get(data.type);

        // Enable/disable components
        doorMesh.SetActive(data.type == WallType.Door);

        // Let behaviors modify visuals
        foreach (var behavior in behaviors) {
            behavior.UpdateVisuals(this);
        }
    }

    public void OnPlayerInteract(Player player) {
        foreach (var behavior in behaviors) {
            if (behavior is IInteractable interactable) {
                if (interactable.CanInteract(player)) {
                    interactable.OnInteract(player);

                    // Update data model
                    DungeonManager.Instance.UpdateWallData(gridPosition, data);

                    // Update visuals
                    UpdateVisuals();
                }
            }
        }
    }
}

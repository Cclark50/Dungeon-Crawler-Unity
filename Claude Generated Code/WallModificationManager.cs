public class WallModificationManager : MonoBehaviour {
    private DungeonData dungeonData;
    private WallRenderer wallRenderer;

    public void DestroyWall(Vector2Int position, WallOrientation orientation) {
        // Update data model
        var wallData = dungeonData.GetWall(position, orientation);
        wallData.exists = false;

        // Update visual
        if (wallRenderer.HasActiveWall(position)) {
            GameObject wallObj = wallRenderer.GetWall(position);

            // Play destruction effect
            var destruction = wallObj.GetComponent<DestructionEffect>();
            if (destruction != null) {
                destruction.Play(() => {
                    wallRenderer.ReturnToPool(wallObj);
                });
            } else {
                wallRenderer.ReturnToPool(wallObj);
            }
        }

        // Update pathfinding
        PathfindingManager.Instance.RecalculateGrid(position);
    }

    public void TransformWall(Vector2Int position, WallType newType) {
        var wallData = dungeonData.GetWall(position, orientation);
        wallData.type = newType;

        // If wall is active, update it
        if (wallRenderer.HasActiveWall(position)) {
            var wallObj = wallRenderer.GetWall(position);
            wallObj.GetComponent<WallController>().Initialize(wallData, position);
        }
    }
}

public class HeightAwareMovement : MonoBehaviour {
    [Header("Movement Settings")]
    public float maxStepHeight = 0.5f;  // Can walk up small steps
    public float climbSpeed = 2f;
    public float jumpHeight = 1.5f;

    private bool isClimbing = false;
    private bool isOnStairs = false;
    private float currentFloorHeight;

    public bool CanMoveTo(Vector2Int from, Vector2Int to, DungeonData data) {
        float fromHeight = data.cells[from.x, from.y].floorHeight;
        float toHeight = data.cells[to.x, to.y].floorHeight;
        float heightDiff = toHeight - fromHeight;

        // Check if there's a wall blocking
        if (HasWallBetween(from, to, data)) {
            var wall = GetWallBetween(from, to, data);

            // Can climb if wall has ladder
            if (wall.hasLadder && Mathf.Abs(heightDiff) <= 2f) {
                return true;
            }

            // Half walls can be vaulted if low enough
            if (wall.isHalfWall && wall.halfWallHeight <= jumpHeight) {
                return true;
            }

            return false;
        }

        // Check height difference
        if (heightDiff <= maxStepHeight) {
            // Can walk up
            return true;
        } else if (heightDiff < 0 && Mathf.Abs(heightDiff) <= jumpHeight * 2) {
            // Can drop down (with potential fall damage)
            return true;
        } else if (HasVerticalConnector(from, to, data)) {
            // Has stairs/ladder
            return true;
        }

        return false;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ladder")) {
            // Enable climbing mode
            isClimbing = true;
            GetComponent<Rigidbody>().useGravity = false;
        } else if (other.CompareTag("Stairs")) {
            isOnStairs = true;
        }
    }

    void HandleClimbing() {
        if (!isClimbing) return;

        float vertical = Input.GetAxis("Vertical");
        transform.position += Vector3.up * vertical * climbSpeed * Time.deltaTime;

        // Check if reached top/bottom
        if (Input.GetButtonDown("Jump") || ReachedLadderEnd()) {
            isClimbing = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
    }
}

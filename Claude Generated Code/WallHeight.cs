[System.Serializable]
public class CellData {
    public float floorHeight = 0f;  // Height in world units
    public float ceilingHeight = 3f;  // Relative to floor
    public FloorType floorType;
    public CeilingType ceilingType;

    // Vertical connections
    public bool hasLadderNorth;
    public bool hasLadderSouth;
    public bool hasLadderEast;
    public bool hasLadderWest;
    public bool hasStairs;
    public StairDirection stairDirection;

    // Special height features
    public bool isPit;  // Can fall through
    public bool isPlatform;  // Raised area
    public float platformHeight;  // Additional height if platform
}

[System.Serializable]
public class WallData {
    public bool exists;
    public WallType type;

    // Height information
    public float bottomHeight;  // Where wall starts
    public float topHeight;     // Where wall ends
    public bool extendsToFloor = true;  // Auto-adjust to floor height
    public bool extendsToCeiling = true;  // Auto-adjust to ceiling

    // Partial walls
    public bool isHalfWall;  // Can see over but not walk through
    public float halfWallHeight = 1.5f;

    // Vertical transitions
    public bool hasLadder;
    public bool hasStairs;
    public float climbSpeed = 2f;
}

public enum VerticalTransitionType {
    None,
    Ladder,
    Stairs,
    Ramp,
    Jump,  // One-way down
    Teleport
}

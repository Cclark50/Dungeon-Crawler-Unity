using UnityEngine;

public class GridMovement : MonoBehaviour
{
    
    FacingDirection _facingDirection = FacingDirection.North;
    Vector2Int _gridPosition = Vector2Int.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveForward()
    {
        
    }

    void MoveBackward()
    {
        
    }

    void StrafeLeft()
    {
        
    }

    void StrafeRight()
    {
        
    }

    void TurnLeft()
    {
        
    }

    void TurnRight()
    {
        
    }

    Vector2Int GetForwardPosition()
    {
        return new Vector2Int(0, 0);
    }

    Vector2Int GetBackwardPosition()
    {
        return new Vector2Int(0, 0);
    }

    Vector2Int GetLeftPosition()
    {
        return new Vector2Int(0, 0);
    }

    Vector2Int GetRightPosition()
    {
        return new Vector2Int(0, 0);
    }

    Vector3 GetWorldPosition()
    {
        return Vector3.zero;
    }
}

public enum FacingDirection{North,South,East,West}
public enum WorldDirection{North,South,East,West}
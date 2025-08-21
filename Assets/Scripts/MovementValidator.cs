using Unity.VisualScripting;
using UnityEngine;

public class MovementValidator : MonoBehaviour
{
    private GridMovement _gridMovement;
    public DirectionResolver _directionResolver;
    
    public Vector2Int _minBounds = new Vector2Int(-10, -10);
    public Vector2Int _maxBounds = new Vector2Int(10, 10);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gridMovement = new GridMovement();
        _gridMovement.OnMovementRequested += HandleMovementRequest;
        _directionResolver = CoreGame.Instance._directionResolver;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        _gridMovement.OnMovementRequested -= HandleMovementRequest;
    }

    private bool IsWithinBounds(Vector2Int targetVec)
    {
        return ((targetVec.x <= _maxBounds.x && targetVec.x >= _minBounds.x) && (targetVec.y <= _maxBounds.y || targetVec.y >= _minBounds.y));
    }

    private void HandleMovementRequest(MovementRequestedEventArgs e)
    {
        Vector2Int requestedVec = _directionResolver.GetMovementVector(e._movementIntent.fromDirection, e._movementIntent.movementType);
        Vector2Int targetVec= e._movementIntent.fromPosition + requestedVec;
        if (IsWithinBounds(targetVec))
        {
            e._GridMovement.ExecuteMovement(e._movementIntent, targetVec);
        }
    }
}

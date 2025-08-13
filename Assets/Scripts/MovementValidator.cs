using Unity.VisualScripting;
using UnityEngine;

public class MovementValidator : MonoBehaviour
{
    private GridMovement _gridMovement;
    public DirectionResolver _directionResolver;
    
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

    private void HandleMovementRequest(MovementRequestedEventArgs e)
    {
        Vector2Int requestedVec = _directionResolver.GetMovementVector(e._movementIntent.fromDirection, e._movementIntent.movementType);
        Vector2Int targetVec= e._movementIntent.fromPosition + requestedVec;
        if ((targetVec.x > 10 || targetVec.x < -10) && (targetVec.y > 10 || targetVec.y < -10))
        {
            e._GridMovement.ExecuteMovement(e._movementIntent, targetVec);
        }
    }
}

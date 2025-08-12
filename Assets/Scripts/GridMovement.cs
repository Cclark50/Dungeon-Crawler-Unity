using System;
using System.Runtime.CompilerServices;
using UnityEditorInternal;
using UnityEngine;

public class GridMovement 
{
    
    FacingDirection _facingDirection = FacingDirection.North;
    Vector2Int _gridPosition = Vector2Int.zero;
    public event Action<MovementRequestedEventArgs> OnMovementRequested;
    public event Action<MovementExecutedEventArgs> OnMovementExecuted;
    
    void RequestMoveForward()
    {
        MovementIntent intent = new MovementIntent(MovementType.Forward, _gridPosition, _facingDirection);
        MovementRequestedEventArgs request = new MovementRequestedEventArgs(this, intent, Time.time);
        OnMovementRequested?.Invoke(request);
    }

    void RequestMoveBackward()
    {
        MovementIntent intent = new MovementIntent(MovementType.Backward, _gridPosition, _facingDirection);
        MovementRequestedEventArgs request = new MovementRequestedEventArgs(this, intent, Time.time);
        OnMovementRequested?.Invoke(request);
    }

    void RequestStrafeLeft()
    {
        MovementIntent intent = new MovementIntent(MovementType.StrafeLeft, _gridPosition, _facingDirection);
        MovementRequestedEventArgs request = new MovementRequestedEventArgs(this, intent, Time.time);
        OnMovementRequested?.Invoke(request);
    }

    void RequestStrafeRight()
    {
        MovementIntent intent = new MovementIntent(MovementType.StrafeRight, _gridPosition, _facingDirection);
        MovementRequestedEventArgs request = new MovementRequestedEventArgs(this, intent, Time.time);
        OnMovementRequested?.Invoke(request);
    }

    void ExecuteMovement(MovementIntent intent, Vector2Int toGridPosition)
    {
        Vector2Int oldPosition = _gridPosition;
        _gridPosition = toGridPosition;
        
        OnMovementExecuted?.Invoke(new MovementExecutedEventArgs(
            intent,
            oldPosition,
            _gridPosition,
            _facingDirection
            ));
    }

    void TurnLeft()
    {
        _facingDirection = _facingDirection.TurnLeft();
    }

    void TurnRight()
    {
        _facingDirection = _facingDirection.TurnRight();
    }

    void SetPosition(Vector2Int position)
    {
        _gridPosition = position;
    }

    void SetFacingDirection(FacingDirection facingDirection)
    {
        _facingDirection = facingDirection;
    }

    Vector3 GetWorldPosition()
    {
        return WorldToGridConverter.GridToWorld(_gridPosition);
    }
}

public class MovementExecutedEventArgs
{
    MovementIntent _intent;
    Vector2Int _oldPosition;
    Vector2Int _newPosition;
    FacingDirection _facingDirection;

    public MovementExecutedEventArgs(MovementIntent intent, Vector2Int oldPosition, Vector2Int newPosition, FacingDirection dir)
    {
        _intent = intent;
        _oldPosition = oldPosition;
        _newPosition = newPosition;
        _facingDirection = dir;
    }
}

public enum FacingDirection{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

public enum MovementType
{
    Forward = 0,
    StrafeRight = 1,
    Backward = 2,
    StrafeLeft = 3
}

public struct MovementIntent
{
    public MovementType movementType;
    public Vector2Int fromPosition;
    public FacingDirection fromDirection;

    public MovementIntent(MovementType movementType, Vector2Int fromPosition, FacingDirection fromDirection)
    {
        this.movementType = movementType;
        this.fromDirection = fromDirection;
        this.fromPosition = fromPosition;
    }
}

public class MovementRequestedEventArgs
{
    public MovementIntent _movementIntent;
    public GridMovement _GridMovement;
    public float _time;

    public MovementRequestedEventArgs(GridMovement gridMovement, MovementIntent movementIntent, float time)
    {
        _movementIntent = movementIntent;
        _GridMovement = gridMovement;
        _time = time;
    }
}

public static class DirectionExtensions
{
    public static FacingDirection TurnLeft(this FacingDirection direction)
    {
        return (FacingDirection)(((int)direction + 1) % 4);
    }
    
    public static FacingDirection TurnRight(this FacingDirection direction)
    {
        return (FacingDirection)(((int)direction + 3) % 4);
    }
}
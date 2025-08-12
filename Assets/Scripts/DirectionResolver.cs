using System;
using UnityEngine;

public class DirectionResolver 
{
	public Vector2Int _northVec = Vector2Int.up; //(0,1) 0
	public Vector2Int _eastVec = Vector2Int.right; //(1,0) 1
	public Vector2Int _southVec = Vector2Int.down; //(0,-1) 2
	public Vector2Int _westVec = Vector2Int.left; //(-1,0) 3

	public Vector2Int GetMovementVector(FacingDirection facing, MovementType type)
	{
		FacingDirection targetDir = (FacingDirection)(((int)facing + (int)type) % 4);
		return GetDirectionArray()[(int)targetDir];
	}

	public Vector2Int[] GetDirectionArray()
	{
		return new[] {_northVec,_eastVec,_southVec,_westVec};
	}
}

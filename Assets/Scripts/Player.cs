using UnityEngine;

public class Player : MonoBehaviour
{
    
    public MovementValidator _movementValidator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _movementValidator._gridMovement.RequestMoveForward();
        }
    }
}

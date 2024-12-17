using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f;
    public GridManager gridManager;
    
    private Vector2Int _currentGridPosition;
    private Vector3 _targetPosition;
    private bool _isMoving;
    
    public Vector2Int startingPosition = new Vector2Int(0, 7);
    
    public bool reachedFinish = false;
    public bool died = false;
    
    private void Start()
    {
        _currentGridPosition = startingPosition;
        transform.position = new Vector3(startingPosition.x, startingPosition.y, 0);
    }

    private void Update()
    {
        if (!_isMoving)
        {
            if (Input.GetKey(KeyCode.W)) // Move up
                Move(Vector2Int.up);
            else if (Input.GetKey(KeyCode.S)) // Move down
                Move(Vector2Int.down);
            else if (Input.GetKey(KeyCode.A)) // Move left
                Move(Vector2Int.left);
            else if (Input.GetKey(KeyCode.D)) // Move right
                Move(Vector2Int.right);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            if (transform.position == _targetPosition)
            {
                _isMoving = false;
                if (!gridManager.IsSafe(_currentGridPosition.x, _currentGridPosition.y))
                {
                    died = true;
                }
            }
        }
    }

    private void Move(Vector2Int direction)
    {
        var newGridPosition = _currentGridPosition + direction;

        newGridPosition.x = Mathf.Clamp(newGridPosition.x, 0, gridManager.gridWidth - 1);
        newGridPosition.y = Mathf.Clamp(newGridPosition.y, 0, gridManager.gridHeight - 1);

        if (!gridManager.IsObstacleAt(newGridPosition.x, newGridPosition.y))
        {
            _currentGridPosition = newGridPosition;
            _targetPosition = new Vector3(newGridPosition.x, newGridPosition.y, transform.position.z);
            _isMoving = true;
        }
    }
}

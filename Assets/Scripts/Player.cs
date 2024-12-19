using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f;
    public GridManager gridManager;
    
    private Vector2Int _currentGridPosition;
    private Vector3 _targetPosition;
    private bool _isMoving;
    
    public Vector2Int startingPosition = new Vector2Int(0, 7);
    public Vector2Int finishPosition = new Vector2Int(7, 0);
    
    public bool reachedFinish = false;
    public bool died = false;
    public bool disappear = false;
    public bool disappeared = false;
    
    public Sprite playerSprite;
    public Sprite playerBackSprite;
    public Sprite playerFinishedSprite;
    
    private Animator animator;
    
    private void Start()
    {
        _currentGridPosition = startingPosition;
        transform.position = new Vector3(startingPosition.x, startingPosition.y, 1.5f);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_isMoving)
        {
            animator.SetBool("isMoving", false);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                // Move up
                Move(Vector2Int.up);
                animator.SetBool("isMovingUp", true);
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                // Move down
                Move(Vector2Int.down);
                animator.SetBool("isMovingUp", false);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                // Move left
                Move(Vector2Int.left);
                animator.SetBool("isMovingUp", false);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                // Move right
                Move(Vector2Int.right);
                animator.SetBool("isMovingUp", false);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
            if (transform.position == _targetPosition)
            {
                _isMoving = false;
                
                var finished = finishPosition == _currentGridPosition;
                if (finished)
                {
                    StartCoroutine(Finish());
                    return;
                }
                
                var isAtStart = startingPosition == _currentGridPosition;
                if (!isAtStart && !gridManager.IsSafe(_currentGridPosition.x, _currentGridPosition.y))
                {
                    StartCoroutine(Die());
                }
            }
            animator.SetBool("isMoving", true);
        }

        if (disappear)
        {
            StartCoroutine(Disappear());
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

    private IEnumerator Finish()
    {
        animator.SetBool("isFinished", true);
        moveSpeed = 0;
        
        while (!IsAnimationFinished("Finish"))
        {
            yield return null;
        }
        
        reachedFinish = true;
    }
    
    public IEnumerator Die()
    {
        animator.SetBool("isDead", true);
        moveSpeed = 0;
        
        while (!IsAnimationFinished("Death"))
        {
            yield return null;
        }

        died = true;
    }
    
    public IEnumerator Disappear()
    {
        animator.SetBool("disappear", true);
        moveSpeed = 0;
        
        while (!IsAnimationFinished("Disappear"))
        {
            yield return null;
        }
        disappeared = true;
    }
    
    private bool IsAnimationFinished(string animationName)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }
}

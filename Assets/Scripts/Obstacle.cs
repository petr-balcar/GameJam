using System;
using UnityEditor;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GridManager gridManager;

    public int x;
    public int y;
    private bool _isDragging = false;

    private void Start()
    {
        var position = transform.position;
        x = (int) position.x;
        y = (int) position.y;
    }

    private void Update()
    {
        if (_isDragging)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }
    }

    public void OnMouseDown()
    {
        _isDragging = true;
    }

    public void OnMouseUp()
    {
        _isDragging = false;

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var newX = gridManager.GetGridCoordinate(mousePosition.x);
        var newY = gridManager.GetGridCoordinate(mousePosition.y);
        
        gridManager.MoveObstacle(this, newX, newY);
    }
}

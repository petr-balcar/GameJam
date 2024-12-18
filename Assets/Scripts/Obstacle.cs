using System;
using UnityEditor;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GridManager gridManager;

    public int x;
    public int y;
    private bool _isDragging = false;

    public Sprite shadow;
    [Range(0, 1)] public float shadowOpacity = 1.0f;
    public Vector3 shadowScale = new Vector3(1.5f, 1.5f, 1);
    public Vector3 shadowPosition = new Vector3(1, 0, 0);

    private SpriteRenderer _shadowRenderer;
    
    private void Start()
    {
        var position = transform.position;
        x = (int) position.x;
        y = (int) position.y;
        
        GameObject childObject = new GameObject("ChildObject");
        childObject.transform.parent = transform;
        childObject.transform.localScale = shadowScale;
        childObject.transform.localPosition = shadowPosition;
        _shadowRenderer = childObject.AddComponent<SpriteRenderer>();
        _shadowRenderer.sprite = shadow;
        _shadowRenderer.sortingOrder = 0;
        _shadowRenderer.color = new Color(1f, 1f, 1f, shadowOpacity); // Set the opacity
        _shadowRenderer.enabled = false;
    }

    private void Update()
    {
        if (gridManager.IsDay())
        {
            _shadowRenderer.enabled = true;
            return;
        }
        else
        {
            _shadowRenderer.enabled = false;
        }
        
        if (_isDragging)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }
    }

    public void OnMouseDown()
    {
        if (gridManager.IsDay()) return;
        _isDragging = true;
    }

    public void OnMouseUp()
    {
        if (gridManager.IsDay()) return;
        
        _isDragging = false;

        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var newX = gridManager.GetGridCoordinate(mousePosition.x);
        var newY = gridManager.GetGridCoordinate(mousePosition.y);
        
        gridManager.MoveObstacle(this, newX, newY);
    }
}

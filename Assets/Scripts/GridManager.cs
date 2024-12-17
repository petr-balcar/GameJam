using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject obstaclePrefab;
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float cellSize = 1.0f;

    public Vector2Int finish = new Vector2Int(8, 0);
    
    private GameObject[,] _grid;
    private bool[,] _obstacleMap;

    private void Start()
    {
        _grid = new GameObject[gridWidth, gridHeight];
        _obstacleMap = new bool[gridWidth, gridHeight];

        GenerateGrid();
        PlaceObstacle(0, 0);
        PlaceObstacle(4, 4);
        PlaceObstacle(3, 3);
        PlaceObstacle(1, 2);
    }

    private void GenerateGrid()
    {
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                var position = new Vector3(x * cellSize, y * cellSize, 1);
                _grid[x, y] = Instantiate(cellPrefab, position, Quaternion.identity); 
                _grid[x, y].transform.parent = transform;
                _obstacleMap[x, y] = false;
            }
        }
    }

    public bool IsInGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }
    
    public bool IsObstacleAt(int x, int y)
    {
        return IsInGrid(x, y) ? _obstacleMap[x, y] : false; 
    }
    
    public bool IsSafe(int x, int y)
    {
        // A tile is safe if it is in a shadow (two tiles left from an obstacle)
        return (IsObstacleAt(x + 1, y) || IsObstacleAt(x - 1, y) || 
                IsObstacleAt(x, y + 1) || IsObstacleAt(x - 1, y + 1) || IsObstacleAt(x + 1, y + 1) ||
                IsObstacleAt(x, y - 1) || IsObstacleAt(x - 1, y - 1) || IsObstacleAt(x + 1, y - 1));
    }
    
    private void PlaceObstacle(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight && !_obstacleMap[x, y])
        {
            _obstacleMap[x, y] = true;
            var position = new Vector3(x * cellSize, y * cellSize, 0);
            var obstacleObject = Instantiate(obstaclePrefab, position, Quaternion.identity);
            var obstacle = obstacleObject.GetComponent<Obstacle>();
            obstacle.gridManager = this;
        }
    }

    public int GetGridCoordinate(float x)
    {
        return (int) Math.Round(x, 0, MidpointRounding.AwayFromZero);
    }
    
    public void MoveObstacle(Obstacle obstacle, int x, int y)
    {
        if (!IsObstacleAt(x, y))
        {
            _obstacleMap[obstacle.x, obstacle.y] = false;
            _obstacleMap[x, y] = true;

            obstacle.transform.position = new Vector3(x, y, 0);
            
            obstacle.x = x;
            obstacle.y = y;
        }
        else
        {
            obstacle.transform.position = new Vector3(obstacle.x, obstacle.y, 0);
        }
    }

    private void ChangeCellBrightness(GameObject cell, float factor)
    {
        var cellRenderer = cell.GetComponent<Renderer>();
        if (cellRenderer != null)
        {
            Color color = cellRenderer.material.color;
            color.r = Mathf.Clamp01(color.r * factor);
            color.g = Mathf.Clamp01(color.g * factor);
            color.b = Mathf.Clamp01(color.b * factor);
            cellRenderer.material.color = color;
        }
    }

    private void ChangeCellBrightness(int x, int y, float factor)
    {
        if (IsInGrid(x, y) && !IsObstacleAt(x, y))
            ChangeCellBrightness(_grid[x, y], factor);
    }
    
    private void EnableShadow(int x, int y)
    {
        ChangeCellBrightness(x - 1, y, 0.5f);
        ChangeCellBrightness(x + 1, y, 0.5f);
        ChangeCellBrightness(x, y + 1, 0.5f);
        ChangeCellBrightness(x - 1, y + 1, 0.5f);
        ChangeCellBrightness(x + 1, y + 1, 0.5f);
        ChangeCellBrightness(x, y - 1, 0.5f);
        ChangeCellBrightness(x - 1, y - 1, 0.5f);
        ChangeCellBrightness(x + 1, y - 1, 0.5f);
    }
    
    private void DisableShadow(int x, int y)
    {
        ChangeCellBrightness(x - 1, y, 2f);
        ChangeCellBrightness(x + 1, y, 2f);
        ChangeCellBrightness(x, y + 1, 2f);
        ChangeCellBrightness(x - 1, y + 1, 2f);
        ChangeCellBrightness(x + 1, y + 1, 2f);
        ChangeCellBrightness(x, y - 1, 2f);
        ChangeCellBrightness(x - 1, y - 1, 2f);
        ChangeCellBrightness(x + 1, y - 1, 2f);
    }

    public void EnableShadows()
    {
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                if (_obstacleMap[x, y] == true)
                    EnableShadow(x, y);
            }
        }
    }
    
    public void DisableShadows()
    {
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                if (_obstacleMap[x, y] == true)
                    DisableShadow(x, y);
            }
        }
    }
}

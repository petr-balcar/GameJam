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
    
    public bool IsObstacle(int x, int y)
    {
        return _obstacleMap[x, y];
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
        if (IsInGrid(x, y) && !IsObstacle(x, y))
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

    public void ChangeCellBrightness(GameObject cell, float factor)
    {
        var renderer = cell.GetComponent<Renderer>();
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.r = Mathf.Clamp01(color.r * factor);
            color.g = Mathf.Clamp01(color.g * factor);
            color.b = Mathf.Clamp01(color.b * factor);
            renderer.material.color = color;
        }
    }
    
    private void EnableShadow(int x, int y)
    {
        if (IsInGrid(x - 1, y) && !IsObstacle(x - 1, y))
            ChangeCellBrightness(_grid[x - 1, y], 0.5f);
        
        if (IsInGrid(x - 2, y) && !IsObstacle(x - 2, y))
            ChangeCellBrightness(_grid[x - 2, y], 0.8f);
    }
    
    private void DisableShadow(int x, int y)
    {
        if (IsInGrid(x - 1, y) && !IsObstacle(x - 1, y))
            ChangeCellBrightness(_grid[x - 1, y], 2f);
        
        if (IsInGrid(x - 2, y)  && !IsObstacle(x - 2, y))
            ChangeCellBrightness(_grid[x - 2, y], 1.25f);
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
    
    public bool IsObstacleAt(int x, int y)
    {
        if (IsInGrid(x, y))
        {
            return _obstacleMap[x, y];
        }
        return false;
    }
}

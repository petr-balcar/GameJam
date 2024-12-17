using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject obstaclePrefab;
    public GameObject garlicPrefab;
    public GameObject crossPrefab;
    
    public GameManager gameManager;
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float cellSize = 1.0f;

    public Vector2Int finish = new Vector2Int(8, 0);
    
    private GameObject[,] _grid;
    private bool[,] _obstacleMap;
    private bool[,] _garlicMap;
    private bool[,] _crossMap;

    private void Start()
    {
        _grid = new GameObject[gridWidth, gridHeight];
        _obstacleMap = new bool[gridWidth, gridHeight];
        _garlicMap = new bool[gridWidth, gridHeight];
        _crossMap = new bool[gridWidth, gridHeight];
        
        GenerateGrid();
        
        PlaceObstacle(1, 6);
        PlaceObstacle(6, 6);
        PlaceObstacle(1, 1);
        PlaceObstacle(1, 3);
        
        PlaceGarlic(3,5);
        
        PlaceCross(2,2);
    }

    private void GenerateGrid()
    {
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                var position = new Vector3(x * cellSize, y * cellSize, 2);
                _grid[x, y] = Instantiate(cellPrefab, position, Quaternion.identity); 
                _grid[x, y].transform.parent = transform;
                _obstacleMap[x, y] = false;
            }
        }
        
        // Instantiate(cellPrefab, new Vector3(-1 * cellSize, 7 * cellSize, 2), Quaternion.identity);
        // Instantiate(cellPrefab, new Vector3(8 * cellSize, 0 * cellSize, 2), Quaternion.identity);
    }

    public bool IsDay()
    {
        return gameManager.isDay;
    }
    
    public bool IsInGrid(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }
    
    public bool IsObstacleAt(int x, int y)
    {
        return IsInGrid(x, y) ? _obstacleMap[x, y] : false; 
    }
    
    public bool IsGarlicAt(int x, int y)
    {
        return IsInGrid(x, y) ? _garlicMap[x, y] : false; 
    }

    public bool IsCrossAt(int x, int y)
    {
        return IsInGrid(x, y) ? _crossMap[x, y] : false; 
    }
    
    public bool isEmpty(int x, int y)
    {
        return !IsObstacleAt(x, y) && !IsGarlicAt(x, y) && !IsCrossAt(x, y);
    }
    
    public bool IsInShadow(int x, int y)
    {
        return (IsObstacleAt(x + 1, y) || IsObstacleAt(x - 1, y) || 
                IsObstacleAt(x, y + 1) || IsObstacleAt(x - 1, y + 1) || IsObstacleAt(x + 1, y + 1) ||
                IsObstacleAt(x, y - 1) || IsObstacleAt(x - 1, y - 1) || IsObstacleAt(x + 1, y - 1));
    }

    // Cross follows the same rules as a rook in chess, and can be blocked by obstacles
    public bool IsCrossSafe(int x, int y)
    {
        var right = x;
        while (right < gridWidth)
        {
            if (_crossMap[right, y]) return false;
            if (_obstacleMap[right, y]) break;
            right++;
        }
        
        var left = x - 1;
        while (left >= 0)
        {
            if (_crossMap[left, y]) return false;   
            if (_obstacleMap[left, y]) break;
            left--;
        }
        
        var up = y;
        while (up < gridHeight)
        {
            if (_crossMap[x, up]) return false;  
            if (_obstacleMap[x, up]) break;
            up++;
        }
        
        var down = y - 1;
        while (down >= 0)
        {
            if (_crossMap[x, down]) return false;
            if (_obstacleMap[x, down]) break;
            down--;
        }
        
        return true;
    }
    
    public bool IsNearGarlic(int x, int y)
    {
        return IsGarlicAt(x, y);
    }
    
    public bool IsSafe(int x, int y)
    {
        return IsInShadow(x, y) && !IsNearGarlic(x, y) && IsCrossSafe(x, y);
    }
    
    private void PlaceObstacle(int x, int y)
    {
        if (IsInGrid(x, y) && !IsObstacleAt(x, y))
        {
            _obstacleMap[x, y] = true;
            var position = new Vector3(x * cellSize, y * cellSize, 0);
            var obstacleObject = Instantiate(obstaclePrefab, position, Quaternion.identity);
            var obstacle = obstacleObject.GetComponent<Obstacle>();
            obstacle.gridManager = this;
        }
    }

    private void PlaceGarlic(int x, int y)
    {
        _garlicMap[x, y] = true;
        var position = new Vector3(x * cellSize, y * cellSize, 1);
        Instantiate(garlicPrefab, position, Quaternion.identity);
    }
    
    private void PlaceCross(int x, int y)
    {
        _crossMap[x, y] = true;
        var position = new Vector3(x * cellSize, y * cellSize, 1);
        Instantiate(crossPrefab, position, Quaternion.identity);
    }
    
    public int GetGridCoordinate(float x)
    {
        return (int) Math.Round(x, 0, MidpointRounding.AwayFromZero);
    }
    
    public void MoveObstacle(Obstacle obstacle, int x, int y)
    {
        if (IsInGrid(x, y) && isEmpty(x, y))
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

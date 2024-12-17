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

        Generate_grid();
        PlaceObstacle(0, 0);
        PlaceObstacle(4, 4);
        PlaceObstacle(3, 3);
        PlaceObstacle(1, 2);
    }

    private void Generate_grid()
    {
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                var position = new Vector3(x * cellSize, y * cellSize, 0);
                _grid[x, y] = Instantiate(cellPrefab, position, Quaternion.identity); 
                _grid[x, y].transform.parent = transform;
                _obstacleMap[x, y] = false;
            }
        }
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
        var isInGrid = x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        var isEmpty = _obstacleMap[x, y] == false;
        
        if (isInGrid && isEmpty)
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
    
    public bool IsObstacleAt(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            return _obstacleMap[x, y];
        }
        return false;
    }
}

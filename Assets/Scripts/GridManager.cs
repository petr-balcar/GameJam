using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject obstaclePrefab;
    public GameObject garlicPrefab;
    public GameObject crossPrefab;

    public Sprite dangerCell;
    public Sprite goldCell;
    public Sprite gridCell1;
    public Sprite gridCell2;
    public Sprite gridCell3;
    public Sprite gridCell4;
    
    public GameManager gameManager;
    public int gridWidth = 8;
    public int gridHeight = 8;
    public float cellSize = 1.0f;

    public Vector2Int finish = new Vector2Int(7, 0);
    
    private GameObject[,] _grid;
    private bool[,] _obstacleMap;
    private bool[,] _garlicMap;
    private bool[,] _crossMap;

    private List<GameObject> _objects = new List<GameObject>();
    
    private void Start()
    {
        _grid = new GameObject[gridWidth, gridHeight];
        _obstacleMap = new bool[gridWidth, gridHeight];
        _garlicMap = new bool[gridWidth, gridHeight];
        _crossMap = new bool[gridWidth, gridHeight];

        GenerateGrid();
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
                _garlicMap[x, y] = false;
                _crossMap[x, y] = false;
            }
        }
    }

    private void ClearLevel()
    {
        foreach (var gameObj in _objects) {
            Destroy(gameObj);
        }
        
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                _obstacleMap[x, y] = false;
                _garlicMap[x, y] = false;
                _crossMap[x, y] = false;
            }
        }
    }
    
    public void LoadLevel(int level)
    {
        ClearLevel();
        
        switch (level)
        {
            case 1:
                LoadLevel1();
                break;
            case 2:
                LoadLevel2();
                break;
            case 3:
                LoadLevel3();
                break;
            case 4:
                LoadLevel4();
                break;
        }
        
        ColorAll();
        ColorDanger();
    }

    private void LoadLevel1()
    {
        PlaceObstacle(1, 6);
        PlaceObstacle(6, 6);
        PlaceObstacle(1, 1);
    }
    
    private void LoadLevel2()
    {
        PlaceObstacle(1, 6);
        PlaceObstacle(6, 6);
        PlaceObstacle(1, 1);
        PlaceGarlic(3,5);
        PlaceGarlic(3,6);
        PlaceGarlic(3,7);
        PlaceGarlic(2,1);
    }
    
    private void LoadLevel3()
    {
        PlaceObstacle(1, 6);
        PlaceObstacle(6, 6);
        PlaceObstacle(6, 6);
        PlaceObstacle(1, 3);
        PlaceCross(2,2);
    }
    
    private void LoadLevel4()
    {
        PlaceObstacle(1, 6);
        PlaceObstacle(6, 6);
        PlaceObstacle(6, 6);
        PlaceObstacle(1, 3);
        PlaceGarlic(3,5);
        PlaceCross(2,2);
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
            var position = new Vector3(x * cellSize, y * cellSize, 1);
            var obstacleObject = Instantiate(obstaclePrefab, position, Quaternion.identity);
            obstacleObject.transform.Rotate(Random.Range(0, 3) * 180,Random.Range(0, 3) * 180, 0);
            var obstacle = obstacleObject.GetComponent<Obstacle>();
            obstacle.gridManager = this;
            _objects.Add(obstacleObject);
        }
    }

    private void PlaceGarlic(int x, int y)
    {
        _garlicMap[x, y] = true;
        var position = new Vector3(x * cellSize, y * cellSize, 1.6f);
        var garlicObject = Instantiate(garlicPrefab, position, Quaternion.identity);
        _objects.Add(garlicObject);
    }

    private void ColorCell(int x, int y, string type)
    {
        var cell = _grid[x, y];
        var cellRenderer = cell.GetComponent<SpriteRenderer>();

        string previousType = "";
        if (cellRenderer.sprite is not null)
            previousType = cellRenderer.sprite.ToString();

        if (type == "danger")
        {
            cellRenderer.sprite = dangerCell;
        }
        else if (type == "normal")
        {
            var variant = Random.Range(1,10);
            string[] grassCells = { gridCell1.ToString(), gridCell2.ToString(), gridCell3.ToString(), gridCell4.ToString() };
            
            if (grassCells.Contains(previousType)) return;
            if (previousType == dangerCell.ToString()) variant = 9; // Set to plain square

            switch (variant)
            {
                case 1:
                    cellRenderer.sprite = gridCell2;
                    break;
                case 2:
                    cellRenderer.sprite = gridCell3;
                    break;
                case 3:
                case 4:
                    cellRenderer.sprite = gridCell4;
                    break;
                default:
                    cellRenderer.sprite = gridCell1;
                    break;
            }
        }
        else if (type == "gold")
        {
            cellRenderer.sprite = goldCell;
        }
    }

    public void ColorAll()
    {
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                ColorCell(x, y, "normal");
            }
        }

        ColorCell(0, 7, "gold");
        ColorCell(7, 0, "gold");
    }

    private void ColorCross(int x, int y)
    {
        var right = x;
        var isDanger = true;
        while (right < gridWidth)
        {
            if (_obstacleMap[right, y]) isDanger = false;
            ColorCell(right, y, isDanger ? "danger" : "normal");
            
            right++;
        }
        
        var left = x - 1;
        isDanger = true;
        while (left >= 0)
        {
            if (_obstacleMap[left, y]) isDanger = false; 
            ColorCell(left, y, isDanger ? "danger" : "normal");
            left--;
        }
        
        var up = y;
        isDanger = true;
        while (up < gridHeight)
        {
            if (_obstacleMap[x, up]) isDanger = false;
            ColorCell(x, up, isDanger ? "danger" : "normal");
            up++;
        }
        
        var down = y - 1;
        isDanger = true;
        while (down >= 0)
        {
            if (_obstacleMap[x, down]) isDanger = false;
            ColorCell(x, down, isDanger ? "danger" : "normal");  
            down--;
        }
    }

    public void ColorDanger()
    {
        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                if (_crossMap[x, y])
                {
                    ColorCross(x, y);
                }

                if (_garlicMap[x, y])
                {
                    ColorCell(x, y, "danger");
                }
            }
        }
    }
    
    private void PlaceCross(int x, int y)
    {
        _crossMap[x, y] = true;
        var position = new Vector3(x * cellSize, y * cellSize, 1.1f);
        var crossObject = Instantiate(crossPrefab, position, Quaternion.identity);
        _objects.Add(crossObject);
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

            obstacle.transform.position = new Vector3(x, y, 1);
            
            obstacle.x = x;
            obstacle.y = y;
            
            ColorDanger();
        }
        else
        {
            obstacle.transform.position = new Vector3(obstacle.x, obstacle.y, 1);
        }
    }
}

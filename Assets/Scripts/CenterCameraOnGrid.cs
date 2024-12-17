using UnityEngine;

public class CenterCameraOnGrid : MonoBehaviour
{
    public GameObject grid;

    void Start()
    {
        CenterCamera();
    }

    void CenterCamera()
    {
        var gridCenter = grid.transform.position;
        var gridWidth = 8.0f;
        var gridHeight = 8.0f;

        gridCenter = new Vector3(gridCenter.x + (gridWidth - 1) / 2, gridCenter.y + (gridHeight - 1) / 2, gridCenter.z);

        Camera.main.transform.position = new Vector3(gridCenter.x, gridCenter.y, Camera.main.transform.position.z);
    }
}

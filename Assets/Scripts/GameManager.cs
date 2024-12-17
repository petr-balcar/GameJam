using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text messageText;
    public string message = "Game Over!";

    public GameObject playerPrefab;
    private GameObject _playerObject;
    private Player _player;

    public Button startDayButton;
    public Button startNightButton;
    
    public GridManager gridManager;

    private void Start()
    {
        startNightButton.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (_player is not null && _player.reachedFinish)
        {
            Time.timeScale = 0f;
            ShowMessage(message);
            startDayButton.gameObject.SetActive(false);
            startNightButton.gameObject.SetActive(false);
        }
    }

    private void ShowMessage(string msg)
    {
        messageText.text = msg;
        messageText.gameObject.SetActive(true);
    }

    private void CreatePlayer()
    {
        _playerObject = Instantiate(playerPrefab, new Vector3(0, 7, 0), Quaternion.identity);
        _player = _playerObject.GetComponent<Player>();
        _player.gridManager = gridManager;
    }

    private void RemovePlayer()
    {
        if (_player is not null) {
            Destroy(_playerObject);
            _player.gridManager = null;
            _player = null;
        }
    }
    
    public void StartDay()
    {
        CreatePlayer();
        startDayButton.gameObject.SetActive(false);
        startNightButton.gameObject.SetActive(true);
    }
    
    public void StartNight()
    {
        RemovePlayer();        
        startDayButton.gameObject.SetActive(true);
        startNightButton.gameObject.SetActive(false);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        RemovePlayer();
        
        messageText.gameObject.SetActive(false);
        startDayButton.gameObject.SetActive(true);
        startNightButton.gameObject.SetActive(false);
    }
}
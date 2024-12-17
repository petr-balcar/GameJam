using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text messageText;
    public string finishMessage = "You Won!";
    public string deathMessage = "You Died!";

    public GameObject playerPrefab;
    private GameObject _playerObject;
    private Player _player;

    public Button startDayButton;
    public Button startNightButton;
    
    public GridManager gridManager;

    public bool isDay = false;
    
    private void Start()
    {
        startNightButton.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (_player is null) return;
        
        if (_player.reachedFinish)
        {
            ShowMessage(finishMessage);
            StopGame();
        } 
        else if (_player.died)
        {
            ShowMessage(deathMessage);
            StopGame();
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
        isDay = true;
            
        CreatePlayer();
        startDayButton.gameObject.SetActive(false);
        startNightButton.gameObject.SetActive(true);
        
        gridManager.EnableShadows();
    }
    
    public void StartNight()
    {
        isDay = false;
        
        RemovePlayer();        
        startDayButton.gameObject.SetActive(true);
        startNightButton.gameObject.SetActive(false);
        
        gridManager.DisableShadows();
    }

    public void StopGame()
    {
        Time.timeScale = 0f;
        startDayButton.gameObject.SetActive(false);
        startNightButton.gameObject.SetActive(false);
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        RemovePlayer();
        gridManager.DisableShadows();
        isDay = false;
        
        messageText.gameObject.SetActive(false);
        startDayButton.gameObject.SetActive(true);
        startNightButton.gameObject.SetActive(false);
    }
}
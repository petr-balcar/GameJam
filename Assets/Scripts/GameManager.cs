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

    public SpriteRenderer dayBackground;
    public SpriteRenderer nightBackground;
    
    public SpriteRenderer nightShadowRenderer;
    
    public float darkenFactor = 0.2f;
    
    public bool isDay = false;
    
    private void Start()
    {
        StartNight();
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
        _playerObject = Instantiate(playerPrefab, new Vector3(0, 7, 1), Quaternion.identity);
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
        SetLight();
        gridManager.ColorAll();
    }
    
    public void StartNight()
    {
        isDay = false;
        
        RemovePlayer();        
        startDayButton.gameObject.SetActive(true);
        startNightButton.gameObject.SetActive(false);
        SetDark();
        gridManager.ColorDanger();
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
        messageText.gameObject.SetActive(false);
        StartNight();
    }

    private void SetDark()
    {
        nightBackground.enabled = true;
        dayBackground.enabled = false;

        nightShadowRenderer.color = new Color(0, 0, 0, 0.5f);
    }
    
    private void SetLight()
    {
        nightBackground.enabled = false;
        dayBackground.enabled = true;
        
        nightShadowRenderer.color = new Color(0, 0, 0, 0);
    }
}
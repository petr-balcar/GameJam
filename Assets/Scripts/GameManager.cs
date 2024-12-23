using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text gameOverText;
    public Text finishedText;
    
    public GameObject playerPrefab;
    private GameObject _playerObject;
    private Player _player;

    public Button startDayButton;
    public Button startNightButton;
    
    public GridManager gridManager;
    public DialogManager dialogManager;
    public SoundManager soundManager;
    
    public SpriteRenderer nightShadowRenderer;
    
    public float darkenFactor = 0.2f;
    
    public bool isDay = false;
    
    private int currentLevel = 0;
    
    private void Start()
    {
        LoadNextLevel();
    }
    
    private void Update()
    {
        if (_player is null) return;
        
        if (_player.reachedFinish)
        {
            ShowMessage(finishedText);
            StopGame();
        } 
        else if (_player.died)
        {
            ShowMessage(gameOverText);
            Destroy(_playerObject);
            _player = null;
            StopGame();
        } 
        else if (_player.disappeared)
        {
            Destroy(_playerObject);
            _player = null;
        }
    }

    private void ShowMessage(Component text)
    {
        text.gameObject.SetActive(true);
    }

    private void CreatePlayer()
    {
        _playerObject = Instantiate(playerPrefab, new Vector3(0, 7, 1.5f), Quaternion.identity);
        _player = _playerObject.GetComponent<Player>();
        _player.gridManager = gridManager;
        _player.soundManager = soundManager;
    }

    private void RemovePlayer()
    {
        if (_player is not null)
        {
            _player.disappear = true;
        }
    }

    public void StartDay()
    {
        if (_player is not null && _player.disappear) return; // prevent day start if player is still disappearing
        
        isDay = true;
            
        CreatePlayer();
        startDayButton.gameObject.SetActive(false);
        startNightButton.gameObject.SetActive(true);
        SetLight();
        gridManager.ColorAll();
    }
    
    public void StartNight()
    {
        soundManager.PlayBubble();

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
        gameOverText.gameObject.SetActive(false);
        finishedText.gameObject.SetActive(false);

        if (_player)
        {
            Destroy(_playerObject);
            _player = null;
        }
        StartNight();
    }

    private void SetDark()
    {
        nightShadowRenderer.color = new Color(0, 0, 0, darkenFactor);
    }
    
    private void SetLight()
    {
        nightShadowRenderer.color = new Color(0, 0, 0, 0);
    }

    public void LoadNextLevel()
    {
        RestartGame();
        gridManager.LoadLevel(++currentLevel);

        switch (currentLevel)
        {
            case 1:
                string[] dialogLevel1 = new string[]
                {
                    "Welcome, my dear sharp-toothed friend!",
                    "Let me tell you how this vampire thing goes…",
                    "You must AVOID direct sunlight and keep in the shadows, for your strength during the day is limited.",
                    "Being a vampire is not a walk in the park… well, in this case, it is…",
                    "Use your complete power to move your surroundings during the night and stay in their shadows during the day.",
                    "But most importantly, suck that delicious blood!",
                    "Good luck, and don't bite the dust!"
                };
                dialogManager.StartDialog(dialogLevel1);
                break;
            case 2:
                string[] dialogLevel2 = new string[]
                {
                    "Ah, it looks like the humans have grown vary of us stealing their blood…",
                    "They use garlic to scare us away.",
                    "Whatever you do, do NOT touch any garlic!"

                };
                dialogManager.StartDialog(dialogLevel2);
                break;
            case 3:
                string[] dialogLevel3 = new string[]
                {
                    "Unbelievable, they made crosses now…",
                    "How do they know all our weaknesses?",
                    "These are a bit trickier than just plain garlic.",
                    "Do not stand in a straight line across them, like the rook in chess!",
                    "Also, try to use obstacles to block them."
                };
                dialogManager.StartDialog(dialogLevel3);
                break;
            case 6:
                string[] dialogLevel6 = new string[]
                {
                    "Congratulations on beating the game!",
                    "And thank you for playing <3"
                };
                dialogManager.StartDialog(dialogLevel6);
                break;
        }
    }
}
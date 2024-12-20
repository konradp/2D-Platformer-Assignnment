using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public enum GameState
{
    MainMenu,
    InLevel
}
public class GameManager : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject coinPrefab;
    [Range(1,5)] [SerializeField] int playerMaxHitPoints;
    [Range(1, 10)] [SerializeField] int playerMaxLives;
    [Range(1f, 5f)] [SerializeField] float deathDelay;
    #endregion

    #region Variables
    //instance
    public static GameManager instance;

    //private vars
    int _playerCoins, _playerLivesLeft, _curLevelIndex, _enemiesKilled;
    int _curLevelCoinsCollected, _curLevelEnemiesKilled;
    int _maxCoinsToCollect, _maxEnemiesToKill;
    bool _gameLostFlag, _levelWonFlag;
    GameState _state;

    //refs
    PlayerController _playerController;
    GameObject _spawnedPlayer;
    LevelHelper _levelHelper;
    #endregion

    #region Game Logic
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Awake()
    {
        //create instance
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        _playerLivesLeft = playerMaxLives;
        _gameLostFlag = false;
        _levelWonFlag = false;
        _spawnedPlayer = null;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _curLevelIndex = scene.buildIndex;
        if (_curLevelIndex > 0)
        {
            _state = GameState.InLevel;
            //search for LevelHelper
            try
            {
                _levelHelper = GameObject.FindObjectOfType<LevelHelper>();
            }
            catch (Exception e)
            {
                Debug.LogError($"Can't find level helper \n {e.Message}");
                throw;
            }
            //construct level things
            SetUpHPCounter();
            SpawnPlayer();
            SetUpCounters();
            OnCoinCountChange();
            OnHitPointChange();
            OnLivesChange();
            //reset level won flag
            _levelWonFlag = false;
        }
        else
        {
            _state = GameState.MainMenu;
            //restart things
            _playerLivesLeft = playerMaxLives;
            _playerCoins = _enemiesKilled = 0;
            _gameLostFlag = false;
            _spawnedPlayer = null;
            _maxCoinsToCollect = _maxEnemiesToKill = 0;
        }
    }
    private void SetUpHPCounter()
    {
        for (int i = playerMaxHitPoints; i < _levelHelper.HpSprites.Length; i++)
        {
            _levelHelper.HpSprites[i].gameObject.SetActive(false);
        }
    }
    void SpawnPlayer()
    {
        _spawnedPlayer = GameObject.Instantiate(playerPrefab, _levelHelper.PlayerRespawnTransform.position, 
            Quaternion.identity, _levelHelper.EntitiesTransform);
        _playerController = _spawnedPlayer.GetComponent<PlayerController>();
        _playerController.OnCreateFromGameManager(playerMaxHitPoints, this);
        //update the camera target
        _levelHelper.CameraSmoothFollow.UpdateTarget(_spawnedPlayer.transform);
        //we should update hp sprites
        OnHitPointChange();
    }
    private void SetUpCounters()
    {
        _levelHelper.CountObjectsInScene();
        _curLevelEnemiesKilled = _curLevelCoinsCollected = 0;
        _maxCoinsToCollect += _levelHelper.AllCoinsInLevel; 
        _maxEnemiesToKill += _levelHelper.EnemyCount;

    }
    private void Update()
    {
        if (_gameLostFlag && Input.GetButton("Submit"))
        {
            SceneManager.LoadScene(0);
        }
        if(_levelWonFlag && Input.GetButtonUp("Submit"))
        {
            LoadNextScene();
        }
    }
    public void ChangeGameDifficulty(int hp, int lives)
    {
        playerMaxHitPoints = hp;
        playerMaxLives = _playerLivesLeft = lives;
    }
    public void OnCoinCountChange()
    {
        _levelHelper.CoinsText.text = $"X {_playerCoins}";
    }
    public void OnHitPointChange()
    {
        for (int i = 0; i < playerMaxHitPoints; i++)
        {
            if (i > _playerController.GetHP-1)
            {
                _levelHelper.HpSprites[i].GetComponent<Image>().sprite = _levelHelper.HpEmptySprite;
            }
            else
            {
                _levelHelper.HpSprites[i].GetComponent<Image>().sprite = _levelHelper.HpFullSprite;
            }
        }
    }
    public void OnLivesChange()
    {
        _levelHelper.LivesText.text = $"X {_playerLivesLeft}";
    }
    public void OnPlayerKilled()
    {
        _playerLivesLeft--;
        StartCoroutine(nameof(DestroyPlayerOverTime),deathDelay);
        OnLivesChange();
        if (_playerLivesLeft == 0)
        {
            _levelHelper.GameOverScreen.SetActive(true);
            _gameLostFlag = true;
        }
    }
    public void OnEnemyDestroyed()
    {
        _curLevelEnemiesKilled++;
        _enemiesKilled++;
    }
    IEnumerator DestroyPlayerOverTime(float delay)
    {
        _playerController.enabled = false;
        yield return new WaitForSeconds(delay);
        GameObject.Destroy(_spawnedPlayer);
        if(_playerLivesLeft>0)
            SpawnPlayer();
    }
    public void OnCoinCollected()
    {
        _curLevelCoinsCollected++;
        _playerCoins++;
        OnCoinCountChange();
    }
    public void SpawnCoin(Transform desTrans)
    {
        GameObject.Instantiate(coinPrefab, desTrans.position, Quaternion.identity, _levelHelper.PickableEntitiesTransform);
    }
    public void OnPlayerWonLevel()
    {
        _levelHelper.GameWinScreen.SetActive(true);
        ChangeEndGameText();
        _levelWonFlag = true;
    }
    private void ChangeEndGameText()
    {
        _levelHelper.CollectedCoinsText.text = $"Coins found : {_curLevelCoinsCollected}/{_levelHelper.AllCoinsInLevel}";
        _levelHelper.EnemyKilledText.text = $"Enemies killed : {_curLevelEnemiesKilled}/{_levelHelper.EnemyCount}";
        if (_levelHelper.IsFinalLevel)
        {
            _levelHelper.TotalCoinsCollectedText.text = $"Total coins collected : {_playerCoins}/{_maxCoinsToCollect}";
            _levelHelper.TotalEnemiesKilledText.text = $"Total killed enemies : {_enemiesKilled}/{_maxEnemiesToKill}";
            _levelHelper.GameCompletionPercentageText.text = $"Percentage complete: {GetCompletionPercentage():n2}%";
        }
    }
    private float GetCompletionPercentage()
    {
        //calc the percentage
        int var1 = _playerCoins + _enemiesKilled;
        int var2 = _maxCoinsToCollect + _maxEnemiesToKill;
        return ((float)var1 / (float)var2) * 100f;
    }
    void LoadNextScene()
    {
        _curLevelIndex = (_curLevelIndex + 1) % SceneManager.sceneCountInBuildSettings;
#if UNITY_WEBGL
        if (_levelHelper.IsFinalLevel)
            SceneManager.LoadScene(0);
#endif
        SceneManager.LoadScene(_curLevelIndex);
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion
}

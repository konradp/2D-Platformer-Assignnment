using System.Collections;
using System.Collections.Generic;
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

    public static GameManager instance;

    int _playerCoins, _playerLivesLeft;
    bool _gameLostFlag;
    GameState _state;

    //refs
    PlayerController _playerController;
    GameObject _spawnedPlayer;
    LevelHelper _levelHelper;
    #endregion

    #region Game Logic
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
        _spawnedPlayer = null;
    }
    //todo: method obsolete, do something different
    private void OnLevelWasLoaded(int level)
    {
        if (level > 0)
        {
            _state = GameState.InLevel;
            //search for LevelHelper
            try
            {
                _levelHelper = GameObject.FindObjectOfType<LevelHelper>();
            }
            catch (Exception e)
            {
                Debug.LogError("Can't find level helper, sayonara");
                throw;
            }
            //construct level things
            SetUpHPCounter();
            SpawnPlayer();
            OnCoinCountChange();
            OnHitPointChange();
            OnLivesChange();
        }
        else
        {
            _state = GameState.MainMenu;
            //restart things
            _playerLivesLeft = playerMaxLives;
            _playerCoins = 0;
            _gameLostFlag = false;
            _spawnedPlayer = null;
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

    private void Update()
    {
        if (_gameLostFlag && Input.GetButton("Submit"))
        {
            SceneManager.LoadScene(0);
        }
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
        _playerCoins++;
        OnCoinCountChange();
    }
    public void SpawnCoin(Transform desTrans)
    {
        GameObject.Instantiate(coinPrefab, desTrans.position, Quaternion.identity, _levelHelper.PickableEntitiesTransform);
    }
    #endregion
}

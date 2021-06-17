using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHelper : MonoBehaviour
{
    //serialize vars
    [SerializeField] bool isFinalLevel;
    [SerializeField] Transform playerRespawnTransform;
    [SerializeField] Transform entitiesTransform;
    [SerializeField] Transform pickableEntitiesTransform;
    [SerializeField] Transform[] hpSpriteObjs;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject gameWinScreen;
    [SerializeField] CameraSmoothFollow cameraSmoothFollow;
    [SerializeField] Text livesText;
    [SerializeField] Text hpText;
    [SerializeField] Text coinsText;
    [SerializeField] Text collectedCoinsText;
    [SerializeField] Text enemyKilledText;
    [SerializeField] Text totalCoinsCollectedText;
    [SerializeField] Text totalEnemiesKilledText;
    [SerializeField] Text gameCompletionPercentageText;
    [SerializeField] Sprite hpFullSprite;
    [SerializeField] Sprite hpEmptySprite;

    //private vars
    int _maxCoins, _enemyCount;

    //getters
    public Transform PlayerRespawnTransform { get => playerRespawnTransform; set => playerRespawnTransform = value; }
    public Transform EntitiesTransform { get => entitiesTransform; set => entitiesTransform = value; }
    public Transform PickableEntitiesTransform { get => pickableEntitiesTransform; set => pickableEntitiesTransform = value; }
    public Transform[] HpSprites { get => hpSpriteObjs; set => hpSpriteObjs = value; }
    public GameObject GameOverScreen { get => gameOverScreen; set => gameOverScreen = value; }
    public CameraSmoothFollow CameraSmoothFollow { get => cameraSmoothFollow; set => cameraSmoothFollow = value; }
    public Text LivesText { get => livesText; set => livesText = value; }
    public Text HpText { get => hpText; set => hpText = value; }
    public Text CoinsText { get => coinsText; set => coinsText = value; }
    public Sprite HpFullSprite { get => hpFullSprite; set => hpFullSprite = value; }
    public Sprite HpEmptySprite { get => hpEmptySprite; set => hpEmptySprite = value; }
    public Text CollectedCoinsText { get => collectedCoinsText; set => collectedCoinsText = value; }
    public Text EnemyKilledText { get => enemyKilledText; set => enemyKilledText = value; }
    public int MaxCoins { get => _maxCoins; set => _maxCoins = value; }
    public int EnemyCount { get => _enemyCount; set => _enemyCount = value; }
    public GameObject GameWinScreen { get => gameWinScreen; set => gameWinScreen = value; }
    public bool IsFinalLevel { get => isFinalLevel; set => isFinalLevel = value; }
    public Text TotalCoinsCollectedText { get => totalCoinsCollectedText; set => totalCoinsCollectedText = value; }
    public Text TotalEnemiesKilledText { get => totalEnemiesKilledText; set => totalEnemiesKilledText = value; }
    public Text GameCompletionPercentageText { get => gameCompletionPercentageText; set => gameCompletionPercentageText = value; }

    #region GameLogic

    private void Start()
    {
        CountObjectsInScene();
    }

    public void CountObjectsInScene()
    {
        var coins = FindObjectsOfType<MonoBehaviour>().OfType<ICoinAcquireable>();
        _maxCoins = coins.Count();
        var enemies = FindObjectsOfType<EnemyController>();
        _enemyCount = enemies.Count();
    }

    #endregion
}

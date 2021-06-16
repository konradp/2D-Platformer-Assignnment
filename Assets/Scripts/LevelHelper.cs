using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHelper : MonoBehaviour
{
    [SerializeField] Transform playerRespawnTransform;
    [SerializeField] Transform entitiesTransform;
    [SerializeField] Transform pickableEntitiesTransform;
    [SerializeField] Transform[] hpSpriteObjs;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] CameraSmoothFollow cameraSmoothFollow;
    [SerializeField] Text livesText;
    [SerializeField] Text hpText;
    [SerializeField] Text coinsText;
    [SerializeField] Sprite hpFullSprite;
    [SerializeField] Sprite hpEmptySprite;

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
}

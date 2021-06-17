using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameDifficulty
{
    easy,
    normal,
    hard,
    hardcore
}
public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Text difficultyNameText;
    [SerializeField] Text difficultyExplainedText;
    [SerializeField] Button mainButton;

    GameManager _gameManager;
    GameDifficulty _selectedDifficulty;
    int[] _hitPointsPerDifficulty = { 5, 3, 2, 1 };
    int[] _livesPerDifficulty = { 20, 4, 2, 1 };
    private void Start()
    {
        AcquireGameManagerInstance();
        _selectedDifficulty = GameDifficulty.easy;
        ChangeDifficultyButton();
        mainButton.Select(); //fix for gamepad users
    }
    void AcquireGameManagerInstance()
    {
        try
        {
            _gameManager = GameManager.instance;
        }
        catch (Exception e)
        {
            Debug.LogError($"GM is nowhere to be found :( \n {e.Message}");
            throw;
        }
    }
    public void ChangeDifficultyButton()
    {
        _selectedDifficulty = (GameDifficulty)(((int)_selectedDifficulty + 1) % Enum.GetNames(typeof(GameDifficulty)).Length);
        switch (_selectedDifficulty)
        {
            case GameDifficulty.easy:
                UpdateDifficulty(_selectedDifficulty.ToString(), _hitPointsPerDifficulty[0], _livesPerDifficulty[0]);
                break;
            case GameDifficulty.normal:
                UpdateDifficulty(_selectedDifficulty.ToString(), _hitPointsPerDifficulty[1], _livesPerDifficulty[1]);
                break;
            case GameDifficulty.hard:
                UpdateDifficulty(_selectedDifficulty.ToString(), _hitPointsPerDifficulty[2], _livesPerDifficulty[2]);
                break;
            case GameDifficulty.hardcore:
                UpdateDifficulty(_selectedDifficulty.ToString(), _hitPointsPerDifficulty[3], _livesPerDifficulty[3]);
                break;
        }
    }
    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }
    void UpdateDifficulty(string shortName, int hp, int lives)
    {
        difficultyNameText.text = $"Selected Difficulty : {shortName}";
        difficultyExplainedText.text = $"Current Lives : {lives} \nCurrent HP : {hp}";
        if (_gameManager != null)
        {
            _gameManager.ChangeGameDifficulty(hp, lives);
        }
        else
        {
            AcquireGameManagerInstance();
            UpdateDifficulty(shortName, hp, lives);
        }
    }

    
}
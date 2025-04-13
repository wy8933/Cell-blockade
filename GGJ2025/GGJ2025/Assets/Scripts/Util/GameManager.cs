using NUnit.Framework;
using System.Data;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerController Player;
    public GameObject gameOverMenu;
    public bool isPaused = false;
    public GameObject pauseMenu;
    public bool isGameOver;
    public bool isPowerUp;

    public TextMeshProUGUI currencyText;

    public FloatReference Currency;

    public List<GameObject> CurrentEnemyList = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            isGameOver = false;
            isPowerUp = false;
        }
        else {
            Destroy(gameObject);
        }
        currencyText.text = "Currency: " + Currency.Value.ToString();
    }

    /// <summary>
    /// toggle pause the game and shot pause menu when game is not over and it's not power up menu
    /// </summary>
    public void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            if (!isGameOver && !isPowerUp)
                pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            if (!isGameOver && !isPowerUp)
                pauseMenu.SetActive(false);
        }
    }

    // Show game over menu and pause the game
    public void GameOver()
    {
        isGameOver = true;
        Pause();
        gameOverMenu.SetActive(true);
    }

    public void ModifyCurrency(float amount) 
    {
        Currency.Value += amount;
        currencyText.text = $"Currency: {((int)Currency.Value)}";
    }



}

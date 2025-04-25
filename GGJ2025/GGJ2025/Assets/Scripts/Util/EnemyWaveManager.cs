using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct EnemySpawn
{
    public EnemyType enemyType;
    public float chance;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public int baseEnemyCount;
    public float spawnInterval;
    public List<EnemySpawn> enemyTypes;
    public bool isBossWave;
    public EnemyType bossType;
    public bool isBuffWave;
}

public class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance;

    [Header("Wave Settings")]
    public List<Wave> waves;
    public int currentWaveIndex = 0;
    public bool isWaveActive = false;

    [Header("Scaling Difficulty")]
    public float enemyIncreaseFactor = 1.2f;

    [Header("Wave Timing")]
    public float timeBetweenWaves = 5f;
    public float currenTime;
    [SerializeField] private GameObject skipWave;

    [Header("Spawner Settings")]
    public List<EnemySpawner> enemySpawners;

    private int _enemiesRemainingToSpawn;
    private int _enemiesRemainingAlive;

    public delegate void OnWaveStart(int waveIndex);
    public event OnWaveStart WaveStarted;

    public GameObject timerHolder;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI TextforTimer;
    public TextMeshProUGUI textforWave;

    private IEnumerator _waitForNextWaveRoutine;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _waitForNextWaveRoutine = WaitForNextWave();
        StartCoroutine(StartWaveCoroutine());
    }

    public void Update()
    {
        // Ill move this to HUD manager later
        if (currenTime > 0)
        {
            currenTime -= Time.deltaTime;   
        }

        if (currenTime <= 0)
        {
            skipWave.SetActive(false);
        }
        else
        {
            skipWave.SetActive(true);
        }

        TimerText.text = $"{(int)currenTime}";

    }

    /// <summary>
    /// skit to the next wave
    /// </summary>
    public void SkipWave()
    {
        if (_waitForNextWaveRoutine != null)
        {
            StopCoroutine(_waitForNextWaveRoutine);
            timerHolder.SetActive(false);
            TimerText.gameObject.SetActive(false);
            TextforTimer.gameObject.SetActive(false);
            currentWaveIndex++;
            StartNextWave();
        }
        else {
            Debug.Log("no referencce");
        }
    }



    /// <summary>
    /// Pause for seconds and start the wave
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartWaveCoroutine()
    {
        yield return new WaitForSeconds(3f);
        StartNextWave();
    }

    /// <summary>
    /// Start the new wave of enemies
    /// </summary>
    private void StartNextWave()
    {
        // Loop the enemy wave and increase it's difficulty
        if (currentWaveIndex >= waves.Count)
        {
            // Reset the wave index
            currentWaveIndex = waves.Count-1;

            // So there is at lease one number of enemy increased
            for (int i = 0; i < waves.Count; i++)
            {
                waves[i].baseEnemyCount += 1;
            }
        }

        textforWave.text = $"{currentWaveIndex + 1}";

        Wave currentWave = waves[currentWaveIndex];

        // Scale the number of enemy
        int scaledEnemyCount = Mathf.RoundToInt(currentWave.baseEnemyCount * Mathf.Pow(enemyIncreaseFactor, currentWaveIndex));
        _enemiesRemainingToSpawn = scaledEnemyCount;
        _enemiesRemainingAlive = _enemiesRemainingToSpawn;

        isWaveActive = true;

        // Start spawning enemies
        WaveStarted?.Invoke(currentWaveIndex);
        StartCoroutine(SpawnWaveEnemies(currentWave, scaledEnemyCount));
    }

    public void IncreaseEnemyCount(int amount) 
    {
        _enemiesRemainingAlive += amount;
    }

    /// <summary>
    /// Spawn the enemy in random enemy spawner
    /// </summary>
    /// <param name="wave">The current wave number</param>
    /// <param name="enemyCount">The number of enemy to spawn</param>
    /// <returns></returns>
    private IEnumerator SpawnWaveEnemies(Wave wave, int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemySpawners.Count == 0)
            {
                Debug.LogWarning("No enemy spawners assigned.");
                yield break;
            }

            // Spawn from a random spawner
            EnemySpawner selectedSpawner = enemySpawners[UnityEngine.Random.Range(0, enemySpawners.Count)];

            float randomFloat = UnityEngine.Random.Range(0.0f,1.0f);
            float currentChance = 0;

            foreach (EnemySpawn value in wave.enemyTypes) {
                currentChance += value.chance;

                if (currentChance >= randomFloat)
                {
                    selectedSpawner.SpawnEnemy(value.enemyType);
                    break; 
                }
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        isWaveActive = false;

        _waitForNextWaveRoutine = WaitForNextWave();
        StartCoroutine(_waitForNextWaveRoutine);
    }

    /// <summary>
    /// Check if it's time for the next wave of enemy
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForNextWave()
    {
        while (_enemiesRemainingAlive > 0)
        {
            yield return null;
        }

        if (waves[currentWaveIndex].isBuffWave)
        {
            yield return new WaitForSeconds(1f);
            ShopManager.Instance.ShowShop();
        }

        timerHolder.SetActive(true);
        TimerText.gameObject.SetActive(true);
        TextforTimer.gameObject.SetActive(true);
        currenTime = timeBetweenWaves;
        yield return new WaitForSeconds(timeBetweenWaves);
        TimerText.gameObject.SetActive(false);
        TextforTimer.gameObject.SetActive(false);
        timerHolder.SetActive(false);

        currentWaveIndex++;
        StartNextWave();
    }

    /// <summary>
    /// Decrease the remaining alive enemy count
    /// </summary>
    public void EnemyDefeated()
    {
        _enemiesRemainingAlive--;
    }
}

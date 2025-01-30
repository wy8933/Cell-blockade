using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public string waveName;
    public int baseEnemyCount;
    public float spawnInterval;
    public EnemyType[] enemyTypes;
    public bool isBossWave;
    public EnemyType bossType;
    public bool isBuffWave;
}

public class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance;
    public PowerUpManager powerUpManager;

    [Header("Wave Settings")]
    public List<Wave> waves;
    public int currentWaveIndex = 0;
    private bool _isWaveActive = false;

    [Header("Scaling Difficulty")]
    public float enemyIncreaseFactor = 1.2f;

    [Header("Wave Timing")]
    public float timeBetweenWaves = 5f;

    [Header("Spawner Settings")]
    public List<EnemySpawner> enemySpawners;

    private int _enemiesRemainingToSpawn;
    private int _enemiesRemainingAlive;

    public delegate void OnWaveStart(int waveIndex);
    public event OnWaveStart WaveStarted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartWaveCoroutine());
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
            currentWaveIndex = 0;

            // So there is at lease one number of enemy increased
            for (int i = 0; i < waves.Count; i++)
            {
                waves[i].baseEnemyCount += 1;
            }
        }

        Wave currentWave = waves[currentWaveIndex];

        // Scale the number of enemy
        int scaledEnemyCount = Mathf.RoundToInt(currentWave.baseEnemyCount * Mathf.Pow(enemyIncreaseFactor, currentWaveIndex));
        _enemiesRemainingToSpawn = scaledEnemyCount;
        _enemiesRemainingAlive = _enemiesRemainingToSpawn;

        _isWaveActive = true;

        // Start spawning enemies
        WaveStarted?.Invoke(currentWaveIndex);
        StartCoroutine(SpawnWaveEnemies(currentWave, scaledEnemyCount));
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
            EnemySpawner selectedSpawner = enemySpawners[Random.Range(0, enemySpawners.Count)];
            selectedSpawner.SpawnEnemy();

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        _isWaveActive = false;
        StartCoroutine(WaitForNextWave());
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
        
        if (waves[currentWaveIndex].isBuffWave) {
            powerUpManager.ShowPowerUpSelection();
        }

        yield return new WaitForSeconds(timeBetweenWaves);
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

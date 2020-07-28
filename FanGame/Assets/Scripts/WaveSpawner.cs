using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState
    {
        Spawning,
        counting
    };
    [System.Serializable]
    public class Wave
    {
        public Transform[] enemy;
        public float rate;
    }
    public Wave[] waves;
    public Transform[] spawnPoints;
    private int nextWave = 0;
    public float timeBetweenWaves = 5f;
    private float waveCountdown = 0f;
    public int enemyCount;
    public int killCount;
    private GameObject[] deadEnemies;
    private int spawnCount;
    ObjectPooler objectPooler;
    private float searchCountdown = 1f;
    private int wavesRate = 5;
    private bool waveOver;

    private void Start()
    {
        waveOver = false;
        objectPooler = ObjectPooler.Instance;
        waveCountdown = timeBetweenWaves;
        enemyCount = 0;
        killCount = 0;
        StartCoroutine(SpawnWave(waves[nextWave],wavesRate));
    }
    public void GetSpawnNumber()
    {
        killCount++;
            if (enemyCount < 18)
            {
                StartCoroutine(SpawnWave(waves[nextWave], 2));
            }
            else
            {
                    StartCoroutine(WaitCoroutine());
                
            }
        }

    
    IEnumerator SpawnWave(Wave _wave,int spawnNumber)
    { 
        for(int i = 0; i < spawnNumber; i++)
        {
            int random = UnityEngine.Random.Range(0, _wave.enemy.Length);
            SpawnEnemy(_wave.enemy[random]);
            yield return new WaitForSeconds(1f / _wave.rate);
        }
        yield break;
    }


    void SpawnEnemy(Transform _enemy)
    {
        Transform _sp = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        Instantiate(_enemy, _sp.position, _sp.rotation); //instantiates object on the scene
        enemyCount++;
    }

    public IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(10f);
        if (!EnemyIsAlive())
        {
            wavesRate++;
            enemyCount = 0;
            StartCoroutine(SpawnWave(waves[nextWave],wavesRate));
        }
        else
        {
            yield break;
        }
    }

    bool EnemyIsAlive()
    {
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        return true;

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHill : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Ant antPrefab;
    [SerializeField]
    private float offscreenDistance;

    
    [SerializeField]
    private float waveTimeMin, waveTimeMax;
    private float nextWaveTime;
    private float newNextWaveTime => Time.time + Random.Range(waveTimeMin, waveTimeMax);
    [SerializeField]
    private float waveDurationMin, waveDurationMax;
    private float stopTime => Time.time + Random.Range(waveDurationMin, waveDurationMax);
    [SerializeField]
    private float waveRateOverTime;
    private float waveRate => waveRateOverTime * Time.time;
    
    private List<Vector2> spawnPoints;

    private GameObject enemyAntParent;

    private void Start() {
        spawnPoints = new();
        spawnPoints.Add(RandomPointOutsideScreen());

        enemyAntParent = new GameObject("EnemyAntParent");
        nextWaveTime = newNextWaveTime;
    }

    private void Update() {
        if (Time.time > nextWaveTime) {
            spawnPoints.Add(RandomPointOutsideScreen());
            StartCoroutine(Wave(stopTime, waveRate));
            nextWaveTime = newNextWaveTime;
        }
    }
    
    private IEnumerator Wave(float stopTime, float rate) {
        WaitForSeconds secondsBetweenSpawn = new WaitForSeconds(1 / rate);
        while (Time.time < stopTime) {
            SpawnEnemyAnt();
            yield return secondsBetweenSpawn;
        }
    }

    public void SpawnEnemyAnt() {
        int randomIndex = Random.Range(0, spawnPoints.Count);
        Vector2 randomPoint = spawnPoints[randomIndex];

        Ant enemyAnt = Instantiate(antPrefab, randomPoint, Quaternion.identity);
        enemyAnt.transform.parent = enemyAntParent.transform;
        enemyAnt.SetBehaviour(BehaviourMode.Attacking);
    }

    private void OnDrawGizmos() {
        if (spawnPoints == null) return;

        Gizmos.color = Color.red;
        foreach (Vector2 spawnPoint in spawnPoints) {
            Gizmos.DrawWireSphere(spawnPoint, 0.5f);
        }
    }

    private Vector2 RandomPointOutsideScreen() {
        int xSign = Random.Range(0, 2) * 2 - 1; // random -1 or 1, but never 0
        int ySign = Random.Range(0, 2) * 2 - 1;


        float randomX = 0;
        float randomY = 0;
        float xEdge = mainCamera.orthographicSize * mainCamera.aspect;
        float yEdge = mainCamera.orthographicSize;
        switch (Random.Range(0, 2)) {
            case 0:
                randomX = xEdge + offscreenDistance;
                randomY = Random.Range(0, yEdge + offscreenDistance);
                break;
            case 1:
                randomX = Random.Range(0, xEdge + offscreenDistance);
                randomY = yEdge + offscreenDistance;
                break;
        }
        
        if (randomX < mainCamera.orthographicSize * mainCamera.aspect) {
            randomY = mainCamera.orthographicSize + offscreenDistance;
        }

        randomX *= xSign;
        randomY *= ySign;

        return new Vector2(randomX, randomY);
    }

}

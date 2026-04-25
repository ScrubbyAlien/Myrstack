using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField]
    private World world;
    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private GameObject gameOverScreen;

    private float collectedFood;
    private float aliveAnts;

    private Coroutine checkIfDeadCoroutine;

    private float gameStartTime;
    
    private void Start() {
        if (world.hill) world.hill.FoodCollected += FoodCollected;
        else world.HillRegistered += (hill) => hill.FoodCollected += FoodCollected;
        world.AntsChanged += OnAntsChanged;

        gameStartTime = Time.time;
    }

    private void FoodCollected(float collected) {
        collectedFood = collected;
        aliveAnts = world.allNonEnemyAnts.Length;
        Check();
    }
    
    private void OnAntsChanged() {
        aliveAnts = world.allNonEnemyAnts.Length;
        Check();
    }

    private void Check() {
        if (checkIfDeadCoroutine != null) StopCoroutine(checkIfDeadCoroutine);
        checkIfDeadCoroutine = StartCoroutine(CheckIfDead());
    }

    private IEnumerator CheckIfDead() {
        yield return new WaitForSeconds(3f);
        if (collectedFood < 5f && aliveAnts == 0) {
            GameOver();    
        }
    }

    public void GameOver() {
        TimeSpan timeSinceStart = TimeSpan.FromSeconds(Time.time - gameStartTime);
        float minutes = timeSinceStart.Minutes;
        float seconds = timeSinceStart.Seconds;

        textField.text = $"Your final time was:\n{minutes:0} min {seconds:0} sec";
        gameOverScreen.SetActive(true);
    }
}

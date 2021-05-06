using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpeedBoost : MonoBehaviour
{
    public GameObject speedboostObj;
    public GameObject snakeObj;
    private Vector2Int speedBoostGridPosition;
    private int minWidth = -20;
    private int maxWidth = 20;
    private int minHeight = -15;
    private int maxHeight = 15;
    private float seconds;
    public float snakeSpeed;
    public float prevSnakeSpeed;
    AudioSource speedboostSound;

    public void StartSpeedboostSpawner()
    {
        /*Koska Snake.cs speedboost objektin sijanti päivitetään joka frame:illa, astetaan speedboost  objektin sijanti pelikentän ulkopuolelle, uuden speedboostin spawnauksen ajaksi. 
       Muutoin speedboost objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
        speedBoostGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
        StartCoroutine(SpeedboostSpawner()); // Aloitetaan speedboostin spawnaus
    }
    // Speedboostin tehoste
    public void StartSpeedBoost()
    {
        StartCoroutine(SpeedBooster(0.1f)); // Asetetaan käärmeen nopeus
    }

    // Speedboostin spawnaus
    IEnumerator SpeedboostSpawner()
    {
        seconds = Random.Range(15f, 30f); // Randomoidaan aikaväli jolloin speedboost spawnaa
        yield return new WaitForSeconds(seconds);
        do
        {
            speedBoostGridPosition = new Vector2Int(Random.Range(minWidth + 1, maxWidth - 1), Random.Range(minHeight + 1, maxHeight - 1));
        }
        while (snakeObj.GetComponent<Snake>().GetFullSnakeGridPositionList().IndexOf(speedBoostGridPosition) != -1);
        speedboostObj = new GameObject("SpeedBoost", typeof(SpriteRenderer));
        speedboostObj.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.speedBoostSprite;
        speedboostObj.transform.position = new Vector3(speedBoostGridPosition.x, speedBoostGridPosition.y);
        StartCoroutine(SpawnTimeForSpeedBoost());   // Aloitetaan ajastin, jolla  määritellään kuinka kauan speedboost objekti näkyy ruudulla
    }
    // Speedboostin spawnaus ajastin
    IEnumerator SpawnTimeForSpeedBoost()
    {
        StopCoroutine(SpeedboostSpawner()); // Pysäytetään speedboostin spawnaus
        yield return new WaitForSeconds(10f); // Speedboost objekti näkyy ruudulla 10 sekuntia
        if (speedboostObj != null) // Jos speedboost objekti on edelleen aktiivinen se tuhotaan
        {
            speedboostObj.SetActive(false);
            /*Koska Snake.cs speedboost objektin sijanti päivitetään joka frame:illa, astetaan speedboost  objektin sijanti pelikentän ulkopuolelle, uuden speedboostin spawnauksen ajaksi. 
             Muutoin speedboost objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
            speedBoostGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
            Object.Destroy(speedboostObj);
        }
        StartCoroutine(SpeedboostSpawner()); // Aloitetaan uusi spawnaus
    }
    // Speedboostin nopeuden ja ajan määritys
    IEnumerator SpeedBooster(float speed)
    {
        prevSnakeSpeed = snakeObj.GetComponent<Snake>().gridMoveTimerMax; // Tallennetaan alkuperäinen nopeus
        snakeSpeed = snakeObj.GetComponent<Snake>().gridMoveTimerMax = speed;
        yield return new WaitForSeconds(10f); // Kauan tehoste on voimassa
        snakeSpeed = prevSnakeSpeed;
        prevSnakeSpeed = snakeObj.GetComponent<Snake>().gridMoveTimerMax = 0.2f; // Palautetaan alkuperäiseen nopeuteen
    }
    public bool TrySnakeEatSpeedBoost(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == speedBoostGridPosition)
        {
            /*Koska Snake.cs speedboost objektin sijanti päivitetään joka frame:illa, astetaan speedboost  objektin sijanti pelikentän ulkopuolelle, uuden speedboostin spawnauksen ajaksi. 
            Muutoin speedboost objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
            speedBoostGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
            StopCoroutine(SpawnTimeForSpeedBoost()); // Pysäytetään speedboostin spawnaus ajastin
            StopCoroutine(SpeedboostSpawner()); // Pysäytetään speedboostin spawnaus
            speedboostSound = GetComponent<AudioSource>(); // Speedboostin syömis ääni
            speedboostSound.Play();
            Score.SpeedboostScore();
            StartCoroutine(SpeedBooster(0.1f)); // Aloiteaan speedboostin tehoste
            if (speedboostObj != null) // Tuhotaan speedboost objekti
            {
                speedboostObj.SetActive(false);
                Object.Destroy(speedboostObj);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}

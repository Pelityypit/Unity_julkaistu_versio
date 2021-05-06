using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEscapeDeath : MonoBehaviour
{
    public GameObject escapeDeathObj;
    public GameObject snakeObj;
    private Vector2Int escapeDeathGridPosition;
    private int minWidth = -20;
    private int maxWidth = 20;
    private int minHeight = -15;
    private int maxHeight = 15;
    private float seconds;
    public Snake.State isAlive;
    public bool isEscapeDeathActive;
    AudioSource heartbeatFast;
    public AudioSource heartbeatSlow;

    public void StartEscapeDeathSpawner()
    {
        /*Koska Snake.cs escapeDeath objektin sijanti päivitetään joka frame:illa, astetaan escapeDeath objektin sijanti pelikentän ulkopuolelle, uuden escapeDeath spawnauksen ajaksi. 
        Muutoin escapeDeath objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
        escapeDeathGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
        StartCoroutine(EscapeDeathSpawner()); // Aloitetaan escapeDeath spawnaus
    }
    public void StartEscapeDeathBoost()
    {
        StartCoroutine(EscapeDeathTime()); // Aloiteaan escapeDeath boostin ajastin
    }
    IEnumerator EscapeDeathSpawner()
    {
        seconds = Random.Range(25f, 50f); // Randomoidaan aikaväli, jolloin escapeDeath spawnaa
        yield return new WaitForSeconds(seconds);
        do
        {
            escapeDeathGridPosition = new Vector2Int(Random.Range(minWidth + 1, maxWidth - 1), Random.Range(minHeight + 1, maxHeight - 1));
        }
        while (snakeObj.GetComponent<Snake>().GetFullSnakeGridPositionList().IndexOf(escapeDeathGridPosition) != -1);
        escapeDeathObj = new GameObject("EscapeDeath", typeof(SpriteRenderer));
        escapeDeathObj.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.escapeDeathSprite;
        escapeDeathObj.transform.position = new Vector3(escapeDeathGridPosition.x, escapeDeathGridPosition.y);
        StartCoroutine(SpawnTimeForEscapeDeath());  // Aloitetaan ajastin, jolla  määritellään kuinka kauan escapeDeath objekti näkyy ruudulla
    }
    IEnumerator SpawnTimeForEscapeDeath()
    {
        StopCoroutine(EscapeDeathSpawner()); // Pysätetään escapeDeath spawnaus
        yield return new WaitForSeconds(10f); // EscapeDeath objekti näkyy ruudulla 10 sekuntia
        if (escapeDeathObj != null) // Jos escapeDeath objekti on edelleen aktiivinen se tuhotaan
        {
            escapeDeathObj.SetActive(false);
            /*Koska Snake.cs escapeDeath objektin sijanti päivitetään joka frame:illa, astetaan escapeDeath objektin sijanti pelikentän ulkopuolelle, uuden escapeDeath spawnauksen ajaksi. 
            Muutoin escapeDeath objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
            escapeDeathGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
            Object.Destroy(escapeDeathObj);
        }

        StartCoroutine(EscapeDeathSpawner()); // Aloiteaan uuden escapeDeathin spawnaus
    }
    // EscapeDeath objektin boosti
    IEnumerator EscapeDeathTime()
    {
        isEscapeDeathActive = true; // Niin kauan kuin boosti on aktiivinen, käärme pysyy elävänä
        isAlive = Snake.State.Alive;
        yield return new WaitForSeconds(10f); // 10 sekunnin jälkeen käärme voi taas kuolla oman kehonsa törmäyksessä
        isEscapeDeathActive = false;
    }
    // EscapeDeath boostin äänieffekti
    IEnumerator EscapeDeathHeartBeat()
    {
        heartbeatSlow.Play();
        yield return new WaitForSeconds(7f);
        heartbeatSlow.Stop();
        heartbeatFast = GetComponent<AudioSource>();
        heartbeatFast.Play();
        yield return new WaitForSeconds(3f);
        heartbeatFast.Stop();
    }

    public bool TrySnakeEatEscapeDeath(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == escapeDeathGridPosition)
        {
            /*Koska Snake.cs escapeDeath objektin sijanti päivitetään joka frame:illa, astetaan escapeDeath objektin sijanti pelikentän ulkopuolelle, uuden escapeDeath spawnauksen ajaksi. 
            Muutoin escapeDeath objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
            escapeDeathGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
            StopCoroutine(SpawnTimeForEscapeDeath()); // Pysäytetään escapeDeath spawnaus ajastin
            StopCoroutine(EscapeDeathSpawner()); // Pysäytetään spawnaus
            SoundManager.PlaySound(SoundManager.Sound.EscapeDeath);
            Score.EscapeDeathScore();
            StartEscapeDeathBoost(); // Aloitetaan escapeDeath boosti
            StartCoroutine(EscapeDeathHeartBeat()); // Aloitetaan äänieffekti
            if (escapeDeathObj != null) // EscapeDeath objekti tuohotaan
            {
                escapeDeathObj.SetActive(false);
                Object.Destroy(escapeDeathObj);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}

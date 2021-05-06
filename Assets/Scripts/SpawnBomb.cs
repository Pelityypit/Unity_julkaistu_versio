using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBomb : MonoBehaviour
{
    public GameObject bombObj;
    public GameObject snakeObj;
    private Vector2Int bombGridPosition;
    private int minWidth = -20;
    private int maxWidth = 20;
    private int minHeight = -15;
    private int maxHeight = 15;
    private float seconds;
    bool didSnakeEat;
    AudioSource bombTicking;
    public AudioSource explosion;
    public Text timerText;
    public float timer;

    public void StartBombSpawner()
    {
        /*Koska Snake.cs pommi objektin sijanti päivitetään joka frame:illa, astetaan pommi objektin sijanti pelikentän ulkopuolelle, uuden pommin spawnauksen ajaksi. 
        Muutoin pommi objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
        bombGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
        StartCoroutine(BombSpawner()); // Aloitetaan pommin spawnaus
    }
    // Pommin spawnaus
    IEnumerator BombSpawner()
    {
        seconds = Random.Range(60f, 90f); // Randomoidaan aikaväli, jolloin pommi spawnaa
        yield return new WaitForSeconds(seconds);
        do
        {
            bombGridPosition = new Vector2Int(Random.Range(minWidth + 1, maxWidth - 1), Random.Range(minHeight + 1, maxHeight - 1));
        }
        while (snakeObj.GetComponent<Snake>().GetFullSnakeGridPositionList().IndexOf(bombGridPosition) != -1);
        bombObj = new GameObject("Bomb", typeof(SpriteRenderer));
        bombObj.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.bombSprite;
        bombObj.transform.position = new Vector3(bombGridPosition.x, bombGridPosition.y);
        StartCoroutine(SpawnTimeForBomb()); // Aloitetaan ajastin, jolla  määritellään kuinka kauan pommi objekti näkyy ruudulla
    }
    // Pommin spawnaus ajastin
    public IEnumerator SpawnTimeForBomb()
    {
        timer = 10; // Ajastin on 10 sekunttia
        timerText.GetComponent<Text>().enabled = true; // Texti komponetti, jolla ajastin näkyy ruudulla
        didSnakeEat = snakeObj.GetComponent<Snake>().snakeAteBomb;
        if (didSnakeEat == false)
        {
            // Aloitetaan ajastin
            while (timer >= 0)
            {
                bombTicking = GetComponent<AudioSource>(); // Pommin tikitys ääni
                bombTicking.Play();
                timerText.text = "BOMB DETONATES IN\n" + timer.ToString(); // Ajastimen teksti
                yield return new WaitForSeconds(1f); // Sekunnit näkyvät ruudulla sekunnin ajan
                timer--; // Ajastin aloittaa 10 ja laskee alaspäin 0
                bombTicking.Stop();
                // Jos pommia ei ole syöty kun ajastin on 0
                if (bombObj != null && timer == 0)
                {
                    explosion.Play(); // Räjähdys ääni
                    bombObj.SetActive(false); // Astetaan pommi objecti ei aktiiviseksi
                    /*Koska Snake.cs pommi objektin sijanti päivitetään joka frame:illa, astetaan pommi objektin sijanti pelikentän ulkopuolelle, uuden pommin spawnauksen ajaksi. 
                    Muutoin pommi objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
                    bombGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
                    timerText.GetComponent<Text>().enabled = false; // Teksti katoaa ruudulta
                    Object.Destroy(bombObj); // Tuhotaan pommi objekti
                    Score.BombExplodedScore(); // Puolittaa pisteet
                }
            }
            // Aloitetaan uuden pommin spawnaus
            StartCoroutine(BombSpawner());
        }
    }
    public bool TrySnakeEatBomb(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == bombGridPosition) // Jos käärmeen pään sijainti on sama kuin pommin sijainti
        {
            /*Koska Snake.cs pommi objektin sijanti päivitetään joka frame:illa, astetaan pommi objektin sijanti pelikentän ulkopuolelle, uuden pommin spawnauksen ajaksi. 
            Muutoin pommi objekti pysyy ruudulla näkymättömänä ja laukaisee syödessä äänet ja muut ajastimet */
            bombGridPosition = new Vector2Int(Random.Range(100, 100), Random.Range(minHeight + 0, maxHeight - 0));
            timer = 0f; // Astetaan ajastin nollaan
            timerText.GetComponent<Text>().enabled = false; // Teksti katoaa ruudulta
            SoundManager.PlaySound(SoundManager.Sound.Bomb); // Pommin syömis ääni
            Score.BombScore();
            StopCoroutine(SpawnTimeForBomb()); // Pysäytetään pommin spawnaus ajastin
            // Jos pommi objekti on aktiivinen se tuhotaan
            if (bombObj != null)
            {
                bombObj.SetActive(false);
                Object.Destroy(bombObj);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
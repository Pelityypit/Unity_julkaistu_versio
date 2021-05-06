using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using CodeMonkey;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
     private Text scoreText;

    private void Awake() {
        // Text-osat GameScene canvas
        // näyttää peliruudulla pistemäärän
        scoreText = transform.Find("ScoreText").GetComponent<Text>(); 
        Score.OnHighscoreChanged += Score_OnHighscoreChanged;
        UpdateHighscore();
    }
    private void Score_OnHighscoreChanged(object sender, System.EventArgs e) {
       // kun piste-ennätys muuttuu palautetaan uusi piste-ennätys
       UpdateHighscore();
    }
    private void Update() {
        // päivittää pistemäärän lisääntymisen peliruudulle
        scoreText.text = Score.GetScore().ToString(); 
    } 
    private void UpdateHighscore() {
        // päivitetään peliruudulla näkyvä piste-ennätys
        int highscore = Score.GetHighscore();
        transform.Find("HighscoreText").GetComponent<Text>().text = "HIGHSCORE\n" + highscore.ToString(); 
    }
}

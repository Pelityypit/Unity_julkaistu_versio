using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Animator transition; // animaatio ruutujen siirtymiselle
    public float transitionTime = 4f; // ruutujen siirtymis aika

    public void LoadNextLevel()
    {
        // Ruudun siirtymät
        StartCoroutine(LoadScenesWithTransitions(SceneManager.GetActiveScene().buildIndex + 1));
    }
    IEnumerator LoadScenesWithTransitions(int levelIndex)
    {
        //Kun painaa start
        transition.SetTrigger("Start");
        //Asetetaan loading
        yield return new WaitForSeconds(transitionTime);
        //Ladataan scene
        SceneManager.LoadScene(levelIndex);
    }
    public void LoadGame()
    {
        // Kun painaa play -nappia, siirrytään päävalikosta "LOAD" -ruudulle
        LoadNextLevel();
        SceneManager.LoadScene("LOAD");
    }
    public void HowToPlay()
    {
        // Kun painaa How to play-nappia, siirrytään päävalikosta "How to play?" -ruudulle
        LoadNextLevel();
        SceneManager.LoadScene("HowToPlay");
    }
    //Siirrytään takaisin päävalikkoon
    public void BackToMainMenu()
    {
        LoadNextLevel();
        SceneManager.LoadScene("MainMenu");
    }
    //Lopetetaan peli
    public void QuitGame()
    {
        // Peliä ei voi lopettaa ennen kuin peli julkaistaan
        // Joten toistaiseksi kun painaa "EXIT" consoliin ilmestyy teksti "Quit!"
        Application.Quit();
        Debug.Log("Quit!");
    }
}

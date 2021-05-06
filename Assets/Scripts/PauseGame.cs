
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class PauseGame : MonoBehaviour
{
    private static PauseGame instance;

    //Kun peli alkaa
    private void Awake()
    {
        instance = this;
        //Haetaan resumeBtn -nappi
        transform.Find("resumeBtn").GetComponent<Button_UI>().ClickFunc = () => {

            GameHandler.GamePaused(false);

        };
        //Haetaan extiGame -nappi 
        transform.Find("exitBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            
            //Jatketaan peliä, jotta scenet voivat latautua
            Time.timeScale = 1;
            //gameExit lataa "MainMenu" -ruudun
            Loader.Load(Loader.Scene.MainMenu);
        };
        //Kutsutaan pelin alkaessa Hide() -funktiota
        Hide();
    }
    //Tuodaan PauseWindow:n nappulat esille
    private void Show()
    {
        gameObject.SetActive(true);
    }
    //Piilotetaan PauseWindow:n nappulat
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    //Staattinen show()
    public static void ShowStatic()
    {
        instance.Show();
    }
    //Staattinen hide()
    public static void HideStatic()
    {
        instance.Hide();
    }
}

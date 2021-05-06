
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour
{
    private static GameOverWindow instance;

    //Kun peli alkaa
    private void Awake()
    {
        instance = this;
        //Haetaan retryBtn -nappi
        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            //retryBtn lataa uudestaan "GameScene" -ruudun
            Loader.Load(Loader.Scene.GameScene);
        };
        //Haetaan extiGame -nappi 
        transform.Find("exitBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {
            //gameExit lataa "MainMenu" -ruudun
            Loader.Load(Loader.Scene.MainMenu);
        };
        //Kutsutaan pelin alkaessa Hide() -funktiota
        Hide();
    }
    //Tuodaan retryBtn esille
    private void Show()
    {
        gameObject.SetActive(true);
    }
    //Piilotetaan retryBtn
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    //Staattinen show()
    public static void ShowStatic()
    {
        instance.Show();
    }
}

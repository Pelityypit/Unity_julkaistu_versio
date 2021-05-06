using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        //Scenes
        GameScene,
        LOADING,
        MainMenu,
    }
    private static Action loaderCallbackAction;
    //Lataa scenen
    public static void Load(Scene scene)
    {
        loaderCallbackAction = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };
        //Lataus scene
        SceneManager.LoadScene(Scene.LOADING.ToString());
    }
    public static void LoaderCallback()
    {
        //Jos scene ei ole ladattu
        if (loaderCallbackAction != null)
        {   //Lataa scene
            loaderCallbackAction();
            loaderCallbackAction = null;
        }
    }
}

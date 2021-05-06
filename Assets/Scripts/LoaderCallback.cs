using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    //Ensimmäinen update
    private bool firstUpdate = true;
    private void Update()
    {
        //Jos firstUpdate on true
        if (firstUpdate)
        {
            //Muutetaan firstUpdate == false
            firstUpdate = false;
            //Ladataan loadercallback uudestaan
            Loader.LoaderCallback();
        }
    }
}

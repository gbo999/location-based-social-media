using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platforms : MonoBehaviour
{

    public void LoadAR()
    {

        SceneManager.LoadScene("ARScene");

    }

    public void LoadMaps()
    {

        SceneManager.LoadScene("UIBubblePopup");

    }



    public void LoadAnalize()
    {

        SceneManager.LoadScene("analize-final");

    }







}

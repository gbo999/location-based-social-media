using DanielLochner.Assets.SimpleSideMenu;
using SocialApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientViewScript : MonoBehaviour
{
    private void Start()
    {
        Screen.orientation = ScreenOrientation.Landscape;
    }
}
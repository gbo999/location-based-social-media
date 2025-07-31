using SocialApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Yamanas.Scripts.MapLoader;
using Yamanas.Scripts.MapLoader.AR;
using static OnlineMapsGoogleGeocoding;

public class SaveToAR : MonoBehaviour
{
    
    public double longToSave;
    public double latToSave;
    

    
    public void save()
    {
        longToSave = GPS.longi;
        latToSave = GPS.lati;

        PostProcessController.Instance.ARPopupSystem.ShowPopup(ARPopupType.ChooseActivity);
        
        publish();
    }

    public void publish()
    {
        //publishPanel.SetActive(false);

        ReverseGeocodingParams requestParams = new ReverseGeocodingParams(longToSave, latToSave);

        requestParams.key = MyCity.Config.APIKeys.GoogleMapsAPIKey;
        requestParams.language = "en";


        OnlineMapsGoogleGeocoding request = OnlineMapsGoogleGeocoding.Find(requestParams);


        request.Send();
        request.OnComplete += OnRequestComplete;
    }


    private void OnRequestComplete(string s)
    {
        XmlDocument xDoc = new XmlDocument();

        xDoc.LoadXml(s);

        Debug.Log("success");

        XmlNodeList name = xDoc.GetElementsByTagName("formatted_address");

        Debug.Log(name[1].InnerText);

        string addressToSave = "in " + name[1].InnerText;
        PostProcessController.Instance.Address = addressToSave;
    }
}
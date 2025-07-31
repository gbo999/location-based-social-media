using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopulateDropDown : MonoBehaviour
{
    // Start is called before the first frame update

    public string apiKey;

    public TMP_InputFieldSearchable text;

    public delegate void OnNoValueChange();
    public event OnNoValueChange onDataAdded;

   // private string[] myOwnDataArray = { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua", "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bhutan", "Bolivia", "Bosnia", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cote dIvoire", "Cabo Verde", "Cambodia", "Cameroon", "Canada", "Central African", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czechia", "Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini", "Ethiopia", "Fiji", "Finland", "France", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guinea", "Guyana", "Haiti", "Holy See", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel State", "Italy", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico", "Micronesia", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "North Macedonia", "Norway", "Oman", "Pakistan", "Palau", "Palestine ", "Panama", "New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russia", "Rwanda", "Saint Kitts", "Saint Lucia", "Grenadines", "Samoa", "San Marino", "Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Korea", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga", "Trinidad", "Tunisia", "Turkey", "Turkmenistan", "Tuvalu", "Uganda", "Ukraine", "UAE", "UK", "USA", "Uruguay", "Uzbekistan", "Vanuatu", "Venezuela", "Vietnam", "Yemen", "Zambia", "Zimbabwe" };

    [HideInInspector]
    public List<string> myOwnDataList = new List<string>(); //how often does this get refreshed

    private void Start()
    {
    }

    ///**** USE THIS TO ADD DATA DURING PLAYMODE*****
    ///You should Save the DataList either into a JSON or PlayerPrefs if you want them to be persistant
    ///And then simply recall them on playmode from JSON

    public void AddMoreData(string dataInput) //Use this to input further data in playMode: note, this will not be persistant,
    {                                         //...if you want it to be persistant, then Save "myOwnDataList" in player prefs or somthing         
        myOwnDataList.Add(dataInput);
        onDataAdded();
    }

    public void PlaceFind( )
    {

        OnlineMapsGooglePlacesAutocomplete.Find(
               text.text,
                apiKey
                ).OnComplete += OnComplete;
    }

    public void OnComplete(string obj)
    {
        OnlineMapsGooglePlacesAutocompleteResult[] results = OnlineMapsGooglePlacesAutocomplete.GetResults(obj);

        // If there is no result
        if (results == null)
        {
            Debug.Log("Error");
            Debug.Log(obj);
            return;
        }

        // Log description of each result.
        foreach (OnlineMapsGooglePlacesAutocompleteResult result in results)
        {
            Debug.Log(result.description);

            AddMoreData(result.description);
        }


    }

    [ContextMenu("AddTesting")]
    public void AddTesting(string input)
    {
        if (Application.isPlaying)
        {
            AddMoreData(input);
        }
    }

}

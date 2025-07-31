using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PopulateSearchingData : MonoBehaviour
{
    public delegate void OnNoValueChange();
    public event OnNoValueChange onDataAdded;

    public TMP_InputFieldSearchable searchInput;

    private SearchIntegrationWithTMP tmp;


    public string apiKey;



    //private string[] myOwnDataArray = { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua", "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bhutan", "Bolivia", "Bosnia", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cote dIvoire", "Cabo Verde", "Cambodia", "Cameroon", "Canada", "Central African", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czechia", "Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini", "Ethiopia", "Fiji", "Finland", "France", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guinea", "Guyana", "Haiti", "Holy See", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel State", "Italy", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico", "Micronesia", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "North Macedonia", "Norway", "Oman", "Pakistan", "Palau", "Palestine ", "Panama", "New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russia", "Rwanda", "Saint Kitts", "Saint Lucia", "Grenadines", "Samoa", "San Marino", "Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Korea", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga", "Trinidad", "Tunisia", "Turkey", "Turkmenistan", "Tuvalu", "Uganda", "Ukraine", "UAE", "UK", "USA", "Uruguay", "Uzbekistan", "Vanuatu", "Venezuela", "Vietnam", "Yemen", "Zambia", "Zimbabwe" };

    private string[] myOwnDataArray = new string[0];

    [HideInInspector] public List<string> myOwnDataList = new List<string>(); //how often does this get refreshed

    private void Awake()
    {
        foreach (string item in myOwnDataArray)
        {
            myOwnDataList.Add(item);
        }
    }


    //**** USE THIS TO ADD DATA DURING PLAYMODE*****
    //You should Save the DataList either into a JSON or PlayerPrefs if you want them to be persistant
    //And then simply recall them on playmode from JSON


    /// <summary>
    /// Add string to the current data
    /// </summary>
    /// <param name="dataInput">String to be Added</param>
    public void AddMoreData(string dataInput)
    {
        myOwnDataList.Add(dataInput);
        onDataAdded();
    }

    /// <summary>
    /// Add string to the current data at a specific Index
    /// </summary>
    /// <param name="dataInput">String to be added</param>
    /// <param name="atIndex">Index to inert into</param>
    public void AddMoreData(string dataInput, int atIndex)
    {
        myOwnDataList.Insert(atIndex, dataInput);
        onDataAdded();
    }

    /// <summary>
    /// Add an array of strings to the current data
    /// </summary>
    /// <param name="dataInput">Data Array</param>
    public void AddMoreData(string[] dataInput)
    {
        foreach (var item in dataInput)
        {
            myOwnDataList.Add(item);
        }

        onDataAdded();
    }

    /// <summary>
    /// Add a list of strings to the current data
    /// </summary>
    /// <param name="dataInput">Data List</param>
    public void AddMoreData(List<string> dataInput)
    {
        foreach (var item in dataInput)
        {
            myOwnDataList.Add(item);
        }

        onDataAdded();
    }

    /// <summary>
    /// Search and Remove the first Identical string found from the current data
    /// </summary>
    /// <param name="removeString">String to be removed, Note: it will only remove the first one it findes</param>
    public void RemoveFromDropdown(string removeString)
    {
        foreach (var name in myOwnDataList)
        {
            if (name == removeString)
            {
                myOwnDataList.Remove(name);
                break;
            }
        }

        onDataAdded();
    }

    /// <summary>
    /// Clear the entire List
    /// </summary>
    public void ClearDropdownData()
    {
        if (Application.isPlaying)
        {
            myOwnDataList.Clear();
        }

        onDataAdded();
    }

    /// <summary>
    /// Replace Current Data with a Data array
    /// </summary>
    /// <param name="dataInput">An array of strings to replace the current data with</param>
    public void ReplaceAllData(string[] dataInput)
    {
        myOwnDataList.Clear();
        foreach (var item in dataInput)
        {
            myOwnDataList.Add(item);
        }

        onDataAdded();
    }

    /// <summary>
    /// Replace Current Data with a Data list
    /// </summary>
    /// <param name="dataInput">A list of strings to replace the current data with</param>
    public void ReplaceAllData(List<string> dataInput)
    {
        myOwnDataList.Clear();
        foreach (var item in dataInput)
        {
            myOwnDataList.Add(item);
        }

        onDataAdded();
    }













    #region Testing the function aboved

    

    
    
    //Functions to test the above

    //Use these to test
    [ContextMenu(nameof(TestingAddSingleString))]
    public void TestingAddSingleString()
    {
        if (Application.isPlaying)
        {
            AddMoreData("TestingTesting");
        }
    }


    [ContextMenu(nameof(TestingAddStringArray))]
    public void TestingAddStringArray()
    {
        string[] injectionArray = {"A", "AA", "AAA", "B", "BB", "BBB", "C", "CC", "CCC", "D", "DD", "DDD", "E", "EE", "EEE"};

        if (Application.isPlaying)
        {
            AddMoreData(injectionArray);
        }
    }


    [ContextMenu(nameof(TestingClearAll))]
    public void TestingClearAll()
    {
        ClearDropdownData();
    }

    [ContextMenu(nameof(TestingReplace))]
    public void TestingReplace()
    {
        string[] testingArray = {"XX", "XXX" };
        ReplaceAllData(testingArray);
    }
    
    
    #endregion 
    
    
    
    
 

    public void findPlace()
    {
        OnlineMapsGooglePlacesAutocomplete.Find(
                       searchInput.text,
                        apiKey
                        ).OnComplete += OnComplete;

        
    }

    public  void OnComplete(string obj)
    {

        OnlineMapsGooglePlacesAutocompleteResult[] results = OnlineMapsGooglePlacesAutocomplete.GetResults(obj);

        List<string> testingInput = new List<string>();

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

           // Debug.Log(result.description);

           testingInput.Add(result.description);
        }

        ReplaceAllData(testingInput);

        tmp = GetComponent<SearchIntegrationWithTMP>();
        tmp.Searching();
    }
}
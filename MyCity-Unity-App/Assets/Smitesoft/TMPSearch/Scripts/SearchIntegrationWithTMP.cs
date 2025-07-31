using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Events;

public class SearchIntegrationWithTMP : MonoBehaviour
{
    #region variables

    public enum DataMode
    {
        ByReferencingDropDownList,
        ByManualInputViaInspector,
        ByScriptPopulation
    }

    

    [Header("Data Population Mode")]
    [SerializeField] private DataMode Mode;

    [Header("Reference Your Original DropDown (Here)")]
    [SerializeField] private TMP_Dropdown OriginalDropDown;
    [SerializeField] private RectTransform OriginalDropDownRectTransform;

    public string[] SlowDataEntry;
    PopulateSearchingData ManualArrayScriptReference;
   


    GameObject[] BlockersArray;

    Transform[] HiddenDropDownChilden;

    String[] OriginalDataArray;
    List<String> OriginalDataList = new List<String>();
    List<String> TempList = new List<String>();

    

    GameObject HiddenDropDownChildLabel;
    GameObject OriginalDropDownChildLabel;
    TextMeshProUGUI HiddenLabelGUI;
    TextMeshProUGUI OriginalLabelGUI;



    GameObject HiddenDropDownChildItemLabel;
    GameObject OriginalDropDownChildItemLabel;
    TextMeshProUGUI HiddenItemLabelGUI;
    TextMeshProUGUI OriginalItemLabelGUI;

    [Header("Preferences")]
    [SerializeField] private bool SearchBoxDisapearsIfListShowsOnlyOneResult;
    [SerializeField] private bool SearchFeaturesForceShareSameParentAsDropDown;


    [Header("Search Input type")]  //Add tolltip " Defult set to (Standard/Name)"
    [SerializeField] private TMP_InputFieldSearchable.ContentType Type = TMP_InputFieldSearchable.ContentType.Name;
    [SerializeField] private bool CaseSensativeSearch;
    

    [Header("Search Input Text Preferences")]
    [SerializeField] private bool CopyDropDownTextStyle;
    [SerializeField] private bool CopyDropDownColoursAndAppearance;
    [SerializeField] private bool CopyDropDownListTextFormating;
    [SerializeField] private bool CopyDropDownListTextAlignmentAndSpacing;

    [Header("Search Input Box Anchoring - This Is Optional")]
    [SerializeField] private bool EclipseDropdown;
    [SerializeField] private bool AutoAnchorSearchTextInput;
    [Range(-200f, 200f)]
    [SerializeField] private float AdjustSearchTextInputProximity = 0;


    public enum SearchTextInput
    {
        AutoAnchorAndAdjustSearchTextInputSize,
        AutoAnchorWithManualAdjustSearchTextInputSize,
        FullyManualAnchorAndAdjustSearchTextInputSize
    }

    [Header("Search Input Box Anchoring Dimentions - This Is Optional")]
    [SerializeField] private SearchTextInput SearchTextInputAnchoringMode;
    
    bool AutoAnchorAndAdjustSearchTextInputSizeBool;
    bool AutoAnchorWithManualAdjustSearchTextInputSizeBool;
    bool FullyManualAnchorAndAdjustSearchTextInputSizeBool;
    [SerializeField] private float SearchTextInputHight = 30f;
    [SerializeField] private float SearchTextInputWidth = 160f;
    [Range(0.1f, 2f)]
    [SerializeField] private float SearchTextInputSizeMultiplier = 1;

    public enum ButtonPosition
    {
        AnhcorButtonLeft,
        AnhcorButtonRight
    }  

    [Header("Search Button Anchoring - This Is Optional")]
    [SerializeField] private bool UseSearchButton;
    [SerializeField] private bool AutoAnchorSearchButton;
    [SerializeField] private ButtonPosition ButtonAnchorPosition;    
    [Range(-200f, 200f)]
    [SerializeField] private float AdjustButtonProximity = 0;
    bool AnhcorButtonLeftBool; //If Auto Ancho is off, I want these not to show in the inspector - I need to learn Editor scripting for this
    bool AnhcorButtonRightBool; //If Auto Ancho is off, I want these not to show in the inspector - I need to learn Editor scripting for this


    public enum ButtonAnchor
    {
        AutoAdjustButtonSize,
        AutoAnchorWithManualAdjustButtomSize,
        FullyManualAnchorAndAdjustButtomSize
    }

    [Header("Search Button Dimentions - This Is Optional")]
    [SerializeField] private ButtonAnchor SearchButtonDimetionsMode;

    bool AutoAdjustButtonSizeBool;
    bool ManualAdjustButtomSizeBool;
    bool FullyManualAnchorAndAdjustButtomSizeBool;
    [SerializeField] private float ButtonHight = 30f;
    [SerializeField] private float ButtonWidth = 30f;
    [Range(0.1f, 2f)]
    [SerializeField] private float ButtonSizeMultiplier = 1;

    [Header("Default References")]
    [SerializeField] private bool ManuallyAssignCanvus; // Add tooltip, Only Use if you have multiple canvasus   
    [SerializeField] private Transform ParentCanvus;  //set your parent canvus, otherwise it will try and do it for you

    [SerializeField] private bool UseMyOwnSearchInputField;
    [SerializeField] private TMP_InputFieldSearchable SearchInput;
    [SerializeField] private RectTransform SearchInputRectTransform;

    [SerializeField] private bool UseMyOwnButton;
    [SerializeField] private Button SearchButton;
    [SerializeField] private RectTransform SearchButtonRectTransform;



    [SerializeField] private TMP_DropdownSearchable HiddenDropDown;
    [SerializeField] private RectTransform HiddenDropDownRectTransform;

    [SerializeField] private TMP_Dropdown ManualDropDown;
    [SerializeField] private RectTransform ManualDropDownRectTransform;


    GameObject SearchInputChildText;
    TextMeshProUGUI SearchInputChildTextGUI;

    int ChangedEnumSwitch = 1;


    //23.02.2021 Adding New Options
    [SerializeField] private bool AutoSelectIfOnlyOneResult;
    [SerializeField] private bool UseInputFieldToShowSelectionResult;
    [SerializeField] private bool DisableInputFieldOnFocusLoss;
    // [SerializeField] private bool EclipseDropdown; //moved up the page


    #endregion

    private void Awake()   //Added 22/03/2021
    {
        ManualArrayScriptReference = GetComponent<PopulateSearchingData>();
        ManualArrayScriptReference.onDataAdded += UpdateSearchingDataViaScript; //Added 22/03/2021
    }

    void Start()
    {
       // ManualDropDown.onValueChanged.AddListener(SubscribingToDropDownOnValueChange);
        OriginalDropDown.onValueChanged.AddListener(SubscribingToDropDownOnValueChange);






        // SubscribingToDropDownOnValueChange()




        switch (Mode)
        {
            case DataMode.ByReferencingDropDownList:   //Use Editor Scripting to show the referencing options for Origininal Drop Down
               
                if (OriginalDropDown == null || OriginalDropDownRectTransform == null)
                {
                    Debug.Log("You Forgot to reference you Drop Down!, Otherwise change Mode. Watch youtube Guide if you get stuck");
                }
                break;

            case DataMode.ByManualInputViaInspector:   //Use Editor Scripting to show Array input Field


                OriginalDropDown = null;
                OriginalDropDownRectTransform = null;
                ActivateOurOwnDropDown();

                foreach (string item in SlowDataEntry)
                {
                    TempList.Add(item); //we can recycle Temp List because it will get cleared anyway
                }
                ManualDropDown.ClearOptions();
                ManualDropDown.AddOptions(TempList);

                //Deparent here
                break;

            case DataMode.ByScriptPopulation:               

                OriginalDropDown = null;
                OriginalDropDownRectTransform = null;
                ActivateOurOwnDropDown();

                ManualArrayScriptReference = GetComponent<PopulateSearchingData>();  //Changed to Get componenet 22/03/2021

                foreach (string item in ManualArrayScriptReference.myOwnDataList)
                {
                    TempList.Add(item);
                }
                ManualDropDown.ClearOptions();
                ManualDropDown.AddOptions(TempList);
                //Deparent here
                break;


            default:
                Debug.Log("D?");
                //I think I am suppose to put the rest of my awake function code here or somthing??
                break;
        }

        switch (SearchTextInputAnchoringMode)
        {
            case SearchTextInput.AutoAnchorAndAdjustSearchTextInputSize:
                AutoAnchorAndAdjustSearchTextInputSizeBool = true;
                AutoAnchorWithManualAdjustSearchTextInputSizeBool = false;
                FullyManualAnchorAndAdjustSearchTextInputSizeBool = false;
                break;
            case SearchTextInput.AutoAnchorWithManualAdjustSearchTextInputSize:
                AutoAnchorAndAdjustSearchTextInputSizeBool = false;
                AutoAnchorWithManualAdjustSearchTextInputSizeBool = true;
                FullyManualAnchorAndAdjustSearchTextInputSizeBool = false;
                break;
            case SearchTextInput.FullyManualAnchorAndAdjustSearchTextInputSize:
                AutoAnchorAndAdjustSearchTextInputSizeBool = false;
                AutoAnchorWithManualAdjustSearchTextInputSizeBool = false;
                FullyManualAnchorAndAdjustSearchTextInputSizeBool = true;
                break;
            default:
                break;
        }

        switch (ButtonAnchorPosition)
        {
            case ButtonPosition.AnhcorButtonLeft:
                AnhcorButtonLeftBool = true;
                AnhcorButtonRightBool = false;
                break;
            case ButtonPosition.AnhcorButtonRight:
                AnhcorButtonLeftBool = false;
                AnhcorButtonRightBool = true;
                break;
            default:
                break;
        }

        switch (SearchButtonDimetionsMode)
        {
            case ButtonAnchor.AutoAdjustButtonSize:
                AutoAdjustButtonSizeBool = true;
                ManualAdjustButtomSizeBool = false;
                FullyManualAnchorAndAdjustButtomSizeBool = false;
                break;

            case ButtonAnchor.AutoAnchorWithManualAdjustButtomSize:
                AutoAdjustButtonSizeBool = false;
                ManualAdjustButtomSizeBool = true;
                FullyManualAnchorAndAdjustButtomSizeBool = false;
                break;

            case ButtonAnchor.FullyManualAnchorAndAdjustButtomSize:
                AutoAdjustButtonSizeBool = false;
                ManualAdjustButtomSizeBool = false;
                FullyManualAnchorAndAdjustButtomSizeBool = true;
                break;
            default:
                break;
        }


        if (ParentCanvus == null)
        {
            if (gameObject.transform.parent != null)
            {
                ParentCanvus = gameObject.transform.parent;
            }
        }

        HiddenDropDown.gameObject.SetActive(true);
        SearchInput.gameObject.SetActive(true);

        if (UseSearchButton)
        {
            SearchButton.gameObject.SetActive(true);
            SearchInput.gameObject.SetActive(false);
        }
        else
        {
            SearchButton.gameObject.SetActive(false);
        }

        //Force parent to Canvus if the user has not Unparented them manualy.
        //Make sure you drag the prefab to the main Canvus

        if (ParentCanvus != null)  //Will this Cause a problem if they are prefabed?? //They Will need to unpack prefab Completely
        {
            if (SearchInput.gameObject.transform.parent == transform.parent)
            {
                //SearchInput.gameObject.transform.parent = ParentCanvus;
                SearchInput.gameObject.transform.SetParent(ParentCanvus);  //Added on 22-03-2021 //works on version 2019.4.17f1, need to test on other versions 
            }
            if (HiddenDropDown.gameObject.transform.parent == transform.parent)
            {
                //  HiddenDropDown.gameObject.transform.parent = ParentCanvus;
                HiddenDropDown.gameObject.transform.SetParent(ParentCanvus); //Added on 22-03-2021 //works on version 2019.4.17f1, need to test on other versions 
            }
            if (SearchButton.gameObject.transform.parent == transform.parent)
            {
                //SearchButton.gameObject.transform.parent = ParentCanvus; 
                SearchButton.gameObject.transform.SetParent(ParentCanvus);   //Added on 22-03-2021 //works on version 2019.4.17f1, need to test on other versions  
            }
        }

        for (int i = 0; i < (OriginalDropDown.options.Count); i++)
        {
            OriginalDataList.Add(OriginalDropDown.options[i].text);
        }

        OriginalDataArray = OriginalDataList.ToArray();



        ApplyAllFunctions();

    }

    private void ActivateOurOwnDropDown()
    {
        if (OriginalDropDown == null || OriginalDropDownRectTransform == null)
        {
            ManualDropDown.gameObject.SetActive(true);
            OriginalDropDown = ManualDropDown;
            OriginalDropDownRectTransform = ManualDropDownRectTransform;
        }
    }

    public void ApplyAllFunctions()
    {
        DropDownMimicOperations();

        SearchTextInputAppearance();
        SearchTextInputPlacemnts();
        SearchTextInputSize();

        SearchButtonPlacemnts();
        SearchButtonSize();
    }

    void Update()
    {
      
    }





    void LateUpdate()
    {
        if (!UseInputFieldToShowSelectionResult)
        {
            if (DisableInputFieldOnFocusLoss)
            {
                if (UseSearchButton)
                {
                    if (SearchInput.isActiveAndEnabled)
                    {
                        if (!SearchInput.isFocused)
                        {
                            Invoke(nameof(turnOffSearchInput), 0.2f);  //This Delay is required because we always Unfocus after we type and refocus
                        }
                    }
                }
            }            
        }               
    }

    private void turnOffSearchInput()
    {
        if (SearchInput.isActiveAndEnabled)
        {
            if (!SearchInput.isFocused)
            {
                SearchInput.gameObject.SetActive(false);
            }
            else
            {
                CancelInvoke(nameof(turnOffSearchInput));
            }
        }
    }

    
    private void UpdateSearchingDataViaScript() //added 22/03/2021
    {
        OriginalDropDown = null;
        OriginalDropDownRectTransform = null;
        ActivateOurOwnDropDown();

        ManualArrayScriptReference = GetComponent<PopulateSearchingData>();  //Changed to Get componenet 22/03/2021

        
        
        TempList.Clear(); //added 22/03/2021
        foreach (string item in ManualArrayScriptReference.myOwnDataList)
        {
            TempList.Add(item);
        }
        ManualDropDown.ClearOptions();

        ManualDropDown.AddOptions(TempList);
        
        OriginalDataList.Clear(); // added 06/10/2021

        for (int i = 0; i < (OriginalDropDown.options.Count); i++)
        {
            OriginalDataList.Add(OriginalDropDown.options[i].text);
        }
        
        OriginalDataArray = OriginalDataList.ToArray();  
    }


    #region Logic


    private void OnEnable()
    {
        HiddenDropDown.activateBlockerTagging = true;
        HiddenDropDown.onNoValueChange += SearchInject; //23.02.2021
    }

    private void OnDisable() //23.02.2021
    {
        HiddenDropDown.onNoValueChange -= SearchInject; //23.02.2021
    }

    public void Searching()
    {
        if (OriginalDropDownOnValueChange) //Added on 22-03-2021
        {
            return; //Added on 22-03-2021
        }
        //if (onValueNoChange) //23.02.2021
        //{
        //    Debug.Log("Skip once"); //this is wrong
        //    onValueNoChange = false; //23.02.2021
        //    return; //23.02.2021
        //}

        SearchInput.SkipDeactivationFunction(true); // this is important on many counts, one of which is to prevent Mobile keyboard from activating // //23.02.2021 this was commented out by accident
        HiddenDropDown.gameObject.SetActive(true);

        if (HiddenDropDown.IsExpanded && TempList.Count > 0) //&& TempList.Count > 1  //shold be > 0 not 1
        {
            HiddenDropDown.ImmediateDestroyDropdownList(); // I Had to make this Public from TMP original Script "ImmediateDestroyDropdownList()", I did this to skip the co-routine
        }

        TempList.Clear();  // Refresh List every time we add text

        if (CaseSensativeSearch)
        {
            if (SearchInput.text.Length >= 0)
            {
                foreach (string Mon in OriginalDataList)
                {
                    if (Mon.Contains(SearchInput.text))
                    {
                        TempList.Add(Mon);  // Temp List Creation
                    }
                }

                HiddenDropDown.ClearOptions();
                HiddenDropDown.AddOptions(TempList);

                SearchInject();
            }
        }
        else
        {
            if (SearchInput.text.Length >= 0)
            {
                foreach (string Mon in OriginalDataList)
                {
                    if (Mon.ToLower().Contains(SearchInput.text.ToLower()))  // or if lowercase.Mon.Contains (LowerCase.searchInput.Text), I would need to learn how to do lower case here, and check weather it may have problems with numbers
                    {
                        TempList.Add(Mon);  // Temp List Creation
                    }
                }

                HiddenDropDown.ClearOptions();
                HiddenDropDown.AddOptions(TempList);

                SearchInject();
            }
        }


        if (!HiddenDropDown.IsExpanded)
        {
            HiddenDropDown.Show();
        }

        SearchInput.Select();

        SearchInput.SkipDeactivationFunction(false); //move it here should  (was at the bottom of this function) //It only needs to go before "SearchInput.Select();"



        if (TempList.Count < 2 ) //Add A bool to make this optional 23/02/2021 //Maybe this should be ==1
        {
            if (AutoSelectIfOnlyOneResult)
            {
                HiddenDropDown.gameObject.SetActive(false);                
            }

            if (SearchBoxDisapearsIfListShowsOnlyOneResult)
            {
                SearchInput.gameObject.SetActive(false);
            }
        }



        //BlockersArray = GameObject.FindGameObjectsWithTag("Blocker");

        //if (BlockersArray.Length > 1)
        //{
        //    Destroy(BlockersArray[0].gameObject);
        //}


        
        if (TMP_DropdownSearchable.blockerList.Count > 1)                                     // 15-03-2021 replacing the tag System
        {           
            Destroy(TMP_DropdownSearchable.blockerList[0]);                                   // 15-03-2021 replacing the tag System
            TMP_DropdownSearchable.blockerList.Remove(TMP_DropdownSearchable.blockerList[0]); // 15-03-2021 replacing the tag System
        }
    }


    #endregion



    public void SearchInject() //this does not run by mous clicking the dropdown option that is already the default selection
    {
        if (TempList.Count > 0) // try >=
        {
            string selectedMonName = TempList[HiddenDropDown.value];   // now I have to run this with the array of names and find the Index value of that name.
            int selectedMonIndex;

            selectedMonIndex = System.Array.IndexOf(OriginalDataArray, selectedMonName);
            OriginalDropDown.value = selectedMonIndex;

     


            if (UseInputFieldToShowSelectionResult) //23.02.2021
            {
                StopAllCoroutines();
                StartCoroutine(InjectDataBackIntoImputField(selectedMonName));
            }
         
        }
    }

    // bool onValueNoChange; //23.02.2021

    int lastInputWordLength = 0;
    IEnumerator InjectDataBackIntoImputField(string SelectionName) //23.02.2021   //This Whole thing was stupid, Should Have subscribed to "actual drop down" (On Value Change event)???
    {                                                              //...not so sure..or not that thing flickers 
            
        yield return new WaitForSeconds(.2f);
        if (SearchInput.text.Length >= lastInputWordLength) //Also need to check what text it contains maybe
        {
            if (AutoSelectIfOnlyOneResult)
            {
                if (TempList.Count == 1)
                {
                    SearchInput.text = SelectionName;
                    SearchInput.MoveTextEnd(false); // false = no hilighting, true = with Highlightining                       
                }
            }  
            
            if (!HiddenDropDown.IsExpanded)
            {
                SearchInput.text = SelectionName;
                SearchInput.MoveTextEnd(false);
                HiddenDropDown.ImmediateDestroyDropdownList();



                //BlockersArray = GameObject.FindGameObjectsWithTag("Blocker");  // 15-03-2021

                //if (BlockersArray.Length > 0)                                  // 15-03-2021
                //{
                //    Destroy(BlockersArray[0].gameObject);                      // 15-03-2021
                //}

                if (TMP_DropdownSearchable.blockerList.Count > 0)                                       // 15-03-2021 replacing the tag System
                {
                    Destroy(TMP_DropdownSearchable.blockerList[0]);                                     // 15-03-2021 replacing the tag System
                    TMP_DropdownSearchable.blockerList.Remove(TMP_DropdownSearchable.blockerList[0]);   // 15-03-2021 replacing the tag System
                }
            }

            //if (!HiddenDropDown.IsExpanded)
            //{
            //    SearchInput.text = SelectionName;
            //}
            //if (AutoSelectIfOnlyOneResult)
            //{
            //    SearchInput.MoveTextEnd(false); // false = no hilighting, true = with Highlightining            
            //}            
            // onValueNoChange = true;
            
        }         
        lastInputWordLength = SearchInput.text.Length;

        //BlockersArray = GameObject.FindGameObjectsWithTag("Blocker");  // 15-03-2021 

        //if (BlockersArray.Length > 1)                                  // 15-03-2021
        //{
        //    Destroy(BlockersArray[0].gameObject);                      // 15-03-2021
        //} 

        if (TMP_DropdownSearchable.blockerList.Count > 1)                                       // 15-03-2021 replacing the tag System
        {
            Destroy(TMP_DropdownSearchable.blockerList[0]);                                     // 15-03-2021 replacing the tag System
            TMP_DropdownSearchable.blockerList.Remove(TMP_DropdownSearchable.blockerList[0]);   // 15-03-2021 replacing the tag System
        }
    }

    bool OriginalDropDownOnValueChange = false; //Added on 22-03-2021
    public void SubscribingToDropDownOnValueChange(int val) //Added on 22-03-2021
    {
        OriginalDropDownOnValueChange = true; //Added on 22-03-2021

        if (!SearchInput.isFocused && UseInputFieldToShowSelectionResult) //Added on 22-03-2021
        {
            SearchInput.text = OriginalDropDown.options[val].text; //Added on 22-03-2021
            SearchInput.MoveTextEnd(false); //Added on 22-03-2021
        }
        OriginalDropDownOnValueChange = false; //Added on 22-03-2021
    }

    public void searchButton()
    {

        if (SearchInput.text.Length > 0)
        {
            SearchInput.text = "";
        }      

        if (SearchInput.isActiveAndEnabled)
        {
            SearchInput.text = "";
            SearchInput.gameObject.SetActive(false);            
        }
        else
        {
            SearchInput.gameObject.SetActive(true);
            SearchInput.text = "";
            SearchInput.ActivateInputField();
        }
    }










    private void DropDownMimicOperations()
    {
        if (SearchFeaturesForceShareSameParentAsDropDown)
        {
            //HiddenDropDown.transform.parent = OriginalDropDownRectTransform.parent.transform;      //We did this because we are using Anchored Position, and not position, (perhaps we should make this into an option)??
            HiddenDropDown.transform.SetParent(OriginalDropDownRectTransform.parent.transform);      //Added on 22-03-2021 //works on version 2019.4.17f1, need to test on other versions 

            HiddenDropDown.transform.SetSiblingIndex(OriginalDropDown.transform.GetSiblingIndex());
            HiddenDropDownRectTransform.anchoredPosition = OriginalDropDownRectTransform.anchoredPosition;            
        }
        else
        {
            // HiddenDropDown.transform.SetSiblingIndex(OriginalDropDown.transform.GetSiblingIndex());   // this works if theres no parenting or if this shares the same parent
            int HiearcharyPosition = Mathf.Max((OriginalDropDownRectTransform.parent.transform.GetSiblingIndex() - 1), 0); // I changed it from "OriginalDropDown.transform.GetSiblingIndex()" 
            
            
            
            //HiddenDropDown.transform.SetSiblingIndex(HiearcharyPosition);                       //This is the old function before Parenting them in preparation for prefabing (instead we should move the parent) = next line
            gameObject.transform.SetSiblingIndex(HiearcharyPosition);                             //This replaced the Line above as a result of parenting for prefabing
            HiddenDropDownRectTransform.position = OriginalDropDownRectTransform.position;
        }
        

        //HiddenDropDown.transform.parent = OriginalDropDownRectTransform.parent.transform; //We did this because we are using Anchored Position, and not position, (perhaps we should make this into an option)??
        //HiddenDropDown.transform.SetSiblingIndex(OriginalDropDown.transform.GetSiblingIndex());
        //Debug.Log("OriginalDropDown.transform.GetSiblingIndex()  " + OriginalDropDown.transform.GetSiblingIndex());



        SearchInput.onFocusSelectAll = false;
        SearchInput.resetOnDeActivation = true;
        SearchInput.restoreOriginalTextOnEscape = true;
        SearchInput.contentType = Type;

        //HiddenDropDownRectTransform.anchoredPosition = OriginalDropDownRectTransform.anchoredPosition;  //This is local position (which is different from Position only when this object has a parent)) // this doesnt work if the target is parents, thats why we are using the same parent
        //HiddenDropDownRectTransform.position = OriginalDropDownRectTransform.position;    //This is actually overriding Anchored Position (this Also WOrks weather or nor the target is prentd)
        HiddenDropDownRectTransform.sizeDelta = OriginalDropDownRectTransform.sizeDelta;  //this is what changes the size

        HiddenDropDown.targetGraphic = OriginalDropDown.targetGraphic;
        HiddenDropDown.colors = OriginalDropDown.colors;
        HiddenDropDown.interactable = OriginalDropDown.interactable;
        HiddenDropDown.navigation = OriginalDropDown.navigation;

        //HiddenDropDown.captionImage = OriginalDropDown.captionImage;  // I should not change these, this will make it change the original (and its only visual - no actual data change)
        //HiddenDropDown.captionText = OriginalDropDown.captionText;    // I should not change these, this will make it change the original (and its only visual - no actual data change)
        //HiddenDropDown.itemImage = OriginalDropDown.itemImage;        // I should not change these, this will make it change the original (and its only visual - no actual data change)

                 

        //==========Child : "Label"  (This is not as Important as it Will Likely Be Hidden Underneath-Needs Testing)================
        ChildLabelDataInjection();

        //===================Child : "Item Label"  (Many More Options to add - SHould add it later)=======================
        ChildItemLabelDataInjection();
    }

    private void ChildLabelDataInjection()
    {
        HiddenDropDownChildLabel = HiddenDropDown.transform.GetChild(0).gameObject;
        OriginalDropDownChildLabel = OriginalDropDown.transform.GetChild(0).gameObject;

        HiddenDropDownChildLabel.TryGetComponent<TextMeshProUGUI>(out HiddenLabelGUI);
        OriginalDropDownChildLabel.TryGetComponent<TextMeshProUGUI>(out OriginalLabelGUI);

        HiddenLabelGUI.fontSize = OriginalLabelGUI.fontSize;
        HiddenLabelGUI.font = OriginalLabelGUI.font;
        HiddenLabelGUI.enableAutoSizing = OriginalLabelGUI.enableAutoSizing;
        HiddenLabelGUI.color = OriginalLabelGUI.color;
        HiddenLabelGUI.colorGradient = OriginalLabelGUI.colorGradient;
        HiddenLabelGUI.alignment = OriginalLabelGUI.alignment;
    }

    private void ChildItemLabelDataInjection()
    {
        HiddenDropDownChildItemLabel = HiddenDropDown.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        OriginalDropDownChildItemLabel = OriginalDropDown.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;

        HiddenDropDownChildItemLabel.TryGetComponent<TextMeshProUGUI>(out HiddenItemLabelGUI);
        OriginalDropDownChildItemLabel.TryGetComponent<TextMeshProUGUI>(out OriginalItemLabelGUI);

        HiddenItemLabelGUI.fontSize = OriginalItemLabelGUI.fontSize; //OMG it actually works lol
        HiddenItemLabelGUI.font = OriginalItemLabelGUI.font;
        HiddenItemLabelGUI.enableAutoSizing = OriginalItemLabelGUI.enableAutoSizing;
        HiddenItemLabelGUI.color = OriginalItemLabelGUI.color;
        HiddenItemLabelGUI.colorGradient = OriginalItemLabelGUI.colorGradient;
        HiddenItemLabelGUI.alignment = OriginalItemLabelGUI.alignment;
        HiddenItemLabelGUI.enableWordWrapping = OriginalItemLabelGUI.enableWordWrapping;
        HiddenItemLabelGUI.overflowMode = OriginalItemLabelGUI.overflowMode;
        HiddenItemLabelGUI.horizontalMapping = OriginalItemLabelGUI.horizontalMapping;
        HiddenItemLabelGUI.verticalMapping = OriginalItemLabelGUI.verticalMapping;
    }


    #region Search Input Functions
    private void SearchTextInputAppearance()
    {
        SearchInputChildText = SearchInput.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        SearchInputChildText.TryGetComponent<TextMeshProUGUI>(out SearchInputChildTextGUI);

        

        if (CopyDropDownTextStyle)
        {
            SearchInputChildTextGUI.font = OriginalLabelGUI.font;           
        }

        if(CopyDropDownColoursAndAppearance)
        {
            SearchInputChildTextGUI.color = OriginalLabelGUI.color;
        }

        if (CopyDropDownListTextFormating)
        {
            SearchInputChildTextGUI.fontStyle = OriginalLabelGUI.fontStyle;
            SearchInputChildTextGUI.fontSize = OriginalLabelGUI.fontSize;
            
        }

        if (CopyDropDownListTextAlignmentAndSpacing)
        {
            SearchInputChildTextGUI.alignment = OriginalLabelGUI.alignment;
            SearchInputChildTextGUI.characterSpacing = OriginalLabelGUI.characterSpacing;
            SearchInputChildTextGUI.lineSpacing = OriginalLabelGUI.lineSpacing;
            SearchInputChildTextGUI.paragraphSpacing = OriginalLabelGUI.paragraphSpacing;
            SearchInputChildTextGUI.wordSpacing = OriginalLabelGUI.wordSpacing;
        }      
    }

    private void SearchTextInputPlacemnts()
    {
        if (OriginalDropDown == null || OriginalDropDownRectTransform == null)
        {
            //Debug.Log("You Need to Referenace Your Drop Down, or change mode!");
            //SearchInput.gameObject.SetActive(false);   //For Some reason this will end Up not updating Scene View if activated        
            return;
        }

        if (!EclipseDropdown)
        {
            if (SearchFeaturesForceShareSameParentAsDropDown)   //This Forces Share the same parent as the drop down, so if the dropdown direct parent is a canvas for the Hidden dropdown and search input also)
            {                                                   //.. if the parent is SearchDropDownManager, the the parent will also be search drop down manager

                //SearchInput.transform.parent = OriginalDropDownRectTransform.parent.transform;
                SearchInput.transform.SetParent(OriginalDropDownRectTransform.parent.transform); //Added on 22-03-2021 //works on version 2019.4.17f1, need to test on other versions 

                if (AutoAnchorSearchTextInput)
                {

                    float Gap = OriginalDropDownRectTransform.rect.height * 0.5f + AdjustSearchTextInputProximity;

                    Vector2 SizeDiff = new Vector2(0f, ((OriginalDropDownRectTransform.rect.height / 2f) + Gap));

                    SearchInputRectTransform.anchoredPosition = OriginalDropDownRectTransform.anchoredPosition + SizeDiff; //OriginalDropDownRectTransform.anchoredPosition
                                                                                                                           //SearchInputRectTransform.position = OriginalDropDownRectTransform.position;  //This Overides Anchored Position                       
                }
            }
            else
            {
                if (AutoAnchorSearchTextInput)
                {
                    //I u Must do the calculations based on SearchInputRectTransform.position
                    float Gap = OriginalDropDownRectTransform.rect.height * 0.5f + AdjustSearchTextInputProximity;

                    Vector3 SizeDiff = new Vector3(0f, ((OriginalDropDownRectTransform.rect.height / 2f) + Gap), 0f);
                    SearchInputRectTransform.position = OriginalDropDownRectTransform.position + SizeDiff;
                }
            }
        }
        else
        {
            if (SearchFeaturesForceShareSameParentAsDropDown)   //This Forces Share the same parent as the drop down, so if the dropdown direct parent is a canvas for the Hidden dropdown and search input also)
            {                                                   //.. if the parent is SearchDropDownManager, the the parent will also be search drop down manager

                //SearchInput.transform.parent = OriginalDropDownRectTransform.parent.transform;
                SearchInput.transform.SetParent(OriginalDropDownRectTransform.parent.transform); //Added on 22-03-2021 //works on version 2019.4.17f1, need to test on other versions 
                SearchInputRectTransform.anchoredPosition = OriginalDropDownRectTransform.anchoredPosition;
            }
            else
            {
                SearchInputRectTransform.anchoredPosition = OriginalDropDownRectTransform.anchoredPosition;
            }
        }

        
        
    }

    private void SearchTextInputSize()
    {
        if (AutoAnchorAndAdjustSearchTextInputSizeBool)
        {
            SearchTextInputSizeMultiplier = 1;
            Vector2 HightAdjustmnt = new Vector2(OriginalDropDownRectTransform.rect.width * SearchTextInputSizeMultiplier, OriginalDropDownRectTransform.rect.height * SearchTextInputSizeMultiplier);
            SearchInputRectTransform.sizeDelta = HightAdjustmnt;
        }
        else
        if (AutoAnchorWithManualAdjustSearchTextInputSizeBool)
        {
            Vector2 HightAdjustmnt = new Vector2(SearchTextInputWidth * SearchTextInputSizeMultiplier, SearchTextInputHight * SearchTextInputSizeMultiplier);
            SearchInputRectTransform.sizeDelta = HightAdjustmnt;
        }
        else
        if (FullyManualAnchorAndAdjustSearchTextInputSizeBool)
        {
            // They use their own - Or I can add it here for them... yes I should add it ( I added it under manual)
            //Vector2 HightAdjustmnt = new Vector2(OriginalDropDownRectTransform.rect.height, OriginalDropDownRectTransform.rect.height);
            //SearchButtonRectTransform.sizeDelta = HightAdjustmnt;
        }
    }
    #endregion


    #region Search Icon Button Functions
    private void SearchButtonPlacemnts()
    {
        if (SearchFeaturesForceShareSameParentAsDropDown)
        {
            // SearchButton.transform.parent = OriginalDropDownRectTransform.parent.transform;
            SearchButton.transform.SetParent(OriginalDropDownRectTransform.parent.transform); //Added on 22-03-2021 //works on version 2019.4.17f1, need to test on other versions 

            if (AutoAnchorSearchButton)
            {              

                float Direction = 0;
                float Gap = 0;

                if (AnhcorButtonRightBool)
                {
                    Direction = 1;
                    Gap = OriginalDropDownRectTransform.rect.width * 0.15f + AdjustButtonProximity;
                }
                else
                if (AnhcorButtonLeftBool)
                {
                    Direction = -1;
                    Gap = OriginalDropDownRectTransform.rect.width * -0.15f - AdjustButtonProximity;
                }
                else
                {
                    Gap = AdjustButtonProximity;
                }

                Vector2 SizeDiff = new Vector2(((OriginalDropDownRectTransform.rect.width / 2f) * Direction + Gap), 0f);
                SearchButtonRectTransform.anchoredPosition = OriginalDropDownRectTransform.anchoredPosition + SizeDiff; //OriginalDropDownRectTransform.anchoredPosition
            }
        }
        else
        {
            if (AutoAnchorSearchButton)
            {
                //I u Must do the calculations based on SearchButtonRectTransform.position
                float Direction = 0;
                float Gap = 0;

                if (AnhcorButtonRightBool)
                {
                    Direction = 1;
                    Gap = OriginalDropDownRectTransform.rect.width * 0.15f + AdjustButtonProximity;
                }
                else
                if (AnhcorButtonLeftBool)
                {
                    Direction = -1;
                    Gap = OriginalDropDownRectTransform.rect.width * -0.15f - AdjustButtonProximity;
                }
                else
                {
                    Gap = AdjustButtonProximity;
                }

                Vector3 SizeDiff = new Vector3(((OriginalDropDownRectTransform.rect.width / 2f) * Direction + Gap), 0f, 0f);
                SearchButtonRectTransform.position = OriginalDropDownRectTransform.position + SizeDiff;

            }
        }      
    }
    
    private void SearchButtonSize()
    {
        if (AutoAdjustButtonSizeBool)
        {
            ButtonSizeMultiplier = 1;
            Vector2 HightAdjustmnt = new Vector2(OriginalDropDownRectTransform.rect.height * ButtonSizeMultiplier, OriginalDropDownRectTransform.rect.height * ButtonSizeMultiplier);
            SearchButtonRectTransform.sizeDelta = HightAdjustmnt;
        }
        else
        if (ManualAdjustButtomSizeBool)
        {
            Vector2 HightAdjustmnt = new Vector2(ButtonHight * ButtonSizeMultiplier, ButtonWidth * ButtonSizeMultiplier);
            SearchButtonRectTransform.sizeDelta = HightAdjustmnt;
        }
        else
        if(FullyManualAnchorAndAdjustButtomSizeBool)
        {
            // They use their own - Or I can add it here for them... yes I should add it
            //Vector2 HightAdjustmnt = new Vector2(OriginalDropDownRectTransform.rect.height, OriginalDropDownRectTransform.rect.height);
            //SearchButtonRectTransform.sizeDelta = HightAdjustmnt;
        }
    }
    #endregion





    #region Editor Function Extentions

    public void EditorRefreshAllFunctions()
    {
        ModeSelectionEditorExtention();
        SearchInputEditorExtentionShowHide();
        SearchInputPlacemntsEditorExtention();
        SearchInputSizingEditorExtention();
       // SearchButtonEditorExtentionShowHide();
        SearchButtonPlacemntsEditorExtention();
        SearchButtonSizingEditorExtention();
    }

    public void ModeSelectionEditorExtention()
    {
       
        switch (Mode)
        {
            case DataMode.ByReferencingDropDownList:   //Use Editor Scripting to show the referencing options for Origininal Drop Down

                if (ChangedEnumSwitch != 1)
                {
                    OriginalDropDown = null;
                    OriginalDropDownRectTransform = null;
                    ChangedEnumSwitch = 1;
                }

                
                if (OriginalDropDown == null || OriginalDropDownRectTransform == null)
                {
                    ManualDropDown.gameObject.SetActive(false);
                }                 
                break;

            case DataMode.ByManualInputViaInspector:   //Use Editor Scripting to show Array input Field

                ChangedEnumSwitch = 2;
                OriginalDropDown = null;
                OriginalDropDownRectTransform = null;
                ManualDropDown.gameObject.SetActive(true);
                OriginalDropDown = ManualDropDown;
                OriginalDropDownRectTransform = ManualDropDownRectTransform;

                foreach (string item in SlowDataEntry)
                {
                    TempList.Add(item); //we can recycle Temp List because it will get cleared anyway
                }
                ManualDropDown.ClearOptions();
                ManualDropDown.AddOptions(TempList);               
                //Deparent here
                break;

            case DataMode.ByScriptPopulation:

                ChangedEnumSwitch = 2;
                OriginalDropDown = null;
                OriginalDropDownRectTransform = null;
                ManualDropDown.gameObject.SetActive(true);
                OriginalDropDown = ManualDropDown;
                OriginalDropDownRectTransform = ManualDropDownRectTransform;

                ManualArrayScriptReference = GetComponent<PopulateSearchingData>();  //Changed to Get componenet 22/03/2021

                foreach (string item in ManualArrayScriptReference.myOwnDataList)
                {
                    TempList.Add(item);
                }
                ManualDropDown.ClearOptions();
                ManualDropDown.AddOptions(TempList);                
                //Deparent here
                break;


            default:
                break;
        }
    }
   
    public void SearchInputEditorExtentionShowHide()
    {        
        if (SearchInput.gameObject.activeSelf)
        {
            SearchInput.gameObject.SetActive(false);           
        }
        else
        if (!SearchInput.gameObject.activeSelf)
        {
            SearchInput.gameObject.SetActive(true);            
        }       
    }
    
    public void SearchInputPlacemntsEditorExtention()
    {       
        SearchTextInputPlacemnts();
    }

    public void SearchInputSizingEditorExtention()
    {
        switch (SearchTextInputAnchoringMode)
        {
            case SearchTextInput.AutoAnchorAndAdjustSearchTextInputSize:
                AutoAnchorAndAdjustSearchTextInputSizeBool = true;
                AutoAnchorWithManualAdjustSearchTextInputSizeBool = false;
                FullyManualAnchorAndAdjustSearchTextInputSizeBool = false;
                break;
            case SearchTextInput.AutoAnchorWithManualAdjustSearchTextInputSize:
                AutoAnchorAndAdjustSearchTextInputSizeBool = false;
                AutoAnchorWithManualAdjustSearchTextInputSizeBool = true;
                FullyManualAnchorAndAdjustSearchTextInputSizeBool = false;
                break;
            case SearchTextInput.FullyManualAnchorAndAdjustSearchTextInputSize:
                AutoAnchorAndAdjustSearchTextInputSizeBool = false;
                AutoAnchorWithManualAdjustSearchTextInputSizeBool = false;
                FullyManualAnchorAndAdjustSearchTextInputSizeBool = true;
                break;
            default:
                break;
        }
        if (OriginalDropDown != null && OriginalDropDownRectTransform != null)
        {
            SearchTextInputSize();
        }        
    }

    public void SearchButtonEditorExtentionShowHide()
    {
        if (UseSearchButton)
        {           
            if (SearchButton.gameObject.activeSelf)
            {
                SearchButton.gameObject.SetActive(false);
            }
            else
            if (!SearchButton.gameObject.activeSelf)
            {
                SearchButton.gameObject.SetActive(true);
            }
        }                  
    }  

    public void SearchButtonPlacemntsEditorExtention()
    {
        switch (ButtonAnchorPosition)
        {
            case ButtonPosition.AnhcorButtonLeft:
                AnhcorButtonLeftBool = true;
                AnhcorButtonRightBool = false;
                break;
            case ButtonPosition.AnhcorButtonRight:
                AnhcorButtonLeftBool = false;
                AnhcorButtonRightBool = true;
                break;
            default:
                break;
        }
        if (OriginalDropDown != null && OriginalDropDownRectTransform != null)
        {
            SearchButtonPlacemnts();
        }
        
    }

    public void SearchButtonSizingEditorExtention()
    {
        switch (SearchButtonDimetionsMode)
        {
            case ButtonAnchor.AutoAdjustButtonSize:
                AutoAdjustButtonSizeBool = true;
                ManualAdjustButtomSizeBool = false;
                FullyManualAnchorAndAdjustButtomSizeBool = false;
                break;

            case ButtonAnchor.AutoAnchorWithManualAdjustButtomSize:
                AutoAdjustButtonSizeBool = false;
                ManualAdjustButtomSizeBool = true;
                FullyManualAnchorAndAdjustButtomSizeBool = false;
                break;

            case ButtonAnchor.FullyManualAnchorAndAdjustButtomSize:
                AutoAdjustButtonSizeBool = false;
                ManualAdjustButtomSizeBool = false;
                FullyManualAnchorAndAdjustButtomSizeBool = true;
                break;
            default:
                break;
        }
        if (OriginalDropDown != null && OriginalDropDownRectTransform != null)
        {
            SearchButtonSize();
        }        
    }

    #endregion
}



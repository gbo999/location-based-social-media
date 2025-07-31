using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(SearchIntegrationWithTMP))]
[CanEditMultipleObjects]
public class SearchEditor : Editor
{
    SearchIntegrationWithTMP targetScript;
    SerializedProperty Mode;
    SerializedProperty OriginalDropDown;
    SerializedProperty OriginalDropDownRectTransform;
    SerializedProperty SlowDataEntry;

    SerializedProperty SearchBoxDisapearsIfListShowsOnlyOneResult;
    SerializedProperty SearchFeaturesForceShareSameParentAsDropDown;

    SerializedProperty Type;
    SerializedProperty CaseSensativeSearch;

    SerializedProperty CopyDropDownTextStyle;
    SerializedProperty CopyDropDownColoursAndAppearance;
    SerializedProperty CopyDropDownListTextFormating;
    SerializedProperty CopyDropDownListTextAlignmentAndSpacing;

    SerializedProperty AutoAnchorSearchTextInput;
    SerializedProperty AdjustSearchTextInputProximity;


    SerializedProperty SearchTextInputAnchoringMode;
    //SerializedProperty AutoAdjustSearchTextInputSize;
    //SerializedProperty ManualAdjustSearchTextInputSize;
    SerializedProperty SearchTextInputHight;
    SerializedProperty SearchTextInputWidth;
    SerializedProperty SearchTextInputSizeMultiplier;

    SerializedProperty UseSearchButton;
    SerializedProperty AutoAnchorSearchButton;
    SerializedProperty ButtonAnchorPosition;
    SerializedProperty AdjustButtonProximity;

    SerializedProperty SearchButtonDimetionsMode;
    SerializedProperty ButtonHight;
    SerializedProperty ButtonWidth;
    SerializedProperty ButtonSizeMultiplier;

    SerializedProperty ManuallyAssignCanvus;
    SerializedProperty ParentCanvus;
    SerializedProperty UseMyOwnSearchInputField;
    SerializedProperty SearchInput;
    SerializedProperty SearchInputRectTransform;
    SerializedProperty UseMyOwnButton;
    SerializedProperty SearchButton;
    SerializedProperty SearchButtonRectTransform;

    //SerializedProperty ManualDropDown;
    //SerializedProperty ModeSelectionEditorExtention;

    //Added 23.02.2021
    SerializedProperty AutoSelectIfOnlyOneResult;
    SerializedProperty UseInputFieldToShowSelectionResult;
    SerializedProperty DisableInputFieldOnFocusLoss;
    SerializedProperty EclipseDropdown;

    private void OnEnable()
    {
        targetScript = (SearchIntegrationWithTMP)target;
        Mode = serializedObject.FindProperty("Mode");
        OriginalDropDown = serializedObject.FindProperty("OriginalDropDown");
        OriginalDropDownRectTransform = serializedObject.FindProperty("OriginalDropDownRectTransform");
        SlowDataEntry = serializedObject.FindProperty("SlowDataEntry");

        SearchBoxDisapearsIfListShowsOnlyOneResult = serializedObject.FindProperty("SearchBoxDisapearsIfListShowsOnlyOneResult");
        SearchFeaturesForceShareSameParentAsDropDown = serializedObject.FindProperty("SearchFeaturesForceShareSameParentAsDropDown");

        Type = serializedObject.FindProperty("Type");
        CaseSensativeSearch = serializedObject.FindProperty("CaseSensativeSearch");

        CopyDropDownTextStyle = serializedObject.FindProperty("CopyDropDownTextStyle");
        CopyDropDownColoursAndAppearance = serializedObject.FindProperty("CopyDropDownColoursAndAppearance");
        CopyDropDownListTextFormating = serializedObject.FindProperty("CopyDropDownListTextFormating");
        CopyDropDownListTextAlignmentAndSpacing = serializedObject.FindProperty("CopyDropDownListTextAlignmentAndSpacing");

        AutoAnchorSearchTextInput = serializedObject.FindProperty("AutoAnchorSearchTextInput");
        AdjustSearchTextInputProximity = serializedObject.FindProperty("AdjustSearchTextInputProximity");

        SearchTextInputAnchoringMode = serializedObject.FindProperty("SearchTextInputAnchoringMode");
        //AutoAdjustSearchTextInputSize = serializedObject.FindProperty("AutoAdjustSearchTextInputSize");
        //ManualAdjustSearchTextInputSize = serializedObject.FindProperty("ManualAdjustSearchTextInputSize");
        SearchTextInputHight = serializedObject.FindProperty("SearchTextInputHight");
        SearchTextInputWidth = serializedObject.FindProperty("SearchTextInputWidth");
        SearchTextInputSizeMultiplier = serializedObject.FindProperty("SearchTextInputSizeMultiplier");

        UseSearchButton = serializedObject.FindProperty("UseSearchButton");
        AutoAnchorSearchButton = serializedObject.FindProperty("AutoAnchorSearchButton");
        ButtonAnchorPosition = serializedObject.FindProperty("ButtonAnchorPosition");
        AdjustButtonProximity = serializedObject.FindProperty("AdjustButtonProximity");

        SearchButtonDimetionsMode = serializedObject.FindProperty("SearchButtonDimetionsMode");
        ButtonHight = serializedObject.FindProperty("ButtonHight");
        ButtonWidth = serializedObject.FindProperty("ButtonWidth");
        ButtonSizeMultiplier = serializedObject.FindProperty("ButtonSizeMultiplier");

        ManuallyAssignCanvus = serializedObject.FindProperty("ManuallyAssignCanvus");
        ParentCanvus = serializedObject.FindProperty("ParentCanvus");
        UseMyOwnSearchInputField = serializedObject.FindProperty("UseMyOwnSearchInputField");
        SearchInput = serializedObject.FindProperty("SearchInput");
        SearchInputRectTransform = serializedObject.FindProperty("SearchInputRectTransform");
        UseMyOwnButton = serializedObject.FindProperty("UseMyOwnButton");
        SearchButton = serializedObject.FindProperty("SearchButton");
        SearchButtonRectTransform = serializedObject.FindProperty("SearchButtonRectTransform");

        //ManualDropDown = serializedObject.FindProperty("ManualDropDown");
        //ModeSelectionEditorExtention = serializedObject.FindProperty("ModeSelectionEditorExtention");

        //Added 23.02.2021

        AutoSelectIfOnlyOneResult = serializedObject.FindProperty("AutoSelectIfOnlyOneResult");
        UseInputFieldToShowSelectionResult = serializedObject.FindProperty("UseInputFieldToShowSelectionResult");
        DisableInputFieldOnFocusLoss = serializedObject.FindProperty("DisableInputFieldOnFocusLoss");
        EclipseDropdown = serializedObject.FindProperty("EclipseDropdown");
    }


    public override void OnInspectorGUI()
    {
        if (targetScript == null)
        {
            return;
        }



        //if (Application.isPlaying)
        //{
        //    return;
        //}

       // base.OnInspectorGUI();                                        //Enable this and Disable everything after this to remembr how it looked like before editing (Keep On for referencing) Turn off when I am done
        serializedObject.Update();                                      // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        EditorGUILayout.BeginVertical();
        EditorGUI.BeginChangeCheck();




        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(Mode);
        if (!Application.isPlaying)
        {
            if (Mode.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(OriginalDropDown);
                EditorGUILayout.PropertyField(OriginalDropDownRectTransform);
            }
            else
            if (Mode.enumValueIndex == 1)
            {

                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(SlowDataEntry, true);
                EditorGUI.indentLevel = 0;

            }
            else
            if (Mode.enumValueIndex == 2)
            {
                //Add Instructions here(add Link to youtube video)
            }

            //Debug.Log("Mode.enumValueIndex   " + Mode.enumValueIndex);
            targetScript.ModeSelectionEditorExtention();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(SearchBoxDisapearsIfListShowsOnlyOneResult);
        EditorGUILayout.PropertyField(AutoSelectIfOnlyOneResult);
        EditorGUILayout.PropertyField(UseInputFieldToShowSelectionResult);
        if (UseInputFieldToShowSelectionResult.boolValue == false)
        {
            EditorGUILayout.PropertyField(DisableInputFieldOnFocusLoss);
        }       
        else
        {
            DisableInputFieldOnFocusLoss.boolValue = false;
        }
        EditorGUILayout.PropertyField(SearchFeaturesForceShareSameParentAsDropDown);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(Type);
        EditorGUILayout.PropertyField(CaseSensativeSearch);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(CopyDropDownTextStyle);
        EditorGUILayout.PropertyField(CopyDropDownColoursAndAppearance);
        EditorGUILayout.PropertyField(CopyDropDownListTextFormating);
        EditorGUILayout.PropertyField(CopyDropDownListTextAlignmentAndSpacing);
        EditorGUILayout.EndVertical();



        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.BeginHorizontal();
        if (UseInputFieldToShowSelectionResult.boolValue == true)
        {
            EditorGUILayout.PropertyField(EclipseDropdown);
        }
        else
        {
            GUI.enabled = false;
            EclipseDropdown.boolValue = false;
            EditorGUILayout.PropertyField(EclipseDropdown);
            GUI.enabled = true;
        }

        if (!Application.isPlaying)
        {

            if (GUILayout.Button("Show/Hide"))
            {
                if (!Application.isPlaying)
                {
                    targetScript.SearchInputEditorExtentionShowHide();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        if (!Application.isPlaying)
        {
            if (EclipseDropdown.boolValue == true)
            {
                AutoAnchorSearchTextInput.boolValue = false;
            }
            else
            {
                EditorGUILayout.PropertyField(AutoAnchorSearchTextInput);
            }
        }

        
        if (!Application.isPlaying)
        {    
            if (AutoAnchorSearchTextInput.boolValue)
            {
                EditorGUILayout.PropertyField(AdjustSearchTextInputProximity);

                targetScript.SearchInputPlacemntsEditorExtention();
            }
        }
        EditorGUILayout.EndVertical();





        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);       
        EditorGUILayout.PropertyField(SearchTextInputAnchoringMode);
        if (!Application.isPlaying)
        {        
            if (SearchTextInputAnchoringMode.enumValueIndex == 0)
            {
                //Its Automated, No Options required
                targetScript.SearchInputSizingEditorExtention();
            }
            else
            if (SearchTextInputAnchoringMode.enumValueIndex == 1)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(SearchTextInputHight);
                EditorGUILayout.PropertyField(SearchTextInputWidth);
                EditorGUILayout.PropertyField(SearchTextInputSizeMultiplier);
                EditorGUI.indentLevel = 0;
                targetScript.SearchInputSizingEditorExtention();
            }
            else
            if (SearchTextInputAnchoringMode.enumValueIndex == 2)
            {
                //Add Instructions here (add Link to youtube video)
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();



        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(UseSearchButton);
        if (!Application.isPlaying)
        {

            if (GUILayout.Button("Show/Hide"))
            {
                if (!Application.isPlaying)
                {
                    targetScript.SearchButtonEditorExtentionShowHide();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
        if (!Application.isPlaying)
        {
            if (UseSearchButton.boolValue)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(AutoAnchorSearchButton);
                EditorGUI.indentLevel = 0;
                targetScript.SearchButtonPlacemntsEditorExtention();
                if (AutoAnchorSearchButton.boolValue)
                {
                    EditorGUI.indentLevel = 4;
                    EditorGUILayout.PropertyField(ButtonAnchorPosition);
                    EditorGUILayout.PropertyField(AdjustButtonProximity);
                    EditorGUI.indentLevel = 0;
                    targetScript.SearchButtonPlacemntsEditorExtention();
                }
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(SearchButtonDimetionsMode);
        if (!Application.isPlaying)
        {
            if (SearchButtonDimetionsMode.enumValueIndex == 0)
            {
                //Its Automated, No Options required
                targetScript.SearchButtonSizingEditorExtention();
            }
            else
            if (SearchButtonDimetionsMode.enumValueIndex == 1)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(ButtonHight);
                EditorGUILayout.PropertyField(ButtonWidth);
                EditorGUILayout.PropertyField(ButtonSizeMultiplier);
                EditorGUI.indentLevel = 0;
                targetScript.SearchButtonSizingEditorExtention();
            }
            else
            if (SearchButtonDimetionsMode.enumValueIndex == 2)
            {
                targetScript.SearchButtonSizingEditorExtention();
                //Add Instructions here (add Link to youtube video)
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(ManuallyAssignCanvus);
        if (ManuallyAssignCanvus.boolValue)
        {
            EditorGUILayout.PropertyField(ParentCanvus);
        }

        EditorGUILayout.PropertyField(UseMyOwnSearchInputField);
        if (UseMyOwnSearchInputField.boolValue)
        {
            EditorGUILayout.PropertyField(SearchInput);
            EditorGUILayout.PropertyField(SearchInputRectTransform);
        }

        EditorGUILayout.PropertyField(UseMyOwnButton);
        if (UseMyOwnButton.boolValue)
        {
            EditorGUILayout.PropertyField(SearchButton);
            EditorGUILayout.PropertyField(SearchButtonRectTransform);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();


        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties(); // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            targetScript.SearchInputPlacemntsEditorExtention();  //I added this here, 24.02.2021, not sure how I made it responsive before
            //targetScript.EditorRefreshAllFunctions();


            //EditorWindow view = EditorWindow.GetWindow<SceneView>();  //I think I needed these for Older versions of Unity, not sure but they seem to get in the way with 2020.1.7 version
            //view.Repaint();                                           //I think I needed these for Older versions of Unity, not sure but they seem to get in the way with 2020.1.7 version
        }


    }

}











//if(GUILayout.Button("Apply All Changes"))
//{
//    Debug.Log("its pressed");            
//}





//if (GUI.changed)
//{         

//    Debug.Log("GUI.CHANGED!!");
//    EditorSceneManager.MarkAllScenesDirty();
//    EditorApplication.DirtyHierarchyWindowSorting();
//    EditorApplication.RepaintHierarchyWindow();
//    EditorUtility.SetDirty(targetScript);
//    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
//    SceneView.RepaintAll();
//    EditorApplication.RepaintProjectWindow();

//}

//if (EditorGUI.EndChangeCheck())
//{
//    //Undo.RecordObject(targetScript, "Do I write Anything here??");
//    Debug.Log("GUI.CHANGED!!");
//}
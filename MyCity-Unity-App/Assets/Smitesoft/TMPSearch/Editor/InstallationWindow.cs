using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Compilation;
using System.IO;
using System;
using Smitesoft.TMPSearch.Editor;

namespace TMPExtension
{
    public class InstallationWindow : EditorWindow
    {
        [MenuItem("Tools/Smitesoft/TMP-Integration")]
        public static void showWindow()
        {
            GetWindow<InstallationWindow>("Smitesoft");
        }


        public static string globalRuntimeDirs; //weird that I can have this inside and outside a function
        public static string globalEditorDirs;  //weird that I can have this inside and outside a function

        private bool Compatible;
        private bool unityCompatible;
        private bool TMProCompatible;

        private static bool Debugging;

        private bool SetupEssentialsCompleted;
        private bool InstallPackagesCompleted;
        private bool recompilationCompleted;
        private string Status = "Not - Installed";

        private bool valveA = false;
        //private bool valveB = false;



        private bool didWeAddBlocker = false;
        private string tagToAdd = "Blocker";

        private bool haveWeInstalledLocally;
        private bool haveWeInstalledGlobally;

        private static bool haveWeUnPackedManagerPrefab;
        private static bool haveWeUnpackedTextMeshProEssencials;

        private string TextMeshProEssentialsDir = "Packages/com.unity.textmeshpro/Package Resources/TMP Essential Resources.unitypackage";
        
        private string managerPrefabPlusDummy = "Assets/Smitesoft/TMPSearch/Editor/SmitesoftInstallationPackages/IgnoreThisFile/DontClickA.unitypackage";
        private string managetPrefabMinusDummy = "Assets/Smitesoft/TMPSearch/Editor/SmitesoftInstallationPackages/IgnoreThisFile/DontClickB.unitypackage";

        string[] TMProCompatibleVersionsArray = { "2.1.0", "2.1.1", "2.1.3","2.1.4", "3.0.1", "3.0.3", "3.0.4","3.0.6" };

        Color newRedColorForDarkTheme;
        Color newGreenColorForLightTheme;
        Color newYellowColorForLightTheme;
        Color newRedColorForLightTheme;


        //private void OnValidate()
        //{
        //    showWindow();
        //    Debug.Log("OnValidate");
        //}

        //private void OnProjectChange()
        //{
        //    showWindow();
        //    Debug.Log("OnProjectChange");
        //}

        private void OnEnable()
        {
            showWindow(); //I want this window to show as soon as the asset gets downloaded (This isnt doing it)



            onEnableButTriggerAble();

            //https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Colors/Color_picker_tool
            // 240, 65, 65, 0.7
            newRedColorForDarkTheme = new Color(243f / 255f, 16f / 255f, 16f / 255f, 1.0f);  //Seems impossible to Change colour in light theme .. I guess I can change background instead (This is red btw) Whats intresting is that we can change the opacity
            //(191, 63, 63, 0.7)
            newRedColorForLightTheme = new Color(255f / 255f, 0f / 255f, 0f / 255f, 0.3f);

            newGreenColorForLightTheme = new Color(0f / 255f, 255f / 255f, 0f / 255f, 0.3f);

            newYellowColorForLightTheme = new Color(255f / 255f, 213f / 255f, 5f / 255f, 0.3f);
        }

        //static void Update()  //This one is harsh, and it updates very frequently (probabaly on every frame for all windows)
        //{
        //    Debug.Log("Updating");
        //}
        void OnInspectorUpdate() //This one is much slower and runs maybe 10x per second
        {
            // Call Repaint on OnInspectorUpdate as it repaints the windows
            // less times as if it was OnGUI/Update
            //Repaint();
            UpdateInstallerUI();
        }



        private void UpdateInstallerUI()
        {
            if (!haveWeUnPackedManagerPrefab) //small window
            {
                if (!Debugging)
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(400, 400);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(600, 400);
                }
                else
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(400, 550);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(600, 550);
                }
            }
            else //bigwindow
            {
                if (!Debugging)
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(400, 600);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(600, 600);
                }
                else
                {
                    GetWindow<InstallationWindow>().minSize = new Vector2(400, 750);
                    GetWindow<InstallationWindow>().maxSize = new Vector2(600, 750);
                }
            }
        }

        private void onEnableButTriggerAble()
        {
            haveWeUnpackedTextMeshProEssencials = CheckingTextoMeshProFile();
            haveWeUnPackedManagerPrefab = CheckingManagerPrefab();

            haveWeInstalledLocally = checkingDllFiles();  //if we get false that means we do not have it locally, but we could still have it globally

            if (searchAbleGlobal.Count > 0)
            {
                haveWeInstalledGlobally = true;
            }
            else
            {
                haveWeInstalledGlobally = false;
            }

            //Must do a blocker check here
            didWeAddBlocker = TMPExtensionUtility.CheckingTag(tagToAdd);

            HaveWeAllreadyInstalled(didWeAddBlocker, haveWeInstalledLocally);

            haveWeCheckUnityVersion = false;        //This will allow rechacking after Unity reboot
            haveWeCheckTMPVersion = false;          //This will allow rechacking after Unity reboot     
        }


        public void OnFocus()  //temp for now
        {
            haveWeUnpackedTextMeshProEssencials = CheckingTextoMeshProFile();
            haveWeUnPackedManagerPrefab = CheckingManagerPrefab();

            //Debug.Log("On Focus");

            VersionCheckTMP();
            CompatibilityCheckTMPro();

            UnityVersionCheck();
            CompatibilityCheckUnity();
        }


        public void HaveWeAllreadyInstalled(bool didWeAddBlocker, bool installed)
        {
            //string path = GetInstallPath("Editor/TMPExtensionInstall.cs");

            if (didWeAddBlocker)
            {
                SetupEssentialsCompleted = true;
                InstallationStatus();
            }
            if (installed)
            {

                InstallPackagesCompleted = true;
                recompilationCompleted = true;
                valveA = true;
                //valveB = true;
                InstallationStatus();
            }
        }



        #region UnityVersionCheck .. THis is Fluff, just for visual

        private bool haveWeCheckUnityVersion;
        private string UnityVersion = "Checking";
        private string UnityCompatability = "Checking";
        private void UnityVersionCheck()
        {
            if (haveWeCheckUnityVersion)
            {
                return;
            }

            UnityVersion = Application.unityVersion;  //I need this for alignmnt? naa I can just use the TMP version 
                                                      // I will just use the as another check for TMP compatability

            haveWeCheckUnityVersion = true;
        }

        #endregion

#if UNITY_2019_1_OR_NEWER
        private void CompatibilityCheckUnity()
        {
            unityCompatible = true;
            UnityCompatability = "Compatible";
        }
#else
    private void CompatibilityCheckUnity()
    {
        unityCompatible = false;
        UnityCompatability = "Not-Compatible";
    }
#endif

        #region TMProVersionCheck  && Compatibility Check

        private bool haveWeCheckTMPVersion;
        private string TMProVersion = "Checking";
        private string TMProCompatability = "Checking";



        private void VersionCheckTMP()
        {
            if (haveWeCheckTMPVersion)
            {
                return;
            }
#if UNITY_2019_1_OR_NEWER

            /*TMProVersion = TMPExtensionUtility.TMProVersionLaterThan2019();
            haveWeCheckTMPVersion = true;*/
#else
            TMProVersion = TMPExtensionUtility.TMProVersionSub2019();            
            haveWeCheckTMPVersion = true;
#endif
        }

        private void CompatibilityCheckTMPro()
        {
            foreach (var version in TMProCompatibleVersionsArray)
            {
                if (version == TMProVersion)
                {
                    TMProCompatible = true;
                    break;
                }
                else
                {
                    TMProCompatible = false;
                }
            }

            if (TMProCompatible)
            {
                TMProCompatability = "Compatible";
            }
            else
            {
                TMProCompatability = "Not-Compatible";
            }
        }
        #endregion

        int successInstallNumber = 0;
        int installNumberFailed = 0;
        private bool InstallSucess;

        private void InstallAll()
        {
            InstallSucess = TMPExtensionUtility.InstallDropdownMain();  //if we click, we will run the function from the Utility script and close the MenuItem at the same time due to the next function!

            SuccessSum(InstallSucess);

            InstallSucess = TMPExtensionUtility.InstallDropdownMeta();

            SuccessSum(InstallSucess);

            InstallSucess = TMPExtensionUtility.InstallInputFieldMain();

            SuccessSum(InstallSucess);

            InstallSucess = TMPExtensionUtility.InstallInputFieldMeta();

            SuccessSum(InstallSucess);

            InstallSucess = TMPExtensionUtility.InstallDropdownEditorMain();  //if we click, we will run the function from the Utility script and close the MenuItem at the same time due to the next function!

            SuccessSum(InstallSucess);

            InstallSucess = TMPExtensionUtility.InstallDropdownEditorMeta();

            SuccessSum(InstallSucess);

            InstallSucess = TMPExtensionUtility.InstallInputFieldEditorMain();

            SuccessSum(InstallSucess);

            InstallSucess = TMPExtensionUtility.InstallInputFieldEditorMeta();

            SuccessSum(InstallSucess);
        }

        private void SuccessSum(bool Successful)
        {
            installNumberFailed += 1;
            if (Successful)
            {
                successInstallNumber += 1;
            }
            else
            {
                Debug.Log("install: " + installNumberFailed + ",has Failed contact Smitesoft for help on discord");
            }

            Debug.Log("Total Successful Installs: " + successInstallNumber.ToString() + "/8");
        }


        private string InstallationStatus()
        {
            if (InstallPackagesCompleted && recompilationCompleted && SetupEssentialsCompleted)
            {
                Status = "Completed!";
            }
            else
            if (SetupEssentialsCompleted)
            {
                Status = "Pending Installation";
            }
            return Status;
        }

        private void OnGUI()
        {
            //if (GUILayout.Button("Testing"))
            //{
            //    //EditorScriptSO.Start();
            //    //GetWindow<EditorWindow>().Close();
            //    OpenFullReference();
            //}

            if (haveWeUnPackedManagerPrefab)
            {
                EditorGUILayout.Space();
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    TMPExtensionUtility.ShowBanner(EditorGUIUtility.LoadRequired("Assets/Smitesoft/Editor Default Resources/Editor banner Dark Theme.png") as Texture);
                }
                else
                {
                    TMPExtensionUtility.ShowBanner(EditorGUIUtility.LoadRequired("Assets/Smitesoft/Editor Default Resources/Editor banner Light Theme.png") as Texture);
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space();  //Dont use EditorGUILayout.Space(60) or similar spacing here, use multiple spaces not to effect lines verical spacing. this only applies here due to overlap
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUILayout.Label("TMP-Searchable : Integration Installer", EditorStyles.boldLabel);
                }
                else
                {
                    GUILayout.Label("TMP-Searchable : Integration Installer", EditorStyles.boldLabel);
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(20); //Not sure why I had to add this, this was working perfeclty, and all of a sudden I had to addt his... weird needs further investigation
            }
            else
            {
                EditorGUILayout.Space();
                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    TMPExtensionUtility.ShowBanner(EditorGUIUtility.LoadRequired("Assets/Smitesoft/Editor Default Resources/Editor banner Dark Theme.png") as Texture);
                }
                else
                {
                    TMPExtensionUtility.ShowBanner(EditorGUIUtility.LoadRequired("Assets/Smitesoft/Editor Default Resources/Editor banner Light Theme.png") as Texture);
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space();  //Dont use EditorGUILayout.Space(60) or similar spacing here, use multiple spaces not to effect lines verical spacing. this only applies here due to overlap
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (TMPExtensionUtility.UnitySkinIsDark)
                {
                    GUILayout.Label("TMP-Searchable : Preparing Dependencies", EditorStyles.boldLabel);
                }
                else
                {
                    GUILayout.Label("TMP-Searchable : Preparing Dependencies", EditorStyles.boldLabel);
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(20); //Not sure why I had to add this, this was working perfeclty, and all of a sudden I had to addt his... weird needs further investigation
            }



            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (InstallationStatus() == "Completed!")
            {
                if (GUILayout.Button("Updating Video-Guide"))
                {
                    Application.OpenURL("https://www.youtube.com/watch?v=KR7Ntee0qQM");
                }

                GUI.enabled = false;
                if (GUILayout.Button("Setup Video-Guide"))
                {
                    Application.OpenURL("https://www.youtube.com/watch?v=Sn4z-Knxdu4");
                }

                if (GUILayout.Button("Setup Documentation"))
                {
                    OpenFullReference();
                }
                GUI.enabled = true;
            }
            else
            {
                if (GUILayout.Button("Setup Video-Guide"))
                {
                    Application.OpenURL("https://www.youtube.com/watch?v=Sn4z-Knxdu4");
                }

                if (GUILayout.Button("Setup Documentation"))
                {
                    OpenFullReference();
                }
            }



            EditorGUILayout.EndVertical();

            if (haveWeUnPackedManagerPrefab)  // This should be, if we have a Manager folder or even a Note thats comes with it
            {

                EditorGUILayout.Space();
                GUILayout.Label("Unity Version", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(UnityVersion, EditorStyles.textArea);
                if (UnityCompatability == "Compatible")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                GUILayout.Label("TMPro Version", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(TMProVersion, EditorStyles.textArea);
                if (TMProCompatability == "Compatible")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                EditorGUILayout.EndHorizontal();



                GUILayout.Space(15);

                if (TMProCompatible && unityCompatible)  // Unity version check and TMP version Check // or if Installation completed
                {
                    Compatible = true;
                }
                else
                {
                    Compatible = false;
                }

                if (Compatible == true)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    if (!SetupEssentialsCompleted)
                    {
                        if (GUILayout.Button("Setup-Essential"))
                        {
                            didWeAddBlocker = TMPExtensionUtility.CreateTag(tagToAdd);

                            SetupEssentialsCompleted = true;
                            GetWindow<EditorWindow>().Close();
                            showWindow();
                            InstallationStatus();
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Setup-Essential"))
                        {
                        }
                        GUI.enabled = true;
                    }


                    if (SetupEssentialsCompleted && !valveA)
                    {
                        if (GUILayout.Button("Install Packages"))
                        {
                            Debug.Log("Installing TMP Extension");

                            //    TempData.InstalationStage += 1;                            
                            EditorScriptSO.RunInstallStage();


                            InstallAll();

                            InstallPackagesCompleted = true;
                            InstallationStatus();

                            AssetDatabase.Refresh();

                            recompilationCompleted = true;
                            InstallationStatus();

                            Debug.Log("TMP-Integration:  Complete");
                            EditorScriptSO.SaveInstalationStatus(); //Increase +1 to make it +2 on the databse
                            valveA = true; //Just so they dont install it again
                        }
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Install Packages"))
                        {
                        }
                        GUI.enabled = true;
                    }

                    EditorGUILayout.EndVertical();
                }
                else //Show but Greyed out
                {
                    GUI.enabled = false;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (GUILayout.Button("Setup-Essential"))
                    {
                    }

                    if (GUILayout.Button("Install Packages"))
                    {
                    }

                    if (GUILayout.Button("Apply Changes"))
                    {
                    }
                    EditorGUILayout.EndVertical();
                    GUI.enabled = true;
                }


                EditorGUILayout.Space();
                GUILayout.Label("Status", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Installation", EditorStyles.textArea);
                if (Status == "Completed!")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                        GUI.backgroundColor = Color.white;
                    }

                }
                else
                if (Status == "Not - Installed")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                        GUI.backgroundColor = Color.white;
                    }

                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.yellow;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newYellowColorForLightTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                        GUI.backgroundColor = Color.white;
                    }

                }

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (InstallationStatus() == "Completed!")
                {
                    if (GUILayout.Button("User Manual"))
                    {
                        OpenFullReference();
                    }

                    if (GUILayout.Button("User Video Manual "))
                    {
                        Application.OpenURL("https://youtu.be/hp3cSSxaYqc");
                    }
                }
                else
                {
                    GUI.enabled = false;
                    if (GUILayout.Button("User Manual"))
                    {
                        OpenFullReference();
                    }

                    if (GUILayout.Button("User Video Manual "))
                    {
                        Application.OpenURL("https://youtu.be/hp3cSSxaYqc");
                    }
                    GUI.enabled = true;
                }


                EditorGUILayout.EndVertical();



                EditorGUILayout.Space();
                GUILayout.Label("Contact us", EditorStyles.label);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (GUILayout.Button("Discord"))
                {
                    Application.OpenURL("https://discord.gg/d39KwkkWn3");
                }

                if (GUILayout.Button("Forums"))
                {
                    Application.OpenURL("https://smitesoft.com/community");
                }
                EditorGUILayout.EndVertical();


                GUILayout.Space(15);
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Special Thanks:", EditorStyles.boldLabel);
                GUILayout.Label("Developer Page:", EditorStyles.boldLabel, GUILayout.MaxWidth(200));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();


                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("SteveSmith", EditorStyles.label);
                GUI.enabled = false;
                if (GUILayout.Button("Anonymous", GUILayout.MaxWidth(200)))
                {

                }

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Invertex", EditorStyles.label);
                if (GUILayout.Button("Portfolio", GUILayout.MaxWidth(200)))
                {
                    Application.OpenURL("https://forum.unity.com/members/invertex.458918/");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Exanite", EditorStyles.label);
                if (GUILayout.Button("Portfolio", GUILayout.MaxWidth(200)))
                {
                    Application.OpenURL("https://github.com/Exanite");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Austin Rife", EditorStyles.label);
                if (GUILayout.Button("Portfolio", GUILayout.MaxWidth(200)))
                {
                    Application.OpenURL("https://techgeek1.github.io/about");
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                Debugging = GUILayout.Toggle(Debugging, " Debugging Mode");

                if (Debugging)
                {
                    EditorGUILayout.Space();
                    GUI.enabled = false;
                    GUILayout.TextArea("Note: Debugging mode will activate all Logs, It is advised to Turn this mode off, important Logs will remain active regardless", 300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(50));
                    GUI.enabled = true;
                    EditorGUILayout.Space();


                    GUI.enabled = false;
                    GUI.contentColor = Color.yellow;
                    GUILayout.TextArea("Warrning!!, Only Delete Cache in a fresh project that does not have this asset fully Installed," +
                                         " This asset cannot function without its Cache and it will attempt to reinstall all the cache with " +
                                         "a fresh installation of this asset. ", 300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(70));
                    GUI.contentColor = Color.white;
                    GUI.enabled = true;

                    EditorGUILayout.Space();
                    GUI.contentColor = Color.yellow;
                    if (InstallationStatus() == "Completed!")
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            onEnableButTriggerAble();
                        }
                        GUI.enabled = true;
                    }
                    else
                    {
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            onEnableButTriggerAble();
                        }
                    }

                    GUI.contentColor = Color.white;
                }

            }
            else //This applies if they have the files globbaly but not locally, so we need to purge them
            {
                



                EditorGUILayout.Space();
                GUILayout.Label("Unity Version", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(UnityVersion, EditorStyles.textArea);
                if (UnityCompatability == "Compatible")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(UnityCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
                GUILayout.Label("TMPro Version", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(TMProVersion, EditorStyles.textArea);

                if (TMProCompatability == "Compatible")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(TMProCompatability, EditorStyles.textArea, GUILayout.MaxWidth(200));
                        GUI.backgroundColor = Color.white;
                    }
                }


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                if (TMProCompatible && unityCompatible)  // Unity version check and TMP version Check // or if Installation completed
                {
                    Compatible = true;
                }
                else
                {
                    Compatible = false;
                }
                if (Compatible)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    if (haveWeUnpackedTextMeshProEssencials)
                    {
                        if (!haveWeInstalledLocally && !haveWeInstalledGlobally) //&& TMP_Pro Essnetials Exists (we really only need to check for TMP folder)... I menan who downloads extras without the essensials..
                        {
                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack TMPro Essentials"))
                            {
                            }
                            GUI.enabled = true;

                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                                //         TempData.InstalationStage += 1;
                                EditorScriptSO.RunInstallStage();
                                GetWindow<EditorWindow>().Close();
                                EditorUtility.OpenWithDefaultApp(managerPrefabPlusDummy);

                            }
                        }
                        else
                        {
                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack TMPro Essentials"))
                            {
                            }
                            GUI.enabled = true;

                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                                //           TempData.InstalationStage += 1;
                                EditorScriptSO.RunInstallStage();
                                GetWindow<EditorWindow>().Close();
                                EditorUtility.OpenWithDefaultApp(managetPrefabMinusDummy);
                            }
                        }
                    }
                    else
                    {
                        if (!haveWeInstalledLocally && !haveWeInstalledGlobally) //&& TMP_Pro Essnetials Exists (we really only need to check for TMP folder)... I menan who downloads extras without the essensials..
                        {
                            if (GUILayout.Button("Unpack TMPro Essentials"))
                            {
                                if (File.Exists(TextMeshProEssentialsDir)) //this is checking the local dir dll (which you can only see from inside unity) not from windows, so weird
                                {
                                    //                  TempData.InstalationStage += 1;
                                    EditorScriptSO.RunInstallStage();
                                    GetWindow<EditorWindow>().Close();
                                    UnPackTextMeshProEssentials(); //This Opens it from the Global dir dll, coz we cant even open it from the local one.. coz its not there! even though we can see it in unity and check for its exisistance                      
                                }
                                else
                                {
                                    Debug.Log("Could Not find TMP_Essentials to install it, Please install it manually to continue." +
                                        " You can find it under Windows tab => TextMeshPro => Import TMP Essential Resources");
                                }

                            }
                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                            }
                            GUI.enabled = true;
                        }
                        else
                        {
                            if (GUILayout.Button("Unpack TMPro Essentials"))
                            {
                                if (File.Exists(TextMeshProEssentialsDir)) //this is checking the local dir dll (which you can only see from inside unity) not from windows, so weird
                                {
                                    //              TempData.InstalationStage += 1;
                                    EditorScriptSO.RunInstallStage();
                                    GetWindow<EditorWindow>().Close();
                                    UnPackTextMeshProEssentials(); //This Opens it from the Global dir dll, coz we cant even open it from the local one.. coz its not there! even though we can see it in unity and check for its exisistance                                
                                }
                                else
                                {
                                    Debug.Log("Could Not find TMP_Essentials to install it, Please install it manually to continue." +
                                        " You can find it under Windows tab => TextMeshPro => Import TMP Essential Resources");
                                }
                            }
                            GUI.enabled = false;
                            if (GUILayout.Button("Unpack Smitesoft Essentials"))
                            {
                            }
                            GUI.enabled = true;
                        }
                    }



                    if (GUILayout.Button("Apply Changes"))
                    {
                        AssetDatabase.Refresh();
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {                    
                    GUI.contentColor = Color.yellow;
                    GUILayout.TextArea("Incompatible builds: Make sure your using Unity versions 2019 onwards. Make sure your TMP version is compatible," +
                        " you can update you TMP version in Package-Manager. Compatible TMP versions Include:  (2.1.0, 2.1.1, 2.1.3, 3.0.1, 3.0.3). Back-Up project before updating TMP!"
                        , 300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(70));
                    GUI.contentColor = Color.white;
                }

                



                EditorGUILayout.Space();
                GUILayout.Label("Status", EditorStyles.label);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Installation", EditorStyles.textArea);
                if (Status == "Completed!")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.green;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newGreenColorForLightTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //green
                        GUI.backgroundColor = Color.white;
                    }

                }
                else
                if (Status == "Not - Installed")
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = newRedColorForDarkTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newRedColorForLightTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //red
                        GUI.backgroundColor = Color.white;
                    }

                }
                else
                {
                    if (TMPExtensionUtility.UnitySkinIsDark)
                    {
                        GUI.contentColor = Color.yellow;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = newYellowColorForLightTheme;
                        GUILayout.Label(Status, EditorStyles.textArea, GUILayout.MaxWidth(200)); //yellow
                        GUI.backgroundColor = Color.white;
                    }

                }

                EditorGUILayout.EndHorizontal();


                GUILayout.Space(20);


                Debugging = GUILayout.Toggle(Debugging, " Debugging Mode");

                if (Debugging)
                {
                    EditorGUILayout.Space();
                    GUI.enabled = false;
                    GUILayout.TextArea("Note: Debugging mode will activate all Logs, It is advised to Turn this mode off, Impotants Logs will remain activ regardless", 300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(50));
                    GUI.enabled = true;
                    EditorGUILayout.Space();


                    GUI.enabled = false;
                    GUI.contentColor = Color.yellow;
                    GUILayout.TextArea("Warrning!!, Only Delete Cache in a fresh project that does not have this asset fully Installed," +
                                         " This asset cannot function without its Cache and it will attempt to reinstall all the cache with " +
                                         "a fresh installation of this asset", 300, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(70));
                    GUI.contentColor = Color.white;
                    GUI.enabled = true;

                    EditorGUILayout.Space();

                    GUI.contentColor = Color.yellow;
                    if (InstallationStatus() == "Completed!")
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            onEnableButTriggerAble();
                        }
                        GUI.enabled = true;
                    }
                    else
                    {
                        if (GUILayout.Button("Delete Cache"))
                        {
                            TMPExtensionUtility.DeleteGlobalDllFiles();
                            TMPExtensionUtility.DeleteLocalDllFiles();
                            onEnableButTriggerAble();
                        }
                    }
                    GUI.contentColor = Color.white;
                }
            }
        }

        private void UnPackTextMeshProEssentials()
        {
            string globalDirsFirstFraction = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity/cache/packages/packages.unity.com");   // This uses using System (not the IO)
                                                                                                                                                                                   //Debug.Log(globalDirsFirstFraction);//

            string globalDirsCombinedFractions = Path.Combine(globalDirsFirstFraction, "com.unity.textmeshpro" + "@" + TMProVersion);

            string packageresources = Path.Combine(globalDirsCombinedFractions, "Package Resources");

            string[] globalTmpEssentialResourcesFile = Directory.GetFiles(packageresources, "TMP Essential Resources.unitypackage", SearchOption.AllDirectories);

            EditorUtility.OpenWithDefaultApp(globalTmpEssentialResourcesFile[0]);
        }

        private void OpenFullReference() //this is the way to do it, was still able to find documentation even after I moved the file
        {
            string[] dirDocumentationFolder = Directory.GetDirectories("Assets/Smitesoft", "Documentation", SearchOption.AllDirectories);
            string[] dirDocumentationFiles = Directory.GetFiles(dirDocumentationFolder[0], "Full Documentation*", SearchOption.AllDirectories);

            if (dirDocumentationFiles.Length > 0)
            {
                EditorUtility.OpenWithDefaultApp(dirDocumentationFiles[0]);
            }
        }

        private bool CheckingManagerPrefab()
        {
            string[] dirManagerFolder = Directory.GetDirectories("Assets/Smitesoft/TMPSearch", "Prefabs", SearchOption.AllDirectories);
            string[] dirManagerFiles = Directory.GetFiles(dirManagerFolder[0], "TMP_Searchable_Manager*", SearchOption.AllDirectories);

            if (dirManagerFiles.Length > 0)
            {
                return true;
            }
            return false;

        }


        private bool CheckingTextoMeshProFile()
        {
            string[] dirTextMeshPro = Directory.GetDirectories("Assets", "TextMesh Pro");

            if (Debugging)
            {
                if (dirTextMeshPro.Length > 0)
                {
                    Debug.Log(dirTextMeshPro[0] + "is found");
                }
                else
                {
                    Debug.Log("TMPro Essencials Not found");
                }
            }

            if (dirTextMeshPro.Length > 0)
            {
                return true;
            }
            return false;
        }






        public static List<string> searchAbleLocal = new List<string>();
        public static List<string> searchAbleGlobal = new List<string>();

        private bool checkingDllFiles()
        {
            searchAbleLocal.Clear();
            searchAbleGlobal.Clear();


            //Finding TMP Locally==Start
            string[] localDirs = Directory.GetDirectories("Library/PackageCache", "com.unity.textmeshpro*", SearchOption.AllDirectories);
            //Finding TMP Locally==End

            //Local Runtime Directory
            string localRuntimeDirs = Path.Combine(localDirs[0], "Scripts/Runtime");

            string[] localRuntimeFiles = Directory.GetFiles(localRuntimeDirs, "TMP_*", SearchOption.AllDirectories);

            foreach (var file in localRuntimeFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleLocal.Add(file);
                }
            }

            //Local Editor Directory
            string localEditorDirs = Path.Combine(localDirs[0], "Scripts/Editor");

            string[] localEditorFiles = Directory.GetFiles(localEditorDirs, "TMP_*", SearchOption.AllDirectories);


            foreach (var file in localEditorFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleLocal.Add(file);
                }
            }





            //Finding TMP Globally==Start
            string globalDirsFirstFraction = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity/cache/packages/packages.unity.com"); // This uses using System (not the IO)

            string globalDirsCombinedFractions = Path.Combine(globalDirsFirstFraction, "com.unity.textmeshpro" + "@" + TMProVersion);
            //Finding TMP Globally==End


            //Global Runtime Directory
            globalRuntimeDirs = Path.Combine(globalDirsCombinedFractions, "Scripts/Runtime");

            string[] globalRuntimeFiles = Directory.GetFiles(globalRuntimeDirs, "TMP_*", SearchOption.AllDirectories);

            foreach (var file in globalRuntimeFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleGlobal.Add(file);
                }
            }

            //Global Editor Directory
            globalEditorDirs = Path.Combine(globalDirsCombinedFractions, "Scripts/Editor");

            string[] globalEditorFiles = Directory.GetFiles(globalEditorDirs, "TMP_*", SearchOption.AllDirectories);

            foreach (var file in globalEditorFiles)
            {
                if (file.Contains("Searchable"))
                {
                    searchAbleGlobal.Add(file);
                }
            }


            if (Debugging)
            {
                Debug.Log("TMP_Searchable Local Cashe file count = " + searchAbleLocal.Count);
            }

            foreach (var item in searchAbleLocal) //This is where I check if they exist and delete them, and then force reload/refresh
            {
                if (Debugging)
                {
                    Debug.Log(item);
                }
            }


            if (Debugging)
            {
                Debug.Log("TMP_Searchable Global Cashe file count = " + searchAbleGlobal.Count);
            }

            foreach (var item in searchAbleGlobal) //This is where I check if they exist and delete them, and then force reload/refresh
            {
                if (Debugging)
                {
                    Debug.Log(item);
                }
            }

            if (searchAbleLocal.Count > 0)
            {
                return true;
            }
            return false;
        }

    }
}



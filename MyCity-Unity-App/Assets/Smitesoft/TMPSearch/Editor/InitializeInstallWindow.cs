using UnityEditor;
using UnityEngine;
using System.IO;

namespace TMPExtension
{
    [InitializeOnLoad]
    public class InitializeInstallWindow
    {
        static InitializeInstallWindow()
        {
            EditorApplication.projectChanged += OnProjectChanged;
        }

        static void OnProjectChanged()
        {
            string[] dirs = Directory.GetDirectories("Assets/Smitesoft/TMPSearch", "Editor/Resources", SearchOption.AllDirectories);
            string SOPath = Path.Combine(dirs[0], "EditorDatabase.asset");

            //Debug.Log("Path = " + SOPath);

            if (File.Exists(SOPath))
            {
                if (EditorScriptSO.ReturnState("InstallStage") == EditorScriptSO.ReturnState("InstallValveState"))
                {
                    InstallationWindow.showWindow();
                    EditorScriptSO.RunInstalValve();
                }
                else
                {
                    if (EditorScriptSO.ReturnState("InstalationState") == 1) //This MSG is generally shown if your installation is not completed
					{
						Debug.Log("Note: TMPro_Searchable Installations/Status Window can be found in Tools -> Smitesoft -> TMP-Integration"); 
					}					
                    //Debug.Log("Install/Valve Missmatch");                    
                }
            }
            else //First Time we Unpack the Asset
            {
                Debug.Log("DataBaseCreated");
                EditorScriptSO.Start();
                EditorScriptSO.RunInstalValve();  //Adding the + 1 right of the get go
				EditorScriptSO.SaveInstalationStatus(); //+1 means not Installed, +2 means its insalled
				InstallationWindow.showWindow();
            }
        }
    }    

}


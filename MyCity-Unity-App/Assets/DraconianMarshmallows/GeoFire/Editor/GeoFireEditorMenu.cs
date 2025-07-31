using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GeoFireEditorMenu : MonoBehaviour
{
    [MenuItem("GeoFire/Support Site")] public static void launchSupportSite()
    {
        Application.OpenURL("http://www.draconianmarshmallows.com/geofire/guide.php");
    }

    [MenuItem("GeoFire/Find ReadMe")] public static void openReadMe()
    {
        var asset = AssetDatabase.LoadMainAssetAtPath(
            "Assets/DraconianMarshmallows/GeoFire/README.txt");

        Selection.SetActiveObjectWithContext(asset, asset);
    }

    [MenuItem("GeoFire/Open Example Scene")] public static void openExample()
    {
        EditorSceneManager.OpenScene(
            "Assets/DraconianMarshmallows/GeoFire/example/SaveAndRetrieveLocation.unity");
    }

    [MenuItem("GeoFire/Visit Draconian Marshmallows")] public static void visitUs()
    {
        Application.OpenURL("http://www.draconianmarshmallows.com");
    }
}

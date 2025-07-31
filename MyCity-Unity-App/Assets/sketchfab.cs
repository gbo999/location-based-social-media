using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class sketchfab : MonoBehaviour
{


    public GameObject obj;

    // Start is called before the first frame update

/*
    object jsonToget;

    public IEnumerator GetRequestWithBody(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            //_json = "[\"" + _string + "\"]";
            www.SetRequestHeader("Authorization", "Token " + MyCity.Config.APIKeys.SketchfabToken);
            //  www.SetRequestHeader("accept", "text/plain");
            //  www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(_json));
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log(jsonResult);

                    
                  //  jsonToget = JsonUtility.FromJson<object>(jsonResult);


                }
            }
        }


    }





*/



        void Start()
    {

        Instantiate(obj,Camera.main.transform);



        /*    StartCoroutine(GetRequestWithBody("https://api.sketchfab.com/v3/models/49d97ca2fbf34f85b6c88ae8ebc7514f/download"));*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsExamples
{
    /// <summary>
    /// Example of get place predictions from Google Autocomplete API.
    /// </summary>
    [AddComponentMenu("Infinity Code/Online Maps/Examples (API Usage)/FindAutocompleteExample")]
    public class FindAutocompleteExample : MonoBehaviour
    {
        /// <summary>
        /// Google API Key
        /// </summary>
        /// 

        public Dropdown drop;

        public Text text;

        public InputField inputsearch;


        public string apiKey;



        public void onchaged(string s)
        {



            while (drop.options.Count > 0)
            {
                drop.options.RemoveAt(0);

            }







            OnlineMapsGooglePlacesAutocomplete.Find(
                s,
                apiKey
                ).OnComplete += OnComplete;




        }



        private void Start()
        {

            drop.options.RemoveAt(0);
            drop.options.RemoveAt(0);
            drop.options.RemoveAt(0);

            // Makes a request to Google Places Autocomplete API.

        }

        /// <summary>
        /// This method is called when a response is received.
        /// </summary>
        /// <param name="s">Response string</param>
       
        
        
        
        private void OnComplete(string s)
        {
            // Trying to get an array of results.
            OnlineMapsGooglePlacesAutocompleteResult[] results = OnlineMapsGooglePlacesAutocomplete.GetResults(s);

            // If there is no result
            if (results == null)
            {
                Debug.Log("Error");
                Debug.Log(s);
                return;
            }

            List<string> l = new List<string>();

            // Log description of each result.
            foreach (OnlineMapsGooglePlacesAutocompleteResult result in results)
            {

                l.Add(result.description);
                
                
                Debug.Log(result.description);
                
            }

        

           



            drop.AddOptions(l);
            text.text = "";

           

            drop.Show();



        }
    }
}
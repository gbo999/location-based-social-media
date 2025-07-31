/*         INFINITY CODE         */
/*   https://infinity-code.com   */

using NinevaStudios.Places;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InfinityCode.OnlineMapsDemos
{
    [AddComponentMenu("Infinity Code/Online Maps/Demos/Search Panel")]
    public class SearchPanel:MonoBehaviour
    {
        public InputField inputField;
        private OnlineMapsMarker marker;

        public TMP_Dropdown drop;

        public TMP_InputFieldSearchable SearchInput;

        public TMP_DropdownSearchable droptwo;

        public UIElementsGroup searchGroup;

        private void Start()
        {
            droptwo.OnSelected.AddListener(SearchDropDown);
        }

        public void testing()
        {
            Debug.Log("testing");

        }

        public void SearchDropDown()
        {

            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
                return;
            }

            /*if (inputField == null) return;
            if (inputField.text.Length < 3) return;*/

            Debug.Log("the dropdown value " + droptwo.options[droptwo.value].text);


            string locationName = droptwo.options[droptwo.value].text;

            OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding(locationName, OnlineMapsKeyManager.GoogleMaps());
            request.OnComplete += OnGeocodingComplete;
            request.Send();

            searchGroup.ChangeVisibility(false);

            SearchInput.text = "";

        }

        public void showSearchGroup()
        {

            #if UNITY_EDITOR

            searchGroup.ChangeVisibility(true);

            #endif

        }



        public void Search()
        {
            if (!OnlineMapsKeyManager.hasGoogleMaps)
            {
                Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
                return;
            }

            if (inputField == null) return;
            if (inputField.text.Length < 3) return;

            string locationName = inputField.text;

            OnlineMapsGoogleGeocoding request = new OnlineMapsGoogleGeocoding(locationName, OnlineMapsKeyManager.GoogleMaps());
            request.OnComplete += OnGeocodingComplete;
            request.Send();

      
        }

        private void OnGeocodingComplete(string response)
        {
            OnlineMapsGoogleGeocodingResult[] results = OnlineMapsGoogleGeocoding.GetResults(response);
            if (results == null || results.Length == 0)
            {
                Debug.Log(response);
                return;
            }

            OnlineMapsGoogleGeocodingResult r = results[0];
            OnlineMaps.instance.position = r.geometry_location;

            Vector2 center;
            int zoom;
            OnlineMapsUtils.GetCenterPointAndZoom(new[] { r.geometry_bounds_northeast, r.geometry_bounds_southwest }, out center, out zoom);
            OnlineMaps.instance.zoom = zoom;

            if (marker == null) marker = OnlineMapsMarkerManager.CreateItem(r.geometry_location, r.formatted_address);
            else
            {
                marker.position = r.geometry_location;
                marker.label = r.formatted_address;
            }
        }

     

        private void Update()
        {



            EventSystem eventSystem = EventSystem.current;
            if ((Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return)) && eventSystem.currentSelectedGameObject == inputField.gameObject)
            {
                Search();
            }
        }


        public void GoCurrentLocation()
        {

            float lng, lat;
            
            OnlineMapsLocationService.instance.GetLocation(out lng, out lat);

            OnlineMaps.instance.position = new Vector2(lng,lat);
            
            
            
            
            if (marker == null) marker = OnlineMapsMarkerManager.CreateItem(OnlineMaps.instance.position);
            else
            {
                marker.position = OnlineMaps.instance.position;
            }
            
            


        }
        
        
        
        
    }
}

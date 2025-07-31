using System;
using InfinityCode.OnlineMapsExamples;
using SocialApp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NinevaStudios.Places;
using UnityEngine;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.MapLoader;

public class PinButton : MonoBehaviour
{
    // Start is called before the first frame update

    #region Fields

    private static PinButton _instance;

    public Image img;

    public Sprite save;

    public Sprite pin;

    public UIElementsGroup chooseTheme;

    public UIElementsGroup Sure;

    [SerializeField] private UIElementsGroup _placeOrPin;

    public Animator anim;

    public PutSpecialMarkers put;

    public UIElementsGroup helperText;

    public UIElementsGroup DragHelper;

    public UIElementsGroup postType;

    private int counter = 1;

    private bool saveMode = false;

    private bool _isStarted = false;

    #endregion


    private void search(Place obj)
    {
        if (!OnlineMapsKeyManager.hasGoogleMaps)
        {
            Debug.LogWarning("Please enter Map / Key Manager / Google Maps");
            return;
        }

        if (obj.formattedAddress == null) return;
        if (obj.formattedAddress.Length < 3) return;

        string locationName = obj.formattedAddress;

        OnlineMapsGoogleGeocoding request =
            new OnlineMapsGoogleGeocoding(locationName, OnlineMapsKeyManager.GoogleMaps());
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

        put.longToSave = r.geometry_location.x;
        put.latToSave = r.geometry_location.y;
    }

    public void ChooseShare()
    {
        //  img.sprite = save;
        saveMode = true;
        AppManager.myCityController.currentTag = AppSettings.ShareTag;

        chooseTheme.ChangeVisibility(false);
        _placeOrPin.ChangeVisibility(false);
        markersMode.markerMode = true;
        // img.sprite = save;
        saveMode = true;
        helperText.ChangeVisibility(true);
    }


    public void ShowPlaceOrAdress()
    {
        //_placeOrPin.ChangeVisibility(true);


        PopupSystem.Instance.ShowPopup(PopupType.HereOrAddressOrPin, "");
    }


    public void SearchPlace()
    {
        _placeOrPin.ChangeVisibility(false);


        OnShowPlacesAutocomplete();
    }

    public void OnShowPlacesAutocomplete()
    {
        var europeBounds = new Place.LatLngBounds(new Place.LatLng(40, -10), new Place.LatLng(70, 40));
        var options =
            new AutocompleteOptions.Builder( /* show a full screen mode activity */ AutocompleteMode.Overlay,
                    new List<Place.Field> {Place.Field.Id})
                .Build();
        Places.ShowPlaceAutocomplete(options, HandlePlace, ShowText,
            () => { ShowText("Autocomplete was cancelled."); });
    }

    private void ShowText(string obj)
    {
        throw new System.NotImplementedException();
    }

    private void HandlePlace(Place place)
    {
        if (!string.IsNullOrEmpty(place.placeID))
        {
            OnGetPlaceClicked(place.placeID);
        }
    }

    public void OnGetPlaceClicked(string placeId)
    {
        if (string.IsNullOrEmpty(placeId))
        {
            //ShowText("Place ID is empty. Please, fill it before calling GetPlace.");
            return;
        }

        var allFields = Enum.GetValues(typeof(Place.Field)).OfType<Place.Field>().ToList();
        Places.FetchPlace(placeId, allFields, OnPlaceChosen,
            ShowText);
    }


    private void OnPlaceChosen(Place obj)
    {
        put.addressToSave = obj.name;
        search(obj);
        Sure.ChangeVisibility(true);
        put.areSure();
    }

    public void SaveSetSprite()
    {
        img.sprite = save;
    }

    public void OnButtonClick()
    {
        if (!_isStarted)
        {
            ShowPlaceOrAdress();
            _isStarted = true;
        }

        else
        {
            PopupSystem.Instance.ShowPopup(PopupType.ApproveLocation,PostProcessController.Instance.Address);
            _isStarted = false;
        }
        
        
        
    }

    public void AreYouSure()
    {
        anim.SetTrigger("flip");
        /*Sure.ChangeVisibility(false);
        //  helperText.ChangeVisibility(false);

        //    selectAct.ChangeVisibility(true);

        postType.ChangeVisibility(true);*/
    }

    public void Nah()
    {
        Sure.ChangeVisibility(false);
    }

    public void ChangeSure()
    {
        img.sprite = pin;
        saveMode = false;
        anim.SetBool("isPressed", false);
    }

    /*public void enter()
    {
        anim.SetBool("isPressed", false);

        Debug.Log("in enter ");
    }

    public void ChooseMusic()
    {
        //  img.sprite = save;
        saveMode = true;
        AppManager.myCityController.currentTag = AppSettings.MusicTag;

        chooseTheme.ChangeVisibility(false);
        markersMode.markerMode = true;
        // img.sprite = save;
        saveMode = true;
        helperText.ChangeVisibility(true);
    }

    public void ChooseReligion()
    {
        // saveMode = true;
        // img.sprite = save;
        AppManager.myCityController.currentTag = AppSettings.religiontag;
        chooseTheme.ChangeVisibility(false);
        markersMode.markerMode = true;
        //  img.sprite = save;
        saveMode = true;
        helperText.ChangeVisibility(true);
    }

    public void ChooseSports()
    {
        // img.sprite = save;
        // saveMode = true;
        AppManager.myCityController.currentTag = AppSettings.sporttag;
        chooseTheme.ChangeVisibility(false);
        markersMode.markerMode = true;
        saveMode = true;

        helperText.ChangeVisibility(true);
    }

    public void ChooseEnrichment()
    {
        //  img.sprite = save;
        markersMode.markerMode = true;

        AppManager.myCityController.currentTag = AppSettings.enrichmentTag;
        chooseTheme.ChangeVisibility(false);
        saveMode = true;
        helperText.ChangeVisibility(true);
    }

    public void exit()
    {
        Debug.Log("counter is " + counter);

        anim.SetBool("isPressed", true);
        
    }*/

    #region Properties

    public static PinButton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PinButton>();
            }

            return _instance;
        }
    }

    #endregion
}
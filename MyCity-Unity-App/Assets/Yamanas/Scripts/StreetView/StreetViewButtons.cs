using System;
using System.Collections;
using System.Collections.Generic;
using InfinityCode.uPano;
using InfinityCode.uPano.Actions;
using InfinityCode.uPano.Examples;
using InfinityCode.uPano.Renderers.Base;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.Map;
using Yamanas.Scripts.MapLoader;

public class StreetViewButtons : MonoBehaviour
{
    #region Fields

    public event Action OnCreationAvailable;


    [SerializeField] private GameObject _pinButton;

    [SerializeField] private GameObject _totemButton;

    [SerializeField] private OnlineMapsPanoConnector _onlineMapsPanoConnector;

    [SerializeField] private Sprite _savespeSprite;

    [SerializeField] private Image _totemImage;

    private bool _saveState;

    private bool _isStarted;
    
    #endregion

    #region methods

    private void Start()
    {
        _isStarted = false;
        Pano.OnPanoStarted += SubtitueButton;
        Pano.OnPanoDestroy += SubtitueButton;
        FindObjectOfType<InstantiateGameObjectsUnderCursorExample>().OnButtonUp += ChangeIcon;
        
    }

    private void ChangeIcon()
    {
        _totemImage.sprite = _savespeSprite;
        _saveState = true;
    }

    private void SubtitueButton(Pano obj)
    {

        _isStarted = !_isStarted;
        _pinButton.SetActive(!_pinButton.activeInHierarchy);
        _totemButton.SetActive(!_totemButton.activeInHierarchy);
    }


    public void EnableStreetView()
    {
        if (!_isStarted)
        {
            var pano = _onlineMapsPanoConnector;

            if (pano.enabled)
            {
                pano.enabled = false;
            }
            else
            {
                pano.enabled = true;
            }
        }

    }


    public void CloseStreetView()
    {
        if (_isStarted)
        {
          Destroy(FindObjectOfType<Pano>().gameObject);
        }
       
    }


    public void AllowCreation()
    {
        if (_saveState)

        {
            Debug.Log("savestate");
            FindObjectOfType<InstantiateGameObjectsUnderCursorExample>().Save();
            return;
        }

        OnCreationAvailable?.Invoke();
    }

    #endregion
}
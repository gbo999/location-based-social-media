using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yamanas.Scripts.MapLoader.AR
{
    public class ARPopupSystem : MonoBehaviour
    {
        #region Fields

        [SerializeField] private UIElementsGroup _chooseActivity;

        [SerializeField] private UIElementsGroup _shareActivity;

        [SerializeField] private UIElementsGroup _sellActivity;

        [SerializeField] private UIElementsGroup _eventActivity;

        [SerializeField] private UIElementsGroup _pollActivity;

        [SerializeField] private UIElementsGroup _success;

        private Dictionary<ARPopupType, UIElementsGroup> _popupObjects;

        [SerializeField] private Camera _arCam;

        [SerializeField] private float _distance = 2.5f;

        #endregion

        private void Awake()
        {
            _popupObjects = new Dictionary<ARPopupType, UIElementsGroup>
            {
                {ARPopupType.ChooseActivity, _chooseActivity},
                {ARPopupType.ShareActivity, _shareActivity},
                {ARPopupType.SellActivity, _sellActivity},
                {ARPopupType.EventActivity, _eventActivity},
                {ARPopupType.PollActivity, _pollActivity},
                {ARPopupType.Success,_success}
            };
        }

        public void ShowPopup(ARPopupType arPopupType)
        {
            foreach (var VARIABLE in _popupObjects)
            {
                if (VARIABLE.Value.Visible)
                {
                    VARIABLE.Value.ChangeVisibility(false);
                }
            }

            Debug.Log($"arCam position: {_arCam.transform.position}");

            Debug.Log($"arCam forward: {_arCam.transform.forward}");


            var menu = _popupObjects[arPopupType];


            menu.transform.position = _arCam.transform.position + _arCam.transform.forward * _distance;

            menu.transform.rotation = _arCam.transform.rotation;

            menu.ChangeVisibility(true);
        }
    }
}
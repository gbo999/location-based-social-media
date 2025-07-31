using System;
using System.Collections.Generic;
using Ricimi;
using RSG;
using UnityEngine;
using Yamanas.Scripts.MapLoader.Flows;
using Object = UnityEngine.Object;

namespace Yamanas.Infrastructure.Popups
{
    public class PopupSystem : MonoBehaviour
    {
        #region Fields

        [Header("Post Process")] [SerializeField]
        private UIElementsGroup _hereOrAddressOrPin;

        [SerializeField] private UIElementsGroup _approveLocation;

        [SerializeField] private UIElementsGroup _chooseActivity;

        [SerializeField] private UIElementsGroup _shareActivity;

        [SerializeField] private UIElementsGroup _sellActivity;

        [SerializeField] private UIElementsGroup _eventActivity;

        [SerializeField] private UIElementsGroup _pollActivity;

        [Header("Login")] [SerializeField] private UIElementsGroup _leavingApprove;

        [SerializeField] private UIElementsGroup _login;

        [SerializeField] private UIElementsGroup _signUp;

        [Header("Other")] [SerializeField] private UIElementsGroup _filterActivity;

        [SerializeField] private UIElementsGroup _profile;

        [SerializeField] private UIElementsGroup _success;

        [SerializeField] private UIElementsGroup _fail;

        [SerializeField] private UIElementsGroup _pressHelper;

        [SerializeField] private UIElementsGroup _dragHelper;

        [SerializeField] private UIElementsGroup _postComments;

        [SerializeField] private UIElementsGroup _chat;

        [SerializeField] private UIElementsGroup _participants;

        [SerializeField] private UIElementsGroup _shop;

        [SerializeField] private UIElementsGroup _leaderboard;
        
        private Dictionary<PopupType, UIElementsGroup> _popupObjects;

        private static PopupSystem _instance;

        #endregion

        #region Methods

        public bool IsVisible(PopupType popupType)
        {
            return _popupObjects[popupType].Visible;
        }
        
        private void Awake()
        {
            Debug.Log("before initialing dictionary");

            try
            {
                _popupObjects = new Dictionary<PopupType, UIElementsGroup>
                {
                    {PopupType.HereOrAddressOrPin, _hereOrAddressOrPin},
                    {PopupType.ApproveLocation, _approveLocation},
                    {PopupType.ChooseActivity, _chooseActivity},
                    {PopupType.ShareActivity, _shareActivity},
                    {PopupType.SellActivity, _sellActivity},
                    {PopupType.EventActivity, _eventActivity},
                    {PopupType.PollActivity, _pollActivity},
                    {PopupType.FilterActivity, _filterActivity},
                    {PopupType.LeavingApprove, _leavingApprove},
                    {PopupType.Login, _login},
                    {PopupType.SignUp, _signUp},
                    {PopupType.Profile, _profile},
                    {PopupType.Success, _success},
                    {PopupType.Fail, _fail},
                    {PopupType.PressHelper, _pressHelper},
                    {PopupType.DrageHelper, _dragHelper},
                    {PopupType.PostComments, _postComments},
                    {PopupType.Chat,_chat},
                    {PopupType.Participants,_participants},
                    {PopupType.Shop,_shop},
                    {PopupType.Leaderboard,_leaderboard}
                    
                };
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void ShowPopup<T>(PopupType popupType, T data)
        {
            foreach (var VARIABLE in _popupObjects)
            {
                if (VARIABLE.Value.Visible)
                {
                    VARIABLE.Value.ChangeVisibility(false);
                }

                /*if (VARIABLE.Value.Visible && VARIABLE.Key != PopupType.HereOrAddressOrPin)
                {
                    VARIABLE.Value.ChangeVisibility(false);
                }*/
            }

            var popupObject = _popupObjects[popupType];
            popupObject.ChangeVisibility(true);
            if (popupType == PopupType.Profile || popupType == PopupType.ApproveLocation||popupType == PopupType.Chat ||popupType == PopupType.Participants||popupType==PopupType.Shop||popupType==PopupType.Leaderboard)
            {
                popupObject.GetComponent<IPopup<T>>().SetData(data);
            }
        }

        public IPromise<T> showPopupWIthPromise<T>(PopupType popupType, T data)
        {
            foreach (var VARIABLE in _popupObjects)
            {
                if (VARIABLE.Value.Visible && VARIABLE.Key != PopupType.HereOrAddressOrPin)
                {
                    VARIABLE.Value.ChangeVisibility(false);
                }
            }

            var popupObject = _popupObjects[popupType];
            popupObject.ChangeVisibility(true);
            if (popupType == PopupType.Profile || popupType == PopupType.ApproveLocation)
            {
                popupObject.GetComponent<IPopup<T>>().SetData(data);
            }

            var p = new Promise<T>();

            popupObject.GetComponent<IPromisePopup<T>>().SetPromise(p);

            return p;
        }

        public void CloseAllPopups()
        {
            foreach (var VARIABLE in _popupObjects)
            {
                if (VARIABLE.Value.Visible)
                {
                    VARIABLE.Value.ChangeVisibility(false);
                }
            }
        }

        public void ClosePopup(PopupType popupType)
        {
            _popupObjects[popupType].ChangeVisibility(false);
        }

        #endregion

        #region Properties

        public static PopupSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<PopupSystem>();
                }

                return _instance;
            }
        }

        #endregion
    }
}
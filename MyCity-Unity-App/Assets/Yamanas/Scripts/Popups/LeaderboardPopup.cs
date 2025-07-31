using System.Collections.Generic;
using SocialApp;
using UnityEngine;
using Yamanas.Infrastructure.Popups;


namespace Yamanas.Scripts.MapLoader.Popups
{
    public class LeaderboardPopup : MonoBehaviour, IPopup<string>
    {
        #region Fields

        [SerializeField] private GameObject[] UserObjects = default;

        #endregion


        #region Methods

        public void OnCLoseButtonClick()
        {
            PopupSystem.Instance.ClosePopup(PopupType.Leaderboard);
        }

        private void GetUserList()
        {
            //FriendsCountLabel.text = string.Empty;
            foreach (GameObject _go in UserObjects)
            {
                _go.SetActive(false);
            }

            AppManager.FIREBASE_CONTROLLER.GetleaderBoard(OnFriendsLoaded);
            /*AppManager.FIREBASE_CONTROLLER.GetUserFriendsCount(CurrentUserID,
                _count => { FriendsCountLabel.text = "Friends (" + _count + ")"; })*/
            ;
        }

        public void OnFriendsLoaded(List<User> _callback)
        {
            for (int i = 0; i < _callback.Count; i++)
            {
                UserObjects[i].SetActive(true);
                UserObjects[i].GetComponent<UserViewController>().DisplayInfo(_callback[i]);
            }
        }

        #endregion


        public void SetData(string data)
        {
            GetUserList();
        }
    }
}
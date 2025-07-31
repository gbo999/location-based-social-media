using System.Collections.Generic;
using SocialApp;
using UnityEngine;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class ParticipantsPopup:MonoBehaviour,IPopup<Feed>
    {

        #region Fields

        [SerializeField] private GameObject[] UserObjects = default;

        private Feed _feed;
        
        
        #endregion
        
        
        #region Methods


        public void OnCLoseButtonClick()
        {
            PopupSystem.Instance.ClosePopup(PopupType.Participants);
        }
        
        private void GetUserList()
        {
            //FriendsCountLabel.text = string.Empty;
            foreach (GameObject _go in UserObjects)
            {
                _go.SetActive(false);
            }

            AppManager.FIREBASE_CONTROLLER.GetUsersWhoParticipatePost(_feed.Key,OnFriendsLoaded);

            /*AppManager.FIREBASE_CONTROLLER.GetUserFriendsCount(CurrentUserID,
                _count => { FriendsCountLabel.text = "Friends (" + _count + ")"; })*/;
        }

        public void OnFriendsLoaded(List<User> _callback)
        {
            
            UserObjects[0].SetActive(true);
            UserObjects[0].GetComponent<UserViewController>().DisplayInfoOfOwner(_feed.OwnerID);
            
                for (int i = 0; i < _callback.Count; i++)
                {
                    UserObjects[i+1].SetActive(true);
                    UserObjects[i+1].GetComponent<UserViewController>().DisplayInfo(_callback[i]);
                }
            
        }

        #endregion


        public void SetData(Feed data)
        {
            _feed = data;
            GetUserList();

        }
    }
}
using SocialApp;
using UnityEngine;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class ChatPopup:MonoBehaviour,IPopup<MessageGroupInfo>
    {



        #region Methods

        public void SetData(MessageGroupInfo data)
        {
            GetComponent<MessagesDataLoader>().LoadMessageGroup(data);
            
        }


        public void OnCloseButtonClick()
        {
            PopupSystem.Instance.ClosePopup(PopupType.Chat);
        }
        
        #endregion
       
    }
}
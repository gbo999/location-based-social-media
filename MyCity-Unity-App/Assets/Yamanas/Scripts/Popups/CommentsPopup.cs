using UnityEngine;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class CommentsPopup:MonoBehaviour
    {
        public void OnCloseButtonClick()
        {
            PopupSystem.Instance.ClosePopup(PopupType.PostComments);
            
        }
    }
}
using UnityEngine;
using System;

namespace SocialApp
{
    public class FeedPopupViewController : MonoBehaviour
    {
        private Action<FeedPopupAction> CurrentAction;

        private void OnDisable()
        {
            CurrentAction = null;
        }

        public void SetupWindows(Action<FeedPopupAction> _action)
        {
            CurrentAction = _action;
        }

        public void OnDeletePost()
        {
            CurrentAction?.Invoke(FeedPopupAction.DELETE);
            HideWindows();
        }

        public void HideWindows()
        {
            AppManager.VIEW_CONTROLLER.HideFeedPopup();
        }
    }

    public enum FeedPopupAction
    {
        NONE,
        DELETE
    }
}

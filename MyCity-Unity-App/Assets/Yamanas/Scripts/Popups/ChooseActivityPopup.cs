using System.Linq;
using SocialApp;
using UnityEngine;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class ChooseActivityPopup : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ToggleGroup tog;

        #endregion

        #region Methods


        public void OncloseButtonClick()
        {
            
            PopupSystem.Instance.ClosePopup(PopupType.ChooseActivity);
            
            
        }
        
        
        
        public void OnActivityButtonClick()
        {
            string kind = tog.ActiveToggles().First().name;

          PopupSystem.Instance.ClosePopup(PopupType.ChooseActivity);

            if (kind == "Event")
            {
                PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.EventActivity, "");

                AppManager.myCityController.currentTag = AppSettings.EventTag;
            }

            else if (kind == "Sale")
            {
                PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.SellActivity, "");


                AppManager.myCityController.currentTag = AppSettings.SaleTag;
            }


            else if (kind == "Share")
            {
                PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.ShareActivity, "");

                AppManager.myCityController.currentTag = AppSettings.ShareTag;
            }


            else
            {
                PostProcessController.Instance.PopupSystem.ShowPopup(PopupType.PollActivity, "");

                AppManager.myCityController.currentTag = AppSettings.PollTag;
            }

            PostProcessController.Instance.Kind = $"{kind}";
        }

        #endregion
    }
}
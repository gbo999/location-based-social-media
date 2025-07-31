using System.Linq;
using SocialApp;
using UnityEngine;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.MapLoader.AR;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class ChooseActivityPopupAR : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ToggleGroup tog;

        #endregion

        #region Methods

        public void OnActivityButtonClick()
        {
            string kind = tog.ActiveToggles().First().name;


            if (kind == "Event")
            {
                PostProcessController.Instance.ARPopupSystem.ShowPopup(ARPopupType.EventActivity);

                AppManager.myCityController.currentTag = AppSettings.EventTag;
            }

            else if (kind == "Sale")
            {
                PostProcessController.Instance.ARPopupSystem.ShowPopup(ARPopupType.SellActivity);


                AppManager.myCityController.currentTag = AppSettings.SaleTag;
            }


            else if (kind == "Share")
            {
                PostProcessController.Instance.ARPopupSystem.ShowPopup(ARPopupType.ShareActivity);

                AppManager.myCityController.currentTag = AppSettings.ShareTag;
            }


            else
            {
                PostProcessController.Instance.ARPopupSystem.ShowPopup(ARPopupType.PollActivity);

                AppManager.myCityController.currentTag = AppSettings.PollTag;
            }

            PostProcessController.Instance.Kind = $"{kind}";
        }

        #endregion
    }
}
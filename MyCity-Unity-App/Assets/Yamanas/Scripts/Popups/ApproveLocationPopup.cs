using TMPro;
using UnityEngine;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class ApproveLocationPopup : MonoBehaviour,
        IPopup<string>
    {
        [SerializeField] private TMP_Text _locationApprovalText;

        public void SetData(string data)
        {
            _locationApprovalText.text = $"Are you sure you want to place a pin at {data}";
        }

        public void OnSureButtonClick()
        {
           PopupSystem.Instance.ClosePopup(PopupType.ApproveLocation);
            PopupSystem.Instance.ShowPopup(PopupType.ChooseActivity, "");
        }

        public void OnCancelButtonClick()
        {
            PostProcessController.Instance.PopupSystem.ClosePopup(PopupType.ApproveLocation);
        }
    }
}
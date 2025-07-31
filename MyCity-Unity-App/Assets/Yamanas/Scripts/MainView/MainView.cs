using UnityEngine;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MainView
{
    public class MainView : MonoBehaviour


    {
        public void OnFilterButtonClick()
        {
            PopupSystem.Instance.ShowPopup(PopupType.FilterActivity, "");
        }
    }
}
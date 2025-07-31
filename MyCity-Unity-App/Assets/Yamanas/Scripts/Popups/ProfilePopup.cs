using System;
using SocialApp;
using UnityEngine;
using Yamanas.Infrastructure.Popups;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class ProfilePopup : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        public void OnCloseButtonClick()
        {
            int childs = _transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(_transform.GetChild(i).gameObject);
            }

            PopupSystem.Instance.ClosePopup(PopupType.Profile);
        }

        public void OnPointsButtonClick()
        {
            
            PopupSystem.Instance.ShowPopup(PopupType.Leaderboard,"");
            
        }
    }
}
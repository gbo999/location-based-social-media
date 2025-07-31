using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.Infrastructure.Server;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class FilterPopup:MonoBehaviour
    {
        
        [SerializeField] private Toggle[] _toggles;

        [SerializeField] private Slider _slider;

        #region Methods

        public void OnFilterButtonClick()
        {
            Dictionary<string, bool> filterData =new Dictionary<string, bool>();

            OnlineMapsMarker3DManager.RemoveAllItems();
            foreach (var toggle in _toggles)
            {
                if (toggle.isOn)
                {
                    GeoPostLoader.Instance.MakeTagQuery(toggle.name,_slider.value);
                    
                    filterData.Add(toggle.name,true);
                    
                }
                else
                {
                    filterData.Add(toggle.name,false);

                }
            }

            GeoPostLoader.Instance.ChangeFilterDate(filterData, _slider.value);
            
            PopupSystem.Instance.ClosePopup(PopupType.FilterActivity);

            
            
        }
        
        public void OnCloseButtonClick()
        {
            PopupSystem.Instance.ClosePopup(PopupType.FilterActivity);
        }

        #endregion
      
    }
}
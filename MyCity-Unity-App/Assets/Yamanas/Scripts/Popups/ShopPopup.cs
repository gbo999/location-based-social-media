using System;
using System.Collections;
using Firebase.Database;
using SocialApp;
using TMPro;
using UnityEngine;
using Yamanas.Infrastructure.Popups;
using Yamanas.Scripts.MapLoader.Shop;

namespace Yamanas.Scripts.MapLoader.Popups
{
    public class ShopPopup : MonoBehaviour, IPopup<string>
    {
        #region Fields

        [SerializeField] private TMP_Text _currencyText;

        private DatabaseReference _currecnyRef;

        private double CurrRes;

        [SerializeField] private ProductViewController[] _productViewControllers;
        
        #endregion

        public void SetData(string data)
        {
            CurrRes = 0;

            _currecnyRef = AppManager.FIREBASE_CONTROLLER.GetCurrentCurrency();
            _currecnyRef.ValueChanged += OnCurrencyChanged;
        }

        private void OnCurrencyChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            foreach (var productViewController in _productViewControllers)
            {
                productViewController.OnCurrencyGot(Convert.ToInt32(args.Snapshot.Value));
            }

            StartCoroutine(addCurrecny(args.Snapshot.Value));
        }

        IEnumerator addCurrecny(object currency)
        {
            int currencyRes;
            if (currency == null)
            {
                currencyRes = 0;
            }

            else
            {
                currencyRes = Convert.ToInt32(currency);
            }

            Debug.Log($"currecny is {currencyRes}");


            while (CurrRes < currencyRes)
            {
                CurrRes += 1;
                _currencyText.text = $"{CurrRes}k";

                yield return new WaitForSeconds(0.1f);
            }

            CurrRes = currencyRes;
        }

        public void OnCloseButtonClick()
        {
            PopupSystem.Instance.CloseAllPopups();
        }
    }
}
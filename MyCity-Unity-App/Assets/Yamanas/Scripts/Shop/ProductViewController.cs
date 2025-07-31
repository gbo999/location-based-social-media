using System;
using ARFoundationRemote.Runtime;
using Firebase.Database;
using SocialApp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Yamanas.Scripts.MapLoader.Shop
{
    public class ProductViewController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private string _productID;

        [SerializeField] private int _price;

        [SerializeField] private Color _colorBlank;

        [SerializeField] private Color _colorGray;

        [SerializeField] private Image _buttonImage;

        [SerializeField] private Image _coinImage;

        [SerializeField] private Image _backgroundImage;

        [SerializeField] private Image _itemImage;

        [SerializeField] private Image _itemBackGround;

        [SerializeField] private TMP_Text _title;

        [SerializeField] private TMP_Text _details;

        [SerializeField] private Button _purchaseButton;

        private int _globalCurrency;

        private bool _isBought;

        #endregion

        #region Methods

        /*private void Awake()
        {
            GetCurrency();
        }*/

        /*public void GetCurrency()
        {
            AppManager.FIREBASE_CONTROLLER.GetCurrecny(data =>
            {
                _globalCurrency = data;
                OnCurrencyGot();
            });
        }*/

        public void OnCurrencyGot(int currency)
        {

            _globalCurrency = currency;
            AppManager.FIREBASE_CONTROLLER.IsProdcutBought(_productID, data =>
            {
                _isBought = data;
                LoadProduct();
            });
        }

        private void Start()
        {
            //  LoadProduct();
        }

        public void LoadProduct()
        {
            Debug.Log($"Global currency {_globalCurrency}");
            Debug.Log($"isbought {_isBought}");

            if (_globalCurrency < _price || _isBought)
            {
                _backgroundImage.color = _colorGray;
                _buttonImage.color = _colorBlank;
                _coinImage.color = _colorBlank;
                _itemImage.color = _colorBlank;
                _title.color = _colorGray;
                _details.color = _colorGray;
                _itemBackGround.color = _colorGray;
                _purchaseButton.interactable = false;
            }
            /*else
            {
                _backgroundImage.color = Color.white;
                _buttonImage.color = Color.white;
                _coinImage.color = Color.white;
                _itemImage.color = Color.white;
                _title.color = Color.white;
                _details.color = Color.white;
                _itemBackGround.color = Color.white;
                _purchaseButton.interactable = true;
            }*/

            //check if you can purchase and check if already bought

            Debug.Log("load product success");
        }

        public void Purchase()
        {
            AppManager.FIREBASE_CONTROLLER.PurchaseProduct(_productID, LoadProduct);
            AppManager.FIREBASE_CONTROLLER.AddCurrecny(-_price);
        }

        #endregion
    }
}
using Adveritisement;
using AwesomeTools.DefinesConstant;
using GoogleMobileAds.Api;
using System;
using UnityEngine;
/// <summary>
/// Обработчик рекламного баннера 
/// </summary>
public class AdvertisementBannerAdHandler : BaseAdvertisementHandler
{
    protected override string AdvertisementType => "BannerAd";
    public override bool IsReady
    {
        get
        {
            try
            {
                _bannerView.Show();
            }
            catch (Exception e)
            {
                LoadBannerAd();

                return false;
            }

            return true;

        }
    }
    private Action _action;
    private BannerView _bannerView;

    private const string _bannerIdAndroid = "ca-app-pub-3940256099942544/6300978111";
    private const string _bannerTest = "ca-app-pub-3940256099942544/6300978111";

    private string PlatformId
    {
        get
        {
            if (ConstantsPlatform.UNITY_EDITOR) return _bannerTest;
            if (ConstantsPlatform.ANDROID) return _bannerTest;

            return string.Empty;
        }
    }
    /// <summary>
    /// Инициализация баннера и его загрузка 
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();

        LoadBannerAd();
    }
    /// <summary>
    /// Метод вызываемый при показе
    /// </summary>
    /// <param name="placement">место показа баннера</param>
    protected override void OnShow(AdvertisementPlacement placement)
    {
        
    }


    /// <summary>
    /// Метод когда баннер не готов, то вызывается снова
    /// </summary>
    protected override void OnNotReady()
    {
        base.OnNotReady();

        LoadBannerAd();
    }
    /// <summary>
    /// Возникает при регистрации клика по объявлению.
    /// </summary>
    private void BannerAdOnClickedEvent()
    {
        Debug.Log($"InterstitialVideoAdClickedEvent {CurrentPlacement};");
    }
    /// <summary>
    /// Возникает при регистрации показа объявления.
    /// </summary>
    private void BannerAdOnAdImpressionRecorded()
    {
        Debug.Log($"InterstitialVideoOnAdImpressionRecorded {CurrentPlacement};");
    }
    /// <summary>
    /// Возникает, когда объявление закрывает полноэкранный контент.
    /// </summary>
    private void BannerAdOnAdFullScreenContentClosed()
    {
        Debug.Log($"InterstitialVideoOnAdFullScreenContentClosed {CurrentPlacement};");

        LoadBannerAd();
    }
    /// <summary>
    /// Возникает, когда считается, что реклама заработала деньги.
    /// </summary>
    /// <param name="adValue">значение оплаты</param>
    private void BannerAdOnAdPaid(AdValue adValue)
    {
        Debug.LogError($"InterstitialVideoOnAdPaid {adValue.Value}, {adValue.CurrencyCode}");

        OnShowSuccess();
    }
    /// <summary>
    /// Создание видиости баннера на нижней позиции экрана
    /// </summary>
    private void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        DestroyBanner();

        AdSize adaptiveSize =
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        _bannerView = new BannerView(PlatformId, AdSize.Banner, AdPosition.Bottom);
    }
    /// <summary>
    /// Уничтожение баннера
    /// </summary>
    public void DestroyBanner()
    {
        if (_bannerView != null)
        {
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
    /// <summary>
    /// Загрузка баннера
    /// </summary>
    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }
    /// <summary>
    /// При утилизации отписка от событий
    /// </summary>
    protected override void OnDispose()
    {
        if(_bannerView == null) return;
        
        _bannerView.OnAdClicked -= BannerAdOnClickedEvent;
        _bannerView.OnAdImpressionRecorded -= BannerAdOnAdImpressionRecorded;
        _bannerView.OnAdPaid -= BannerAdOnAdPaid;
        _bannerView.OnAdFullScreenContentClosed -= BannerAdOnAdFullScreenContentClosed;
        
    }

    /// <summary>
    /// Ховає банер
    /// </summary>
    public void Hide()
    {
        _bannerView.Hide();
    }
}

using Adveritisement;
using AwesomeTools.DefinesConstant;
using GoogleMobileAds.Api;
using System;
using UnityEngine;

/// <summary>
/// Обработчик межстраничной рекламы
/// </summary>
public class AdvertisementInterstitialAdHandler : BaseAdvertisementHandler
{
    protected override string AdvertisementType => "InterstitialGoogleAd";
    public override bool IsReady
    {
        get
        {
            try
            {
                return _interstitialAd.CanShowAd();
            }
            catch (Exception e)
            {
                LoadInterstitialVideo();

                throw;
            }
        }
    }

    private InterstitialAd _interstitialAd;
    private Action _action;
    private const string _interstitialIdAndroid = "ca-app-pub-3940256099942544/1033173712";
    private const string _interstitialTest = "ca-app-pub-3940256099942544/1033173712";


    private string PlatformId
    {
        get
        {
            if (ConstantsPlatform.UNITY_EDITOR) return _interstitialTest;
            if (ConstantsPlatform.ANDROID) return _interstitialTest;

            return string.Empty;
        }
    }
    /// <summary>
    /// Инициализации рекламы и её загрузка
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();

        LoadInterstitialVideo();
    }
    /// <summary>
    /// Показать рекламу
    /// </summary>
    /// <param name="placement">место показа</param>
    protected override void OnShow(AdvertisementPlacement placement)
    {
        _interstitialAd.Show();
    }
    /// <summary>
    /// Если реклама не готова, то загрузить
    /// </summary>
    protected override void OnNotReady()
    {
        base.OnNotReady();

        LoadInterstitialVideo();
    }
    /// <summary>
    /// Возникает при регистрации клика по объявлению.
    /// </summary>
    private void InterstitialVideoOnClickedEvent()
    {
        Debug.Log($"InterstitialVideoAdClickedEvent {CurrentPlacement};");
    }
    /// <summary>
    /// Возникает при регистрации показа объявления.
    /// </summary>
    private void InterstitialVideoOnAdImpressionRecorded()
    {
        Debug.Log($"InterstitialVideoOnAdImpressionRecorded {CurrentPlacement};");
    }
    /// <summary>
    /// Возникает, когда рекламе не удается открыть полноэкранный контент.
    /// </summary>
    /// <param name="error">ошибка</param>
    private void InterstitialVideoOnFullScreenContentFailed(AdError error)
    {
        Debug.Log($"InterstitialVideoAdShowFailedEvent {CurrentPlacement}; Description: {error.GetMessage()}; Error {error.GetCode()};");

        OnShowFail();
    }
    /// <summary>
    /// Возникает, когда объявление закрывает полноэкранный контент.
    /// </summary>
    private void InterstitialVideoOnAdFullScreenContentClosed()
    {
        Debug.Log($"InterstitialVideoOnAdFullScreenContentClosed {CurrentPlacement};");

        LoadInterstitialVideo();
    }
    /// <summary>
    /// Возникает, когда считается, что реклама заработала деньги.
    /// </summary>
    /// <param name="adValue">значение оплаты</param>
    private void InterstitialVideoOnAdPaid(AdValue adValue)
    {
        Debug.LogError($"InterstitialVideoOnAdPaid {adValue.Value}, {adValue.CurrencyCode}");

        OnShowSuccess();
    }
    /// <summary>
    /// Загрузка межстраничной рекламы
    /// </summary>
    private void LoadInterstitialVideo()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        var adRequest = new AdRequest.Builder().Build();
        InterstitialAd.Load(PlatformId, adRequest, (ad, error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.LogError("interstitial ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log("Interstitial ad loaded with response : "
                      + ad.GetResponseInfo());

            _interstitialAd = ad;
            _interstitialAd.OnAdClicked += InterstitialVideoOnClickedEvent;
            _interstitialAd.OnAdImpressionRecorded += InterstitialVideoOnAdImpressionRecorded;
            _interstitialAd.OnAdFullScreenContentFailed += InterstitialVideoOnFullScreenContentFailed;
            _interstitialAd.OnAdPaid += InterstitialVideoOnAdPaid;
            _interstitialAd.OnAdFullScreenContentClosed += InterstitialVideoOnAdFullScreenContentClosed;
        });
    }

    /// <summary>
    /// При утилизации отписка от событий
    /// </summary>
    protected override void OnDispose()
    {
        _interstitialAd.OnAdClicked -= InterstitialVideoOnClickedEvent;
        _interstitialAd.OnAdImpressionRecorded -= InterstitialVideoOnAdImpressionRecorded;
        _interstitialAd.OnAdFullScreenContentFailed -= InterstitialVideoOnFullScreenContentFailed;
        _interstitialAd.OnAdPaid -= InterstitialVideoOnAdPaid;
        _interstitialAd.OnAdFullScreenContentClosed -= InterstitialVideoOnAdFullScreenContentClosed;
    }

    /// <summary>
    /// Вводимо дію [action]- додаємо події "OnInterstitialAdClosed" 
    /// </summary>   
    public void SetOnClosedAction(Action action)
    {        
        _action = action;
        _interstitialAd.OnAdFullScreenContentClosed += _action;
        _interstitialAd.OnAdFullScreenContentClosed += RemoveOnClosedAction;
    }

    /// <summary>
    /// Видаляємо дії з події "OnInterstitialAdClosed" 
    /// </summary>   
    private void RemoveOnClosedAction()
    {
        _interstitialAd.OnAdFullScreenContentClosed -= _action;          
        _interstitialAd.OnAdFullScreenContentClosed -= RemoveOnClosedAction;
    }
}

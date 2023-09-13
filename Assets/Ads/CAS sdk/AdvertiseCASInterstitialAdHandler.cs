using System;
using Adveritisement;
using CAS;
using AwesomeTools.DefinesConstant;
using UnityEngine;

public class AdvertiseCASInterstitialAdHandler : BaseAdvertisementHandler
{
   protected override string AdvertisementType => "InterstitialCASAd";
    public override bool IsReady
    {
        get
        {
            try
            {
                return _manager.IsReadyAd(AdType.Interstitial);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    private IMediationManager _manager;
    private Action _action;
    private const int INTERSTITIAL_INTERVAL_TIME = 1;
    /// <summary>
    /// Инициализации рекламы и её загрузка
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();
        _manager = MobileAds.BuildManager().Initialize();
        MobileAds.settings.loadingMode = LoadingManagerMode.Optimal;
        MobileAds.settings.interstitialInterval = INTERSTITIAL_INTERVAL_TIME;
        InitializeInterstitialAd();
    }
    /// <summary>
    /// Показать рекламу
    /// </summary>
    /// <param name="placement">место показа</param>
    protected override void OnShow(AdvertisementPlacement placement)
    {
        _manager.ShowAd(AdType.Interstitial);
    }
    /// <summary>
    /// Если реклама не готова, то загрузить
    /// </summary>
    protected override void OnNotReady()
    {
        base.OnNotReady();
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
    private void InterstitialVideoOnAdImpressionRecorded(AdMetaData metaData)
    {
        Debug.Log($"InterstitialVideoOnAdImpressionRecorded {CurrentPlacement} ::::: {metaData};");
    }
    /// <summary>
    /// Возникает, когда рекламе не удается открыть полноэкранный контент.
    /// </summary>
    /// <param name="error">ошибка</param>
    private void InterstitialVideoOnFullScreenContentFailed(string error)
    {
        Debug.Log($"InterstitialVideoAdShowFailedEvent {CurrentPlacement}; Description: {error}; Error {error};");

        OnShowFail();
    }
    /// <summary>
    /// Возникает, когда объявление закрывает полноэкранный контент.
    /// </summary>
    private void InterstitialVideoOnAdFullScreenContentClosed()
    {
        Debug.Log($"InterstitialVideoOnAdFullScreenContentClosed {CurrentPlacement};");
    }

    /// <summary>
    /// Загрузка межстраничной рекламы
    /// </summary>
    private void InitializeInterstitialAd()
    {
        _manager.OnInterstitialAdClicked += InterstitialVideoOnClickedEvent;
        _manager.OnInterstitialAdImpression += InterstitialVideoOnAdImpressionRecorded;
        _manager.OnInterstitialAdFailedToShow += InterstitialVideoOnFullScreenContentFailed;
        _manager.OnInterstitialAdClosed += InterstitialVideoOnAdFullScreenContentClosed;
    }
    /// <summary>
    /// При утилизации отписка от событий
    /// </summary>
    protected override void OnDispose()
    {
        _manager.OnInterstitialAdClicked -= InterstitialVideoOnClickedEvent;
        _manager.OnInterstitialAdImpression -= InterstitialVideoOnAdImpressionRecorded;
        _manager.OnInterstitialAdFailedToShow -= InterstitialVideoOnFullScreenContentFailed;
        _manager.OnInterstitialAdClosed -= InterstitialVideoOnAdFullScreenContentClosed;
    }

    /// <summary>
    /// Вводимо дію [action]- видаляємо діюї події "OnInterstitialAdClosed" 
    /// </summary>   
    public void SetOnClosedAction(Action action)
    {        
        _action = action;
        _manager.OnInterstitialAdClosed += _action;
        _manager.OnInterstitialAdClosed += RemoveOnClosedAction;
    }

    /// <summary>
    /// Вводимо дію [action]- видаляємо дії з події "OnInterstitialAdClosed" 
    /// </summary>   
    private void RemoveOnClosedAction()
    {
        _manager.OnInterstitialAdClosed -= _action;          
        _manager.OnInterstitialAdClosed -= RemoveOnClosedAction;
    }

    
}

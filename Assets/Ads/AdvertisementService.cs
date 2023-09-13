using System;
using System.Collections;
using Adveritisement;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdvertisementService : MonoBehaviour
{
    private const string DISABLE_ADS_KEY = "DisableADS";

    public AdvertisementBannerAdHandler BannerGoogle;
    public AdvertisementInterstitialAdHandler InterstitialGoogle;
    public AdvertiseCASInterstitialAdHandler InterstitialCAS;
    public AdvertisementCASBannerAdHandler BannerCAS;

    private bool _firstTimeOnMainScene = true;
    private bool _isADSDisabled = false;
    public bool IsADSDisabled => _isADSDisabled;
    public static bool ISCasAdEnabled = false;
    public static bool ISGoogleAdEnabled = false;

    private Coroutine _onAdMobBannerClosedCoroutine;
    private Coroutine _onCASBannerClosedCoroutine;
    /// <summary>
    /// Шукає об'єкт типу "AdvertisementService" та знищує об'єкт, якщо не співпадає  з елементом 
    /// </summary>
    private void Awake()
    {
        AdvertisementService advertisementService = FindObjectOfType<AdvertisementService>();
        if (advertisementService.gameObject != this.gameObject)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Виконує ф-цію "InitializeHandlers" та присвоює значення для "_isADSDisabled", "_firstTimeOnMainScene"
    /// </summary>
    public void Construct(){
        _isADSDisabled = PlayerPrefs.GetInt(DISABLE_ADS_KEY, 0) == 0 ? false : true;

        DontDestroyOnLoad(this);
        InitializeHandlers();

        _firstTimeOnMainScene = false;
    }

    /// <summary>
    /// Ініціалізує скрипти реклам увімкненого джерела
    /// </summary>
    private void InitializeHandlers()
    {
        if (ISCasAdEnabled) 
            ActivateCASAds();

        if (ISGoogleAdEnabled)
            ActivateGoogleAds();
    }

    public void ShowBannerAd(){
        if (ISCasAdEnabled) 
            BannerCAS.Show(AdvertisementPlacement.BannerCasAd);

        if (ISGoogleAdEnabled)
            BannerGoogle.Show(AdvertisementPlacement.BannerGoogleAd);
    }
    /// <summary>
    /// Активує скрипти реклам джерела "CAS sdk"
    /// </summary>
    private void ActivateCASAds()
    {
        InterstitialCAS = new AdvertiseCASInterstitialAdHandler();
        BannerCAS = new AdvertisementCASBannerAdHandler();

        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        InterstitialCAS.Initialize();
        BannerCAS.Initialize();

        MobileAds.Initialize(initStatus =>
        {
            BannerCAS.Initialize();
            InterstitialCAS.Initialize();
        });

        BannerCAS.PostInitialize();
        InterstitialCAS.PostInitialize();

        BannerCAS.Show(AdvertisementPlacement.BannerCasAd);
    }

    /// <summary>
    /// Активує скрипти реклам джерела "AdMob"
    /// </summary>
    private void ActivateGoogleAds()
    {
        BannerGoogle = new AdvertisementBannerAdHandler();
        InterstitialGoogle = new AdvertisementInterstitialAdHandler();

        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.Initialize(initStatus =>
        {
            BannerGoogle.Initialize();
            InterstitialGoogle.Initialize();
        });

        BannerGoogle.PostInitialize();
        InterstitialGoogle.PostInitialize();

    }

    /// <summary>
    /// Вимикає рекламу
    /// </summary>
    public void DisableADS()
    {
        if(IsADSDisabled) return;

        PlayerPrefs.SetInt(DISABLE_ADS_KEY, 1);
        _isADSDisabled = true;

        BannerGoogle.DestroyBanner();
    }

    /// <summary>
    /// Викликає "Dispose" для скриптів реклам
    /// </summary>
    private void OnDestroy()
    {
        if (BannerGoogle != null)
        {
            BannerGoogle.Dispose();
        }
        if (InterstitialGoogle != null)
        {
            InterstitialGoogle.Dispose();
        }
        if (BannerCAS != null)
        {
            BannerCAS.Dispose();
        }
        if (InterstitialCAS != null)
        {
            InterstitialCAS.Dispose();
        }
    }

}

using System.Diagnostics;
using Adveritisement;
using CAS;
using AwesomeTools.DefinesConstant;
using System;
using UnityEditor;
public class AdvertisementCASBannerAdHandler : BaseAdvertisementHandler
{
    protected override string AdvertisementType => "BannerAd";
    public override bool IsReady
    {
        get
        {
            try
            {
                
            }
            catch (Exception e)
            {
                return false;
            }

            return true;

        }
    }

    private Action _action;
    private IAdView _banner;
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
    /// Створює рекламу типа "Banner"
    /// </summary>
    protected override void OnInitialize()
    {        
        _banner = MobileAds.BuildManager().Build().GetAdView(AdSize.Banner);
        _banner.SetActive(false);
        _banner.Load();
    }

    /// <summary>
    /// Вводимо рекламу [view] - деактивовуємо рекламу [view]
    /// </summary>
    private void CloseAd(IAdView view)
    {
        view.SetActive(false);
    }

    /// <summary>
    /// Вводимо місце показу реклами [placement]- показуємо рекламу типа "Banner" [_banner]
    /// </summary>    
    protected override void OnShow(AdvertisementPlacement placement)
    {
        _banner.SetActive(true);
    }

    /// <summary>
    /// Спрацьовує при утилізації
    /// </summary>
    protected override void OnDispose()
    {
       
    }

    /// <summary>
    /// Деактивує банер
    /// </summary>
    public void Hide()
    {
        _banner.SetActive(false);
    }

}

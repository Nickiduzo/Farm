using System;
using System.Collections;
using Adveritisement;
using UnityEngine;

public class QuestBannerManager : MonoBehaviour
{    
    [SerializeField] private GameObject _sceneMediatorObj;
    private AdvertisementService _advertisementService;    
    private BannerBGScript _bannerBG;
    private Coroutine _activateBannerRoutine;
    private static bool _isFirstTimeStarted = true;
    //if advertisementService is null or advertising is disabled, then we will leave the method, otherwise we will enable the banner
    private void Start()
    {
        if(_isFirstTimeStarted && _sceneMediatorObj == null) {
            _isFirstTimeStarted = false;
            return;
        }
        _advertisementService = FindObjectOfType<AdvertisementService>();
        _bannerBG = FindObjectOfType<BannerBGScript>();
        if(_sceneMediatorObj == null){
            if(_advertisementService == null ||_advertisementService.IsADSDisabled) 
            {
                return;
            }else{
                _advertisementService.ShowBannerAd();
            }
        }
        else
        {
             if(_advertisementService == null ||_advertisementService.IsADSDisabled) 
                {
                    _sceneMediatorObj.GetComponent<IStartScenable>().StartScene();
                    return;
                }

             if(AdvertisementService.ISCasAdEnabled)
                {
                    _advertisementService.BannerCAS.Hide();
                    _bannerBG.Hide();
                    ShowInterstitialCASAd(_advertisementService);
        
                }
                else if (AdvertisementService.ISGoogleAdEnabled)
                {
                    _advertisementService.BannerGoogle.Hide();
                    ShowInterstitialGoogleAd(_advertisementService);
                }
        }
       
    }

    /// <summary>
    /// Вводимо рекламний сервіс [advertisementService]- показуємо рекламу типа "Interstitial" від "CAS sdk" 
    /// та додаємо/видаляємо ф-цію "StartScene" для "InterstitialCas"
    /// </summary>    
    private void ShowInterstitialCASAd(AdvertisementService advertisementService)
    {
        advertisementService.InterstitialCAS.Show(AdvertisementPlacement.InterpageCasAd);
        advertisementService.InterstitialCAS.SetOnClosedAction(GetCASStartScene());
    }

    /// <summary>
    /// Вводимо рекламний сервіс [advertisementService]- показуємо рекламу типа "Interstitial" від "AdMob" 
    /// та додаємо/видаляємо ф-цію "StartScene" для "InterstitialGoogle"
    /// </summary>
    private void ShowInterstitialGoogleAd(AdvertisementService advertisementService)
    {
        advertisementService.InterstitialGoogle.Show(AdvertisementPlacement.InterpageGoogleAd);
        advertisementService.InterstitialGoogle.SetOnClosedAction(GetStartScene());
    }

    /// <summary>
    /// Повертає ф-цію "StartScene" у викляді події
    /// </summary>    
    private Action GetStartScene()
    {
        return () => _sceneMediatorObj.GetComponent<IStartScenable>().StartScene(_advertisementService);
    }

    private Action GetCASStartScene()
    {
        return () => {
            _bannerBG.Show();
            _sceneMediatorObj.GetComponent<IStartScenable>().StartScene(_advertisementService);
            };
    }
}


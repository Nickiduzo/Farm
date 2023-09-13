using System;
using System.Collections;
using AwesomeTools.Sound;
using UnityEngine;
using UnityEngine.Networking;

public class AdsAPIManager : MonoBehaviour
{    
    private const string ANDROID_STRING = "android";
    private const string IOS_STRING = "ios";
    private const int WEB_REQUEST_TIME = 6;

    [SerializeField] AdvertisementService _service;
    [SerializeField] SoundSystem _soundSystem;
    [SerializeField] BannerBGScript _bannerBG;

    /// <summary>
    /// Виконує ф-цію "LoadTextFromServer"
    /// </summary>
    private void Awake() {
        _soundSystem.InitLevelMusic();
        StartCoroutine(LoadTextFromServer("https://farm.llill.xyz/api.json", new Action<string>(ActionWeb)));
    }

    /// <summary>
    /// Вводимо json строку [json] - виконує ф-цію "FindTurnOnOption" та визначає 2 параметр на основі операційної системи
    /// </summary>    
    private void ActionWeb(string json)
    {         
        try
        {
            #if UNITY_ANDROID
                    FindTurnOnOption(json, ANDROID_STRING);
            #elif UNITY_IOS
                    FindTurnOnOption(json, IOS_STRING);
            #else
                    FindTurnOnOption(json, ANDROID_STRING);
            #endif
            _service.Construct();
        }
        catch (System.Exception)
        {
            
        }
    }

    /// <summary>
    /// Вводимо json строку [json], назву операційної системи [OSTitle]-
    /// перевіряємо яке джерело реклами увімкнене та 
    /// присвоюємо в статичний параметр значення "true", якщо будь-яке джерело увімкнене 
    /// </summary>    
    private void FindTurnOnOption(string json, string OSTitle)
    {
        string AdsSourceString = "noAds";
        bool isEnd = false;
        string[] strings = json.Split(',', '{', '}', ':');

        for(int i = 0; i < strings.Length; i++)
        {
            strings[i] = strings[i].Trim();
        }

        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i].Contains(OSTitle))
            {
                i++;
                print("finded");
                int index = 0;
                while (index < 6)
                {
                    if (strings[i].Contains("1"))
                    {
                        AdsSourceString = strings[i - 1];
                        isEnd = true;
                        break;
                    }
                    else
                    {
                        i += 2;
                    }

                }
                if (isEnd) break;
            }        
        }
        AdsSourceString = AdsSourceString.Replace("\"", "");
        switch (AdsSourceString)
        {
            case  nameof(AdsSources.adMob):
                AdvertisementService.ISGoogleAdEnabled = true;
                break;
            case nameof(AdsSources.casSdk):
                AdvertisementService.ISCasAdEnabled = true;
                break;
        }

        _bannerBG.SetBGSize();
    }

    /// <summary>
    /// Вводимо посилання [url] та дію [response] - 
    /// спробуємо дістати json з посилання, потім передаємо в "responce" "request.downloadHandler.text"
    /// </summary>
    IEnumerator LoadTextFromServer(string url, Action<string> response)
    {        
        var request = UnityWebRequest.Get(url);
        request.timeout = WEB_REQUEST_TIME;

        yield return request.SendWebRequest();
        if (!request.isHttpError && !request.isNetworkError)
        {            
            response(request.downloadHandler.text);        
        }
        else
        {
            Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);       
            
        }

        request.Dispose();
    }
}
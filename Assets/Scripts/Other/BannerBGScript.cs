using UnityEngine;

public class BannerBGScript : MonoBehaviour
{
    private static Vector2 _bannerSize;

    /// <summary>
    /// Змінює розмір заднього фону реклами типа "Banner"
    /// </summary>
    public void SetBGSize()
    {
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        if(AdvertisementService.ISCasAdEnabled)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 275);
        }else if(AdvertisementService.ISGoogleAdEnabled){
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x ,100);
        }else{
            rectTransform.sizeDelta = Vector2.zero;
        }

        _bannerSize = rectTransform.sizeDelta;
    }

    /// <summary>
    /// Деактивує задній фон реклами типа "Banner"
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Активує задній фон реклами типа "Banner"
    /// </summary>
    public void Show(){
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Присвоює розмір фону після заходу в сцену
    /// </summary>
    void Start()
    {
        if(_bannerSize != null){
            transform.GetComponent<RectTransform>().sizeDelta = _bannerSize;
        }
    }
}

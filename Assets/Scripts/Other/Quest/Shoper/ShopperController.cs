using Quest;
using UnityEngine;
/// <summary>
/// Контроллер для покупателя
/// </summary>
public class ShopperController : MonoBehaviour
{
    [SerializeField] private ReloadTaskButton _reloadButton;
    private Shopper _currentShopper;
    public bool IsShopperExist => _currentShopper != null;


    /// <summary>
    /// Следит за тем, пришел ли покупатель
    /// </summary>
    /// <param name="shopper"></param>
    public void SetShopper(Shopper shopper)
    {
        _currentShopper = shopper;
        shopper.OnArrived += _reloadButton.SetInteractable;
        shopper.transform.SetParent(transform);
    }

    /// <summary>
    /// Вернёт текущий Shopper в SpawnFinishShopper
    /// </summary>
    /// <returns>возвращает текущий Shopper</returns>
    public Shopper GetShopper()
        => _currentShopper;
}

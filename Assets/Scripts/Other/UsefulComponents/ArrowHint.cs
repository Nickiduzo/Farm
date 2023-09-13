using DG.Tweening;
using UnityEngine;
/// <summary>
/// Класс подсказки в виде стрелки
/// </summary>
public class ArrowHint : MonoBehaviour
{
    [SerializeField] private bool _activateOnStart;

    /// <summary>
    /// Called on start to activate the arrow if specified
    /// </summary>
    private void Start()
    {
        if (_activateOnStart)
            ActivateArrow();
    }

    /// <summary>
    /// Activates the arrow, making it visible and applying a scaling animation
    /// </summary>
    public void ActivateArrow()
    {
        this.gameObject.SetActive(true);
        this.transform.DOScale(new Vector3(0.9f, 0.9f, 0.5f), 1f).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Hides the arrow by deactivating it
    /// </summary>
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

}

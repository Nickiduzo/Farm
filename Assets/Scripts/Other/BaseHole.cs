using DG.Tweening;
using UnityEngine;
using UsefulComponents;
/// <summary>
/// Uses to fill containers by editor extensions
/// </summary>
public class BaseHole : MonoBehaviour
{
    [SerializeField] protected Collider2D _selfCollider;

    protected IProgressCalculator _progressCalculator;

    /// <summary>
    /// starts smooth disappearance of hole
    /// </summary>
    /// <returns></returns>
    public Tween Disappear()
        => transform.DOScale(Vector3.zero, 0.5f).SetLink(gameObject);

    /// <summary>
    /// disable collider, delete all Tweens
    /// </summary>
    protected void MakeNonInteractable()
    {
        DOTween.Kill(gameObject);
        _selfCollider.enabled = false;
    }

    /// <summary>
    /// enable collider
    /// </summary>
    public void MakeInteractable()
        => _selfCollider.enabled = true;

    /// <summary>
    /// invoke [addProgress] in IProgressCalculator pass [deltaTime] parameter
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void AddProgress(float deltaTime)
    {
        _progressCalculator.AddProgress(deltaTime);
    }
    /// <summary>
    /// Включение эффекта Ground
    /// </summary>
    public virtual void PlayGroundFX()
    {
        FxSystem.Instance.PlayEffect("Ground", transform.position);
    }
    /// <summary>
    /// Включение эффекта Success
    /// </summary>
    public virtual void PlaySuccessFX()
    {
        FxSystem.Instance.PlayEffect("Success", transform.position);
    }
}
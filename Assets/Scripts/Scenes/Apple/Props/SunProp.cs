using DG.Tweening;
using UnityEngine;

public class SunProp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    // It sets the animation of the object and continuously rotates it
    private void Awake()
    {
        SetAnimation();
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 0, 180), 20).SetLoops(-1, LoopType.Incremental);
    }
    
    //Sets an animation
    private void SetAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(UnityEngine.Random.Range(1, 5));
        sequence.Append(_renderer.DOColor(Color.yellow, UnityEngine.Random.Range(1, 5)));
        sequence.AppendCallback(Restart);
    }


    //Restart an animation
    private void Restart() => SetAnimation();
}

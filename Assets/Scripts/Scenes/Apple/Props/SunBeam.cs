using DG.Tweening;
using UnityEngine;

public class SunBeam : MonoBehaviour
{
    //Init animations
    private void Awake()
    {
        SetAnimation();
    }

    //Sets an animation
    private void SetAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one, 1));
        sequence.AppendInterval(UnityEngine.Random.Range(1, 5));
        sequence.Append(transform.DOScale(new Vector3(1.3f, 1, 1), UnityEngine.Random.Range(1, 5)));
        sequence.AppendCallback(Restart);
    }

    //Restart an animation
    private void Restart() => SetAnimation();
}

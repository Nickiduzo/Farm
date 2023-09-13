using DG.Tweening;
using UnityEngine;

public class RotateHint : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _appearDuration;
    [SerializeField] private SpriteRenderer _sprite;

    // Tween the appearance of the object by fading in the sprite
    public Tween Appear()
        => _sprite.DOFade(1, _appearDuration);

    // Make the object disappear by fading out the sprite
    public void Disappear()
        => _sprite.DOFade(0, _appearDuration);

    // Rotate the object based on the given direction
    public void Rotate(Vector3 direction)
    {
        transform.Rotate(0, 0, -direction.magnitude * Time.deltaTime * _rotateSpeed);
    }

}
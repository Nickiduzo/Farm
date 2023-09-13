using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using AwesomeTools; 

public class Vermin : MonoBehaviour
{
    private const string SUCCESS = "Success";

    public event Action OnHit;

    [SerializeField] private MouseTrigger _mouseTrigger;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private FxSystem _fxSystem;
    [SerializeField] private SpriteRenderer[] _sprites;

    private Vector3 _flipedX = new Vector3(0, 180, 0);
    private Transform _treePoint;
    private Vector3 _startPos { get; set; }
    private SoundSystem _soundSystem { get; set; }

    // It subscribes to events
    private void Awake()
    {
        _mouseTrigger.OnDown += Die;
    }

    // Disables the collider, shows die visual effects, plays a sound, and moves the object back to its start position
    private void Die()
    {
        _collider.enabled = false;
        var sequence = DOTween.Sequence();
        sequence.AppendCallback(ShowDieFx);
        sequence.AppendCallback(PlaySound);
        sequence.AppendCallback(MoveBack);
    }

    // Shows die visual effects at the current position
    private void ShowDieFx()
        => _fxSystem.PlayEffect(SUCCESS, transform.position);

    // Moves the object back to its start position after the die effect
    private void MoveBack()
    {
        transform.DOKill();
        OnHit?.Invoke();
        var sequence = DOTween.Sequence();
        Flip(_startPos);
        sequence.Append(transform.DOMove(_startPos, 2f));
        sequence.AppendCallback(() => gameObject.SetActive(false));
    }

    // Flips the object horizontally by changing its rotation
    public void Flip()
    {
        if (transform.rotation == Quaternion.Euler(Vector3.zero))
        {
            transform.rotation = Quaternion.Euler(_flipedX);
        }
        else
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    // Flips the object based on its destination position
    private void Flip(Vector3 destination)
    {
        transform.rotation = Quaternion.Euler((transform.position - destination).x > 0 ? _flipedX : Vector3.zero);
    }

    // Plays a sound effect.
    private void PlaySound() => _soundSystem.PlaySound(SUCCESS);

    // Constructs
    public void Construct(Vector3 start, Vector3 end, SoundSystem soundSystem,Transform treePoint)
    {
        _startPos = start;
        _soundSystem = soundSystem;
        _treePoint = treePoint;
        
        transform.position = _startPos;
        transform.DOMove(GetRandomPositionAroundTree(), UnityEngine.Random.Range(10, 12)).onComplete += SurroundTheTree;
    }

    // Returns a random position around the tree by applying random offsets to the tree point's position
    private Vector3 GetRandomPositionAroundTree()
    {
        var randomOffsetX = UnityEngine.Random.Range(-3f, 3f);
        var randomOffsetY = UnityEngine.Random.Range(-3f, 3f);
        return _treePoint.position + new Vector3(randomOffsetX, randomOffsetY);
    }

    // Moves the object to a random position around the tree and flips it based on the destination position
    private void SurroundTheTree()
    {
        var destination = GetRandomPositionAroundTree();
        Flip(destination);
        transform.DOMove(destination, UnityEngine.Random.Range(3, 4)).onComplete += SurroundTheTree;
    }

    // Sets new sortingOrder for all sprites
    public void SenNewSortingOrder(int index)
    {
        foreach (var sprite in _sprites)
        {
            sprite.sortingOrder += index;
        }
    }
}

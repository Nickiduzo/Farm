using System;
using System.Collections;
using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;

namespace ChickenScene.Entities
{
  public class ChickenBasket : MonoBehaviour,IDisposable
  {
    private const string HINT_SOUND = "HintSound";

    [SerializeField] private float _movingDuration;
    [SerializeField] private EggTriggerObserver _observer;
    [SerializeField] private SpriteRenderer _frontBasketSprite;
    [SerializeField] private ChickenBasketMover _chickenBasketMover;
    [SerializeField] private Transform[] _eggStorePositions;
    [SerializeField] private Vector2 _basketXConstrain;
    
    public event Action OnArrived;

    private int _storedEggCount;
    private Vector3 _destination; // point in center scene
    private Vector3 _spawnPoint; // spawn point beyond scene
    private SoundSystem _soundSystem;

    
    // set up variables and parameters
    public void Construct(Vector3 destination, Vector3 startPoint, SoundSystem soundSystem,InputSystem inputSystem)
    {
      _soundSystem = soundSystem;
      _destination = destination;
      _spawnPoint = startPoint;
      
      Sequence sequence = DOTween.Sequence();
      sequence.Append(MoveToDestination()).OnComplete(() => BasketArrivedToStartPoint());
      
      
      _chickenBasketMover.Construct(inputSystem, _basketXConstrain);
      _chickenBasketMover.OnDragStart += DisableHint;
      _observer.OnTriggerEnter += StoreEgg;
    }

    // basket appears on scene with hint, and becomes available for dragging
    private void BasketArrivedToStartPoint()
    {
      OnArrived?.Invoke();
      _chickenBasketMover.IsDraggable = true;
      _soundSystem.PlaySound(HINT_SOUND);
      HintSystem.Instance.ShowPointerHint(transform.position);
    }

    // if this is first tap to basket, disable hint
    private void DisableHint()
      => HintSystem.Instance.HidePointerHint();

    // basket arrives at center of scene
    private Tween MoveToDestination()
      => transform.DOMove(_destination, _movingDuration);

    // basket of eggs leave scene  
    private Tween MoveToSpawnPoint()
      => transform.DOMove(_spawnPoint, _movingDuration);

    // if basket catches egg, make it part of basket
    private void StoreEgg(Egg egg)
    {
      if (_storedEggCount >= _eggStorePositions.Length)
      {
        return;
      }
      
      MakeEggChildOfBasket(egg);
      egg.StoreProcess(GetStorePosition());
      _storedEggCount++;
    }

    // update store poitn for next egg
    private Vector3 GetStorePosition() => _eggStorePositions[_storedEggCount].localPosition;
    
    // make egg child object of basket and update sortingOrder for egg sprite
    private void MakeEggChildOfBasket(Egg egg)
    {
      egg.transform.SetParent(transform);
      DOVirtual.DelayedCall(.1f, () =>
      {
        egg.SpriteRenderer.sortingOrder = _frontBasketSprite.sortingOrder - 1;
      });
    }

    // if all eggs are collected in basket, basket leaves scene
    public void AllEggsCollected()
      => StartCoroutine(LeaveScene());

    // make basket unavailable for dragging and moves to spawn point
    private IEnumerator LeaveScene()
    {
      _chickenBasketMover.IsDraggable = false; 
       
      yield return new WaitForSeconds(0.5f);
      
      var sequence = DOTween.Sequence();
      sequence.Append(MoveToSpawnPoint());
    }

    public void Dispose()
    {
      _chickenBasketMover.OnDragStart -= DisableHint;
      _observer.OnTriggerEnter -= StoreEgg;
    }
  }
}

using System;
using System.Collections;
using DG.Tweening;
using AwesomeTools.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChickenScene.Entities
{
    public class Egg : MonoBehaviour
    {
        private const string FALL_IN_BASKET = "SuccessEgg";

        [SerializeField] private Collider2D _collider;
        [SerializeField] private EggVisual[] _eggVisuals;

        public SpriteRenderer SpriteRenderer { get; private set; }
        
        private float _dropSpeed = 1.5f;
        private float _storeDuration = 0.1f;
        private float _delayToFade = 1.5f;
        private int _rotationDirection;
        private int _rotationAngle;
        
        private Vector2 _startPosiniton;
        private Vector2 _dropPosition;
        private EggVisual _currentEggVisual; 
        private Sprite _brokenEggSprite;
        private Sprite _defaultEggSprite;
        private SoundSystem _soundSystem;
        private Transform _transformEggVisual;

        public event Action OnEggStored;


        // start drop egg and set up variables and parameters
        public void Construct(Vector2 startPosiniton,SoundSystem soundSystem, float _dropPosOffSetY)
        {
            SetUpEggSprite();
            _collider.enabled = true;
            _startPosiniton = startPosiniton;
            _soundSystem = soundSystem;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.position = _startPosiniton;
            EggDrop(_dropPosOffSetY);
        }

        // set all components to default and choose random sprite for EggVisual
        private void SetUpEggSprite()
        {
            var eggVisual = _eggVisuals[Random.Range(0, _eggVisuals.Length)];
            _currentEggVisual = Instantiate(eggVisual, transform.position, Quaternion.identity, transform);
            _transformEggVisual = transform.GetChild(0);
            SpriteRenderer = _currentEggVisual.SpriteRenderer;
            _defaultEggSprite = _currentEggVisual.SpriteRenderer.sprite;
            _brokenEggSprite = _currentEggVisual.BrokenEgg;
        }
        
        // egg starts to fall to floor and rotate, also set up [_dropPosition]
        private void EggDrop(float _dropPosOffSetY)
        {
            EggRotate();
            _dropPosition = new Vector2(_startPosiniton.x, _startPosiniton.y - _dropPosOffSetY);
            transform.DOMove(_dropPosition, _dropSpeed).SetEase(Ease.Linear).OnComplete(EggBroke);
        }

        // set in which direction egg will rotate during fall
        private void EggRotate() 
        {
            _rotationDirection = Random.Range(0, 2) * 2 - 1;
            _rotationAngle = Random.Range(0, 2) * 2 - 1 * 90;
            transform.DOLocalRotate(new Vector3(0, 0, _rotationAngle * _rotationDirection), _dropSpeed, RotateMode.FastBeyond360);
        }

        // egg fell to floor and broke, so we change sprite, disable collider, play "BreakingEgg" sound and launch [FadeEggDelay()]
        private void EggBroke()
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _transformEggVisual.localScale *= 1.5f;
            SpriteRenderer.sprite = _brokenEggSprite;
            _soundSystem.PlaySound("BreakingEgg");
            _collider.enabled = false;
            StartCoroutine(FadeEggDelay());
        }

        // start smooth disappearance of broken egg 
        private IEnumerator FadeEggDelay()
        {
            yield return new WaitForSeconds(_delayToFade);
            SpriteRenderer.DOFade(0, 1).OnComplete(() => BrokenEggReset());
        }

        // egg is caught in basket
        public void StoreProcess(Vector3 storePosition)
        {
            MoveToBasket(storePosition);
            OnEggStored?.Invoke();
            _collider.enabled = false;
        }

        private void MoveToBasket(Vector3 storePosition)
        {
            transform.DOKill();
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            PlaySuccessEffect();
            var newStorePosition = new Vector2(transform.localPosition.x, storePosition.y);
            var sequence = DOTween.Sequence();
            StartCoroutine(PlayFallSound());
            sequence.Append(transform.DOLocalMove(newStorePosition, _storeDuration));
            sequence.Append(SpriteRenderer.DOFade(0, 0));
            sequence.Append(transform.DOLocalMove(storePosition, 0));
            sequence.Append(SpriteRenderer.DOFade(1, 0.5f));
        }

        /// <summary>
        /// Вмикає звук приземлення шерсті трохи раніше перед приземленням
        /// </summary>        
        private IEnumerator PlayFallSound()
        {
            yield return new WaitForSeconds(_storeDuration - .2f);
            _soundSystem.PlaySound(FALL_IN_BASKET);
            StopCoroutine(PlayFallSound());
        }
        private void PlaySuccessEffect()
        {
            FxSystem.Instance.PlayEffect("Success",transform.position);
            _soundSystem.PlaySound("Success");
        }
        
        // return default egg sprite (we do this so that next egg appears with default egg sprite)
        private void BrokenEggReset()
        {
            SpriteRenderer.sprite = _defaultEggSprite;
            gameObject.SetActive(false);
            Destroy(_currentEggVisual.gameObject);
        }
    }
}

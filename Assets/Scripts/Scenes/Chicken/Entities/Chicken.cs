using System;
using DG.Tweening;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;

namespace ChickenScene.Entities
{
    public class Chicken : MonoBehaviour,IDisposable
    {
        private const string Success = "Success";
        [SerializeField] private ChickenEyesAnimator _chickenEyesAnim;
        [SerializeField] private Animator _anim;
        [SerializeField] private WormTriggerObserver _wormTrigger;
        [SerializeField] private Transform _jaw;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private FxSystem _fxSystem;
    
        private float _value = -1;
        private bool _canEatAgain;
        private bool _canEate;
        public event Action OnWormEaten;
    
        // set up all parameters, components and monitor whether worm has been picked
        public void Consruct()
        {
            _canEate = true;
            _canEatAgain = false;
            _anim = GetComponent<Animator>();
            _anim.SetBool("ChikenEat", false);
            
            _wormTrigger.OnTriggerEnter += WormEating;
            WormChicken.OnWormPickedUp += IncreaseAnimValue;
            WormChicken.OnWormDroped += ReduceAnimValue;
        }

        // worm was caught and eaten by chicken
        private void WormEating(WormChicken wormChicken)
        {
            if(!_canEate)
                return;
        
            _canEate = false;
            HintSystem.Instance.HidePointerHint();
            wormChicken.Disappearing(_jaw.position); 
            EatingAnimAndSuccessFx();
        }

        // after calling "CanStillEat" chickens are ready to eat new 4 worms again
        public void CanStillEat()
        {
            _canEate = true;
            DOVirtual.DelayedCall(1f, () =>
            {
                ReturnIdleAnim();
            });
        }

        // changing [_value] in Animator
        private void Update()
            => _anim.SetFloat("Blend", _value); 

        // set default chicken position
        private void ReturnIdleAnim()
        {
            _anim.CrossFade("IdleAndChcikenWantToEat", 0.4f);
            _anim.SetBool("ChikenEat", false);
        }

        // (invoke when we take worm) chicken's head is raised and waiting to eat a worm
        public void IncreaseAnimValue()
        {
            DOTween.To(() => _value, x => _value = x, 1f, 1f);
            if(_canEate == true)
            {
                _chickenEyesAnim.LookToCenter();
            }
        }
        
        // (invoke when we put worm) chicken's head is down and waiting for us to take worm
        public void ReduceAnimValue()
        {
            DOTween.To(() => _value, x => _value = x, -1f, 1f);
            _chickenEyesAnim.MoveToDefaultEyesPosition();
        }
        
        // Enable eating animation and invoke action [OnWormEaten] that worm have been eaten and WormSpawner can spawn another worm,
        // after this method, chicken will not raise its head until method [CanStillEat()] is called
        private void EatingAnimAndSuccessFx()
        {
            _chickenEyesAnim.MoveToDefaultEyesPosition();
            _anim.SetBool("ChickenIdle", true);
            if(_canEatAgain == true)
            {
                _anim.SetTrigger("ChikenEatAgain");
            }
            _canEatAgain = true;

            DOVirtual.DelayedCall(0.3f, () =>
            {
                _anim.SetBool("ChikenEat", true);
                _fxSystem.PlayEffect(Success, transform.position);
                _soundSystem.PlaySound(Success);
            }).OnComplete(() => OnWormEaten?.Invoke());
        }

        // enable anim "SpawnEgg" and choose right color for feathers efect
        public void SpawnEggAnimAndFeathersFx(int index)
        {
            _anim.CrossFade("SpawnEgg", 0.3f);
            _soundSystem.PlaySound("Crowing");
            _chickenEyesAnim.LookToCenter();
            DOVirtual.DelayedCall(1.2f, () =>
            {
                _chickenEyesAnim.MoveToDefaultEyesPosition();
            });
            
            switch (index)
            {
                case 0:
                    _fxSystem.PlayEffect("Brown", transform.position);
                    break;
                case 1:
                    _fxSystem.PlayEffect("White", transform.position);
                    break;
                case 2:
                    _fxSystem.PlayEffect("Gray", transform.position);
                    break;
                case 3:
                    _fxSystem.PlayEffect("Orange", transform.position);
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            _wormTrigger.OnTriggerEnter -= WormEating;
            WormChicken.OnWormPickedUp -= IncreaseAnimValue;
            WormChicken.OnWormDroped -= ReduceAnimValue;
        }
    }
}
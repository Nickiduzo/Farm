using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using UnityEngine.Events;
using AwesomeTools;

namespace Carrot
{
    public class Mole : MonoBehaviour
    {
        private const float MoleHandsScale = 0.9f;
        
        private const string Success = "Success";
        public event Action OnCatch;
        public event UnityAction<Mole> OnDied;
        public event UnityAction<Mole> OnHied;
        public bool IsRegistered { get; set; }

        [SerializeField] private Collider2D _collider;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private float _appearDuration;
        [SerializeField] private float _appearOffsetMoving;
        [SerializeField] private Animation _idleAnimation;
        [SerializeField] private GameObject _deadEye;
        [SerializeField] private Animation _deadEyeAnim;
        [SerializeField] private GameObject _deadFX;
        [SerializeField] private GameObject _hands;
        [SerializeField] private SpriteRenderer _body;
        [SerializeField] private Sprite _deadBodySprite;
        [SerializeField] private Sprite _moleBodySprite;

        [SerializeField] private GameObject _eyeSprite;
        [SerializeField] private GameObject _closeEyeSprite;
        [SerializeField] private Vector2 _minMaxHideDelay;

        private int _spawnPointIndex;
        private bool _isInit;
        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;
        private Tween _hideTween;
        private Vector3 _scale;

        public int SpawnPointIndex => _spawnPointIndex;

        // set up [soundSystem] and [fxSyste]
        public void Construct(ISoundSystem soundSystem, FxSystem fxSystem)
        {
            if (_isInit) return;

            _isInit = true;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
        }

        // set spawn point and mole scale
        public void SetSpawnPointIndexAndScale(int spawnPointIndex, float scale)
        {
            _scale = new Vector3(scale, scale, scale);
            _spawnPointIndex = spawnPointIndex;
        }

        // monitor whether mole is dead ([Die] invoke when we tap on mole)
        private void Awake()
        {
            transform.localScale = Vector3.zero;
            _mouseTrigger.OnDown += Die;
        }

        private void OnDestroy()
        {
            _hideTween.Kill();   
            _mouseTrigger.OnDown -= Die;
        }

        // appear mole, set mole sprite and scale, play eyes anim, set delay before hiding
        public Tween Appear()
        {
            _body.sprite = _moleBodySprite;
            _hands.transform.position = Vector3.zero;
            _hands.transform.localScale = Vector3.one;
            _deadEye.SetActive(false);
            OffDeadFx();
            _deadEyeAnim.clip.SampleAnimation(_deadEyeAnim.gameObject, 0f);
            _deadEyeAnim.Stop();
            _eyeSprite.SetActive(true);
            _collider.enabled = true;
            transform.DOMoveY(transform.localPosition.y - _appearOffsetMoving, _appearDuration);
            var appearTween = transform.DOScale(_scale, _appearDuration);
            appearTween.OnComplete(() =>
            {
                var sequence = DOTween.Sequence();
                sequence.Append(_hands.transform.DOScaleY(1f, 0.2f));
                sequence.Append(DOVirtual.DelayedCall(UnityEngine.Random.Range(_minMaxHideDelay.x, _minMaxHideDelay.y), Hide));
                sequence.Join(transform.DOLocalMoveY(transform.localPosition.y + 0.05f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo));
                sequence.Join(_hands.transform.DOLocalMoveY(_hands.transform.localPosition.y - 0.05f, 1f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo));

                _hideTween = sequence;

            });

            return appearTween;
        }

        // play sound effect based on name of sound
        private void PlaySound(string name)
            => _soundSystem.PlaySound(name);

        // mole move to down
        private Tween MoveOff()
            => transform.DOMoveY(transform.localPosition.y - _appearOffsetMoving * 10f, _appearDuration);

        // smooth downscaling to zero for body and hands
        private Tween Disappear()
        {
            var sequence = DOTween.Sequence();
            
            sequence.Append(transform.DOScale(Vector3.zero, _appearDuration));
            sequence.Join(_hands.transform.DOScale(0f, 0.2f));;

            return sequence;
        }

        // disable dead eye sprites
        private void OffDeadFx()
            => _deadFX.SetActive(false);

        // set dead mole sprite, turn on stars FX above mole, play "Success" and "Put" sounds, enable "deadEyesAnim", 
        // after all these actions, mole move to under hole and disappear/disable
        private void Die()
        {
            if (_hideTween != null && _hideTween.IsActive())
            {
                _hideTween.Kill();
            }

            _body.sprite = _deadBodySprite;
            HideRenderer();
            OnCatch?.Invoke();
            _deadEye.SetActive(true);
            _deadFX.SetActive(true);
            _deadEyeAnim.Play();
            PlaySound(Success);
            PlaySound("Put");

            var sequence = DOTween.Sequence();
            sequence.AppendCallback(ShowDieFx);
            sequence.Join(Stretch());
            sequence.AppendCallback(OffDeadFx);
            sequence.Join(_hands.transform.DOScale(MoleHandsScale, 0.5f));
            sequence.Append(MoveOff());
            sequence.Join(Disappear());
            sequence.AppendCallback(DisableObject);
        }
        
        // a bit shake mole from side to side
        private Tween Stretch()
        {
            var sequence = DOTween.Sequence();
            sequence.Join(transform.DOLocalMoveX(transform.localPosition.x - 0.08f, 0.3f)
                .SetEase(Ease.InOutSine)
                .SetLoops(4, LoopType.Yoyo));
            sequence.Join(_hands.transform.DOLocalMoveX(_hands.transform.localPosition.x + 0.08f, 0.3f)
                .SetEase(Ease.InOutSine)
                .SetLoops(4, LoopType.Yoyo));

            return sequence;
        }

        private void ShowDieFx()
            => _fxSystem.PlayEffect("Success", transform.position);

        // invoke [OnDied] that mole is dead and disable it
        private void DisableObject()
        {
            gameObject.SetActive(false);
            OnDied?.Invoke(this);
        }

        // start hiding under hole
        public void Hide()
        {
            Debug.Log("Hiding object");
            if (_hideTween != null && _hideTween.IsActive())
            {
                _hideTween.Kill();
            }
            var sequence = DOTween.Sequence();
            sequence.Join(_hands.transform.DOScale(MoleHandsScale, 0.5f));
            sequence.Append(MoveOff());
            sequence.Join(Disappear());
            sequence.AppendCallback(HideObject);
        }

        // disable eyes sprites and collider, stop idle anim
        private void HideRenderer()
        {
            _idleAnimation.Stop();
            _closeEyeSprite.SetActive(false);
            _eyeSprite.SetActive(false);
            _collider.enabled = false;
        }

        // invoke [OnHide] and disable mole as gamobject
        private void HideObject()
        {
            HideRenderer();
            gameObject.SetActive(false);
            OnHied?.Invoke(this);
        }
    }
}
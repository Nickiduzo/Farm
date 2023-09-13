using System;
using DG.Tweening;
using UnityEngine;
using AwesomeTools.Sound;
using AwesomeTools;

namespace SunflowerScene
{
    public class SunflowerHead : MonoBehaviour, ICountable<Transform>
    {
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private SpriteRenderer _seedsRender;
        [SerializeField] private SoundSystem _soundSystem;
        private Vector3 _scale;
        private Tween _pulsing;
        public event Action<Transform> CountUp;

        public void Construct(SoundSystem soundSystem)
        {
            _soundSystem = soundSystem;
        }
        private void Awake()
        {
            _scale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        public void Activate()
        {
            _mouseTrigger.OnDown += Drop;
            _rigidbody2D.simulated = true;
            _pulsing = transform.DOScale(_scale * 1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        private void Drop()
        {
            _soundSystem.PlaySound("sunflower_seeds");
            _mouseTrigger.OnDown -= Drop;
            OnDropped();
            _pulsing.Kill();
            transform.DOScale(_scale, 1f).SetLink(gameObject);
            _seedsRender.DOFade(0, .5f).SetLink(_seedsRender.gameObject);
            
        }

        public void Appear()
        {
            transform.DOScale(_scale, 0.1f).SetLink(gameObject);
        }

        private void OnDropped()
        {
            CountUp?.Invoke(transform);
        }

        private void OnDisable()
        {
            _mouseTrigger.OnDown -= Drop;
        }
    }
}
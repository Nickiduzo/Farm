using DG.Tweening;
using Fx;
using System;
using UnityEngine;
using AwesomeTools;

namespace Apple
{
    public class Mole : MonoBehaviour
    {
        public event Action OnCatch;
        public bool IsRegistered { get; set; }

        [SerializeField] private Collider2D _collider;
        [SerializeField] private Sprite _deathSprite;
        [SerializeField] private Sprite _aliveSprite;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private Effect _deathFx;
        [SerializeField] private float _appearDuration;

        private Vector3 _cachedScale;

        private void Awake()
        {
            transform.localScale = Vector3.zero;
            _mouseTrigger.OnDown += Die;
        }

        private void OnDestroy()
            => _mouseTrigger.OnDown -= Die;

        public Tween Appear()
        {
            GetComponent<SpriteRenderer>().sprite = _aliveSprite;
            _collider.enabled = true;
            return transform.DOScale(Vector3.one, _appearDuration);
        }


        private Tween Disappear()
            => transform.DOScale(Vector3.zero, _appearDuration);

        private void Die()
        {
            GetComponent<SpriteRenderer>().sprite = _deathSprite;
            _collider.enabled = false;
            var sequence = DOTween.Sequence().SetLink(gameObject);
            sequence.AppendCallback(ShowDieFx);
            sequence.Append(Stretch());
            sequence.Append(Disappear());
            sequence.AppendCallback(DisableObject);
        }

        private Tween Stretch()
            => transform.DOShakeScale(0.5f, 0.3f, 1, 45).SetEase(Ease.InOutBack);

        private void ShowDieFx()
            => Instantiate(_deathFx, transform.position, Quaternion.identity);

        private void DisableObject()
        {
            gameObject.SetActive(false);
            OnCatch?.Invoke();
        }
    }
}
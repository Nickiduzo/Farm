using DG.Tweening;
using System;
using UnityEngine;

namespace SunflowerScene
{
    public class SunflowerHole : BaseHole, ICountable<Transform>
    {
        [SerializeField] private Transform _carStopPoint;
        [SerializeField] private SpriteRenderer _spriteRenderer,_endHole;
        [SerializeField] private Sprite _digHoleSprite;
        [SerializeField] private Transform _holeRenderTransform;

        private Rigidbody2D _rigidbody2D;
        private bool _planted;
        public Vector3 CarStopPoint => _carStopPoint.position;
        public Transform _storePosition;

        public bool Planted => _planted;
        
        public event Action<Transform> CountUp;

        [SerializeField] private bool layerOrderChange;
        [SerializeField] private int layerInt;
        //rigidbody2D simulated sets false
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            SetAvailableStore(false);
        }

        // Processes the hole by making it non-interactable, assigning a sunflower as its child,
        // changing its sprite to a dig hole sprite, and triggering the hole planted event.
        public void ProcessHole(Sunflower sunflower)
        {
            MakeNonInteractable();
            sunflower.transform.SetParent(gameObject.transform);
           
            _spriteRenderer.sprite = _digHoleSprite;
            if (layerOrderChange)
            {
                _spriteRenderer.DOFade(0, 0.5f);;
                _endHole.sortingOrder = layerInt;
                _endHole.DOFade(1, 0.6f);
                _endHole.transform.DOScale(.55f, .3f);
            }
            OnHolePlanted();
        }

        // Performs the digging animation for the hole by scaling down its render transform.
        public void DigHole()
        {
            _holeRenderTransform.DOScale(_holeRenderTransform.localScale * 0.7f, 0.2f);
        }

        // Sets the availability of the hole for storing a sunflower based on the specified value.
        public void SetAvailableStore(bool value)
        {
            if (_planted)
            {
                Debug.LogWarning("Hole already store");
            }

            _rigidbody2D.simulated = value;
        }

        // Triggers the hole planted event, passing the transform of the hole.
        private void OnHolePlanted()
        {
            CountUp?.Invoke(transform);
        }
    }
}
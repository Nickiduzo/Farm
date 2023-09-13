using System;
using System.Collections.Generic;
using DG.Tweening;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;

namespace Tomato
{
    public class TomatoHole : BaseHole, IDiggable
    {
        private const string Success = "Success";
        public event Action OnHoleReadyToSeed;
        public Vector3 StorePosition => _storePosition.position;
        public Vector3 MoveToHolePosition => _moveToHolePosition.position;

        [SerializeField] private SpriteRenderer _dirt;
        [SerializeField] private List<Sprite> _dirtSprites;
        [SerializeField] private List<Transform> _tomatoAppearPoints;
        [SerializeField] private Transform _storePosition;
        [SerializeField] private Transform _moveToHolePosition;
        [SerializeField] private float _framesToFill;
        [SerializeField] private SpriteRenderer _tomatoTreeDirtSprite; 
        
        private List<TomatoSeed> _storedSeeds = new();
        
        [SerializeField] private SoundSystem _soundSystem;

        private List<Vector3> _tomatoAppearPositions = new();
        private int _currentSprite;
        private IChainAction _action;
        private TomatoSeed _tomatoSeed;

        public bool IsDig { get; set; }
        

        /// <summary>
        /// викликає ф-ції CalculateTomatoSpawnPoints(), InitActions(), SetDefaultDirt() та
        /// створює калькулятор прогресу [_progressCalculator]
        /// </summary>
        private void Awake()
        {
            CalculateTomatoSpawnPoints();
            InitActions();
            SetDefaultDirt();
            _progressCalculator = new FrameProgressCalculator(CalculateMaxProgress());
        }

        /// <summary>
        /// Присвоює події
        /// </summary>
        private void InitActions()
        {
            DirtHalfScale.HalfScaleOn = true;
            _action = new DirtHalfScale(_dirt);
            _action.SetNextAction(new DirtDisappear(_dirt, this));
        }

        /// <summary>
        /// Додає позиції появи томатів [_tomatoAppearPositions]
        /// зі списку точок появи томатів [_tomatoAppearPoints]
        /// </summary>
        private void CalculateTomatoSpawnPoints()
        {
            foreach (var point in _tomatoAppearPoints)
                _tomatoAppearPositions.Add(point.position);
        }

        /// <summary>
        /// Вводимо саженець томату [seed] - присвоюємо саженець томату до списку саженців томатів [_storedSeeds]
        /// </summary>
        /// <param name="seed">саженець томату</param>
        public void StoreSeed(TomatoSeed seed)
        {
            SuccessEffect();
            MakeNonInteractable();

            _storedSeeds.Add(seed);

            _tomatoSeed = seed;
        }

        /// <summary>
        /// Закриває яму землею
        /// </summary>
        public Tween CloseHole(){ 
            return _tomatoTreeDirtSprite.DOFade(1, .6f);
        }
        /// <summary>
        /// Забороняє взаємодію з ямою та викликає подію "OnHoleReadyToSeed"
        /// </summary>
        public void Dig()
        {
            MakeNonInteractable();
            OnHoleReadyToSeed?.Invoke();
            IsDig = true;
        }

        /// <summary>
        ///Вводимо подію [onFinishCallback] - змінює спрайт ями [_dirt]
        /// </summary>
        public void ChangeSprite()
        {
            if (_currentSprite < _dirtSprites.Count - 2)
            {
                _currentSprite++;
                _dirt.sprite = _dirtSprites[_currentSprite];
            }
            else
            {
                _dirt.sprite = _dirtSprites[^1];
            }
        }
        
        /// <summary>
        /// Перевіряє чи у ями [_dirt] останній спрайт ями [_dirtSprites[^1]]
        /// </summary>
        public bool IsThisLastSprite()
        {
            return _dirt.sprite == _dirtSprites[^1];
        }

        /// <summary>
        /// Присвоює початковий спрайт ями  [_dirtSprites[0]] ямі [_dirt]
        /// </summary>
        private void SetDefaultDirt()
        {
            _dirt.sprite = _dirtSprites[_currentSprite];
        }

        /// <summary>
        /// Викликає ефект успіху
        /// </summary>
        private void SuccessEffect()
        {
            _soundSystem.PlaySound(Success);
            PlaySuccessFX();
        }

        /// <summary>
        /// Повертає прогрес
        /// </summary>
        private float CalculateMaxProgress()
            => Time.deltaTime * _framesToFill;

        /// <summary>
        /// Перевіряє чи значення саженцця помідорів [_tomatoSeed] пусте 
        /// </summary>
        public bool IsEmpty()
            => _tomatoSeed == null;

    }
}
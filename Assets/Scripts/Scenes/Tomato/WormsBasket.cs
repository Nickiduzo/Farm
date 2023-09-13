using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UI;
using UnityEngine;
using UsefulComponents;
using Random = UnityEngine.Random;

namespace Tomato
{
    public class WormsBasket : MonoBehaviour, IProgressWriter
    {
        private const int WORM_COUNT_THAT_NEED_EARLY_DEACTIVATE_ANIMATION = 2;
        private const string Success = "Success";
        private const string OPEN = "Open";
        private const string CLOSE = "Close";

        public event Action OnProgressChanged;
        public event Action OnCatchAllWorms;
        public static event Action OnOneWormLeft;
        public int CurrentProgress { get; private set; }
        public int MaxProgress => _config.WormsToSpawn;

        [SerializeField] private float _storeOffSetX;
        [SerializeField] private float _storeOffSetY;
        [SerializeField] private TomatoLevelConfig _config;
        [SerializeField] private MoveStartDestination _move;
        [SerializeField] private Transform _storePoint;
        [SerializeField] private Transform _preStorePoint;
        [SerializeField] private WormsTriggerObserver _trigger;
        [SerializeField] private WormJarCover _cover;

        private int _indexSortingOrder = 13;
        private float _step;
        private ISoundSystem _soundSystem;
        private int _wormCount = 0;
        [SerializeField] private Transform _firstWormTrans;

        /// <summary>
        /// Вводимо систему звуку [soundSystem]- Запам'ятовуємо систему звуку в полі "_soundSystem"
        /// </summary>        
        public void Construct(ISoundSystem soundSystem)
            => _soundSystem = soundSystem;

        /// <summary>
        /// Присвоюємо ф-ції до подій
        /// </summary>
        private void Awake()
        {
            _cover.OnClosed += MoveToStart;
            _move.MoveToDestinationCompleted += Open;
            _trigger.OnTriggerEnter += MoveToPreStorePoint;
        }

        /// <summary>
        /// Видаляємо ф-ції з подій
        /// </summary>
        private void OnDestroy()
        {
            _cover.OnClosed -= MoveToStart;
            _move.MoveToDestinationCompleted -= Open;
            _trigger.OnTriggerEnter -= MoveToPreStorePoint;
        }

        /// <summary>
        /// Відчиняє кришку банки [_cover]
        /// </summary>
        public void Open()
        {
            _soundSystem.PlaySound(OPEN);
            _cover.Open();
        }
        
        /// <summary>
        /// Зачиняє кришку банки [_cover]
        /// </summary>
        public void Close()
        {
            _soundSystem.PlaySound(CLOSE);
            _cover.Close();
        }
        
        /// <summary>
        /// Вводимо позицію [at] - показуємо ефект успіху в позиції
        /// </summary>
        private void SuccessEffect(Vector3 at)
        {
            _soundSystem.PlaySound(Success);
            FxSystem.Instance.PlayEffect(0, at);
        }

        /// <summary>
        /// Вводимо черв'яка [worm]- 
        /// забороняє взаємодію з черв'яком та переміщує до точки над банкою [_preStorePoint]
        /// </summary>        
        private void MoveToPreStorePoint(Worm worm)
        {            
            ///worm.GetComponent<SpriteRenderer>().sortingOrder = 13;
            HalfScaleWorm(worm);
            worm.MakeNonInteractable();
            worm.ChangeDragAnimation(true);
            worm.transform.DOMove(_preStorePoint.position, 1)
                .OnComplete(() => 
                {
                    MakeVisualChildOfBasket(worm);
                    ProcessWorm(worm);
                });
        }
        
        /// <summary>
        /// Вводимо черв'яка [worm] - викликає ф-цію переміщення черв'яка до точки складування [_firstWormTrans.position]
        /// </summary>        
        private void ProcessWorm(Worm worm)
        {
            HintSystem.Instance.HidePointerHint();
            if (_wormCount < WORM_COUNT_THAT_NEED_EARLY_DEACTIVATE_ANIMATION)
            {
                DOTween.Sequence().Append(
                    worm.transform.DOMove(_firstWormTrans.position, .5f))
                                  .OnComplete(() => {worm.ChangeDragAnimation(false); TweenMoveToBasket(worm);});
                _wormCount++;
                return;
            }
            TweenMoveToBasket(worm);
        }

        /// <summary>
        /// Вводимо черв'яка [worm] - викликає ефект успіху після переміщення до точки складування
        /// </summary>
        private void TweenMoveToBasket(Worm worm)
        {
            MoveToBasket(worm)
                .OnComplete(() =>
                {
                    worm.ChangeDragAnimation(false);
                    SuccessEffect(worm.transform.position);
                    NextStep();

                    CurrentProgress++;
                    OnProgressChanged?.Invoke();
                    
                    if(CurrentProgress == 4)
                    {
                        OnOneWormLeft?.Invoke();
                    }

                    if (!IsEnoughWorms()) return;

                    Close();
                    worm.StopCrawlingSound();
                });
        }

        /// <summary>
        /// Переміщує елемент до точки появи
        /// </summary>
        private void MoveToStart()
        {
            _move.MoveToStart()
                .OnComplete(() => OnCatchAllWorms.Invoke());
        }

        /// <summary>
        /// Вводимо черв'яка [worm] - зменшує розмір черв'яка 
        /// </summary>        
        private void HalfScaleWorm(Worm worm)
            => worm.transform.DOScale(worm.transform.localScale / 1.3f, 1f);

        /// <summary>
        /// Збільшує значення висоти для наступного черв'яка [_step]
        /// </summary>
        private void NextStep()
            => _step += _storeOffSetY;

        /// <summary>
        /// Вводимо черв'яка [worm] - переміщує черв'яка до точки в банці
        /// </summary>
        private Tween MoveToBasket(Worm worm)
            => worm.transform.DOMove(CalculateRandomPositionInBasket(), 1);

        /// <summary>
        /// Повертає позицію в банці
        /// </summary>        
        private Vector3 CalculateRandomPositionInBasket()
            => new(_storePoint.position.x + CalculateRandomValue(_storeOffSetX), _storePoint.position.y + _step, 0);

        /// <summary>
        /// Вводимо значення відступу в банці [value] - повертає випадкове значення від "-value до value" 
        /// </summary>        
        private float CalculateRandomValue(float value)
            => Random.Range(-value, value);

        /// <summary>
        /// Вводимо черв'яка [worm]- назначає черв'яка дочірнім елементом банки
        /// </summary>        
        private void MakeVisualChildOfBasket(Worm worm)
        {
            worm.SetVisualNonInteractOrder(_indexSortingOrder);
            worm.transform.SetParent(transform);
        }

        /// <summary>
        /// Перевіряє чи достатньо черв'яків зібрано
        /// </summary>        
        private bool IsEnoughWorms()
            => CurrentProgress >= MaxProgress;
    }
}
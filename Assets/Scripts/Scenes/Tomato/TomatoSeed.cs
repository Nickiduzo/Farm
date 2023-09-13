using DG.Tweening;
using AwesomeTools.Inputs;
using System;
using System.Collections.Generic;
using UnityEngine;
using AwesomeTools.Sound;
using UsefulComponents;
using AwesomeTools;

namespace Tomato
{
    public class TomatoSeed : MonoBehaviour, IRipeInvoker
    {
        public event Action OnStored;
        public event Action OnTomatosGrowed;
        public event Action OnTomatosFullGrowed;
        private const string animationBoolName = "Waveing";

        [SerializeField] private Animator _animator;
        [SerializeField] private TomatoHoleTriggerObserver _observer;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Collider2D _triggerCollider;
        [SerializeField] public List<GameObject> _tomatoSpawnPoints = new List<GameObject>();
        [SerializeField] private List<GameObject> _tomatoSpawned = new List<GameObject>();
        [SerializeField] private List<SpriteRenderer> _leaves = new List<SpriteRenderer>();
        [SerializeField] private float _timeLeavesFade;
        [SerializeField] private float _timeSeedScaleDuration;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private Vector3 _scalesForGrowLeaves;
        [SerializeField] private Vector3 _scalesForSeed;
        [SerializeField] private Transform _destinationReservPoint;
        [SerializeField] private List<Transform> _destinationReservPoints;
        [SerializeField] private float _maxValueToSpawn;

        private SpriteRenderer _currentSprite;
        private SoundSystem _soundSystem;
        private GameObject _tomatoPool;
        private Vector3 _destination;
        private Action _onGrowAction;
        
        private float _currentValueToSpawn;
        private bool isSeedPlanted = false;
        private int _orderLayer = 12;
        
        public bool isUsing;
        public bool fullWet;
        public bool? canGrow = false;
        public int tomatoReady;

        /// <summary>
        /// Додає ф-цію "ProcessHole" до події "OnTriggerEnter"
        /// </summary>
        private void Awake()
            => _observer.OnTriggerEnter += ProcessHole;

        /// <summary>
        /// Видаляє ф-цію "ProcessHole" з події "OnTriggerEnter"
        /// </summary>
        private void OnDestroy()
            => _observer.OnTriggerEnter -= ProcessHole;

        /// <summary>
        /// Виконує ф-цію "HideFade"
        /// </summary>
        private void Start()
        {
            HideFade();
        }

        /// <summary>
        /// Вводимо яму [hole] - додає саженець до ями[hole] та активує колайдер для поливу 
        /// </summary>
        /// <param name="hole">яма</param>
        private void ProcessHole(TomatoHole hole)
        {
            if (hole.IsEmpty() && !isSeedPlanted)
            {
                isSeedPlanted = true;
                MakeNonInteractable();
                hole.StoreSeed(this);
                HintSystem.Instance.HidePointerHint();
                transform.SetParent(hole.transform);
                transform.GetComponent<CircleCollider2D>().enabled = true;
                ContactArea.Instance.MoveSeedToHole(hole.MoveToHolePosition, gameObject.transform, 0.5f)
                                    .OnComplete(() => MoveInHole(hole));
                _animator.SetBool(animationBoolName, true);                
            }
        }

        /// <summary>
        /// Додаємо яму [hole]- викликає ф-цію "AdditionalMovementToHole" 
        /// </summary>
        /// <param name="hole">яма</param>
        private void MoveInHole(TomatoHole hole)
        {
            ContactArea.Instance.AdditionalMovementToHole(hole.StorePosition, gameObject.transform, 0.5f)
                                .OnComplete(() => hole.CloseHole()
                                                      .OnComplete(() => OnStored?.Invoke()));
            _currentSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        /// <summary>
        /// Вводимо позицію призначення [destination] та систему вводу [inputSystem] - 
        /// Присвоюємо позицію призначення "DestinationOnDrag" та систему вводу для "DragAndDrop"
        /// </summary>
        public void Construct(Vector3 destination, InputSystem inputSystem, SoundSystem soundSystem)
        {
            if (_inputSystem == null)
            {
                _inputSystem = inputSystem;
            }
            _soundSystem = soundSystem;
            _destination = destination;
            _dragAndDrop.Construct(inputSystem);
            _destinationOnDragEnd.Construct(destination);
        }

        /// <summary>
        /// Збільшує розмір саженця томатів
        /// </summary>
        public void Appear()
        {
            if (transform.localScale != _scalesForSeed)
            {
                transform.DOScale(_scalesForSeed, _timeSeedScaleDuration);
            }
        }

        /// <summary>
        /// Деактивує колайдер взаємодії з ямою
        /// </summary>
        public void DisableInteractorCollider()
            => _triggerCollider.enabled = false;  
        
        /// <summary>
        /// Активує ф-цію "MoveTo"
        /// </summary>
        public Tween MoveToDestination()
            => MoveTo(_destination);

        /// <summary>
        /// Вводимо томат [tomato]- додаємо томат то саженця томатів
        /// </summary>
        /// <param name="tomato">томат</param>
        public void AddTomatoes(GameObject tomato)
        {
            var readyIndex = GetIndexToSpawn();
            if (tomato.GetComponent<Tomato>() && !_tomatoSpawned.Contains(tomato) || tomato.CompareTag("Tomato"))
            {
                _tomatoSpawned.Add(tomato);
            }
            
            if (readyIndex >= 0)
            {
                Transform tomatoSpawnPointTrans = _tomatoSpawnPoints[readyIndex].transform.GetChild(0);
                tomato.transform.SetParent(tomatoSpawnPointTrans);
                tomato.transform.localPosition = Vector3.zero;
                tomato.transform.localRotation = Quaternion.Euler(Vector3.zero);

                if (readyIndex >= 4)
                {
                    isUsing = true;
                }
            }            

            tomato.GetComponent<DragAndDrop>().Construct(_inputSystem);
            tomato.GetComponent<Tomato>().Construct(_soundSystem);

            if (CheckIfAllSpawned())
            {
                FxSystem.Instance.PlayEffect(0, _destinationReservPoint.position);
                MakeNonInteractable();
                transform.GetComponent<CircleCollider2D>().enabled = false;
            }
        }

        /// <summary>
        /// Вводимо позицію [point] - виконуємо переміщення елемента до позиції
        /// </summary>
        /// <param name="point">позиція</param>        
        private Tween MoveTo(Vector3 point)
            => transform.DOMove(point, 1f);

        /// <summary>
        /// Забороняє взаємодію з елементом
        /// </summary>
        public void MakeNonInteractable()
        {
            _collider.enabled = false;
            DOTween.Kill(gameObject);
            _dragAndDrop.IsDraggable = false;
        }
        
        /// <summary>
        /// Перевіряє чи достатньо з'явилось томатів в саженця томатів
        /// </summary>
        public bool CheckIfAllSpawned()
            => _tomatoSpawned.Count == _tomatoSpawnPoints.Count;

        /// <summary>
        /// Вводимо максимальну к-сть томатів на саженець - якщо к-сть томатів достатня, тоді вимикаємо 
        /// </summary>
        /// <param name="maxValue">максимальна к-сть томатів на саженець</param>
        private void CheckProgress(int maxValue)
        {
            OnTomatosGrowed.Invoke();
            if (tomatoReady >= maxValue)
            {
                OnTomatosFullGrowed.Invoke();
                _observer.GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        /// <summary>
        /// Виконує зміну спрайтів для томатів 
        /// </summary>
        public void GrowTomatos()
        {
            if (canGrow == true)
            {
                foreach (GameObject seed in _tomatoSpawned)
                {
                    Tomato tomato = seed.GetComponent<Tomato>();
                    tomato.SetWhileOnGrow(_onGrowAction);
                    tomato.OnGrow += AddGrowTomato;
                    tomato.OnGrow += () => CheckProgress(_tomatoSpawned.Count);
                    tomato.Appear(this);
                    CheckProgress(_tomatoSpawned.Count);
                }
                canGrow = null;
            }
        }

        /// <summary>
        /// Присвоює томатам точки приземлення
        /// </summary>
        public void SetReservDestinationPointToSpawnedTomato()
        {
            if (_tomatoSpawned.Count == _tomatoSpawnPoints.Count)
            {
                for (int i = 0; i < _tomatoSpawned.Count; i++)
                {
                    Vector3 destPoint = _destinationReservPoints[i].position;
                    _tomatoSpawned[i].GetComponent<Tomato>().SetReservDestinationPoint(destPoint);
                    _tomatoSpawned[i].GetComponent<Tomato>().SetTomatoPool(_tomatoPool.transform);
                }
            }
        }

        /// <summary>
        /// Вводимо подію [action] - присвоюємо події [_onGrowAction]
        /// </summary>        
        public void SetAction(Action action)
            => _onGrowAction = action;
        
        /// <summary>
        /// Викликає метод ShowFade
        /// </summary>
        public void WetAnimation()
        {
            ShowFade();
            fullWet = true;
        }

        /// <summary>
        /// Збільшує розмір гілок саженця томатів
        /// </summary>
        private void ShowFade()
        {
            foreach (SpriteRenderer leaf in _leaves)
            {
                Scale(leaf, _scalesForGrowLeaves, _timeLeavesFade);
            }

            transform.DOScale(_scalesForSeed, _timeLeavesFade);
        }

        /// <summary>
        /// Зменшує розмір гілок саженця томатів
        /// </summary>
        private void HideFade()
        {
            foreach (SpriteRenderer leaf in _leaves)
            {
                Scale(leaf, Vector3.zero, _timeLeavesFade);
            }

            _currentSprite = GetComponent<SpriteRenderer>();
            _currentSprite.maskInteraction = SpriteMaskInteraction.None;
        }

        /// <summary>
        /// Вводимо "SpriteRenderer" [render], розмір[value], тривалість[duration]ь - 
        /// змінюємо розмір гілок саженця томатів
        /// </summary>
        private void Scale(SpriteRenderer render, Vector3 value, float duration)
            => render.transform.DOScale(value, duration);

        /// <summary>
        /// Додає к-сть готових томатів
        /// </summary>            
        private void AddGrowTomato()
            => tomatoReady++;

        /// <summary>
        /// Вимикає підказку при взаємодії з саженцем томатів
        /// </summary>
        private void OnMouseDown()
            => HintSystem.Instance.HidePointerHint();

        /// <summary>
        /// Вводимо об'єкт [pool] - запам'ятовуємо в полі "_tomatoPool"
        /// </summary>        
        public void SetTomatoPool(GameObject pool)
            => _tomatoPool = pool;

        /// <summary>
        /// Повертає індекс появи томата
        /// </summary>
        private int GetIndexToSpawn()
        {
            var index = _tomatoSpawned.Count;
            if (index >= _tomatoSpawnPoints.Count)
            {
                canGrow = true;
                return -1;
            }
            else
            {
                return index;
            }
        }
        
        /// <summary>
        /// Вводимо значення до появи [value] - додаємо до показнику значення появи [_currentValueToSpawn]
        /// </summary>        
        public void SetValueToSpawn(float value)
            => _currentValueToSpawn += value;

        public bool IsTomatoCanSpawn()
        {
            if (_currentValueToSpawn >= _maxValueToSpawn)
            {
                _currentValueToSpawn = 0;
                return true;
            }
            else
                return false;
        }

        public void RipedEnded()
        {
        }
    }
}
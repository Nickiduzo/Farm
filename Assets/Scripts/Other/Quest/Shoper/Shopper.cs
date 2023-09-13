using DG.Tweening;
using System;
using UnityEngine;
using AwesomeTools.Sound;

namespace Quest
{
    public class Shopper : MonoBehaviour
    {
        private const string ARRIVING = "Arriving";
        private const string LEAVING = "Leaving";

        [SerializeField] private float _movingDuration;
        [SerializeField] private GameObject[] _skins;
        [SerializeField] private Transform _skinContainer;
        [SerializeField] private ProductSpawner _productSpawner;
        [SerializeField] private AnimalSelector _animalSelector;

        private QuestCar _questCar;
        private TaskSpawner _taskSpawner;
        private SoundSystem _soundSystem;
        private Task _currentTask;
        private Transform _basketSpawnPoint;
        private Transform _basketDestinationPoint;
        public event Action OnArrived;
        public int SkinsCount => _skins.Length;
        private Vector3 _offSet;
        private bool _playAgain = false;

        // set new [_taskSpawner], pass [skinId] to [SetSkin] and invoke it
        public void Construct(TaskSpawner taskSpawner, int skinId, SoundSystem soundSystem)
        {
            _soundSystem = soundSystem;
            _taskSpawner = taskSpawner;
            SetSkin(skinId, soundSystem);
        }

        // set offset based on type of car [skinId] for more accurate position of animal in the car
        private void ApplyOffSet(int carSkinId)
        {
            Debug.Log("CarID: " + carSkinId);
            switch (carSkinId)
            {
                case 0:
                    _offSet = new Vector3(1.27f, -0.4f, 0);
                    break;
                case 1:
                    _offSet = new Vector3(1.27f, -0.4f, 0);
                    break;
                case 2:
                    _offSet = new Vector3(2.46f, 0.389f, 0);
                    break;
                case 3:
                    _offSet = new Vector3(1.309f, -0.92f, 0);
                    break;
                default:
                    _offSet = Vector3.zero;
                    break;
            }
        }

        // move shopper to destination and spawn random task
        public void MoveToAndSpawn(Vector3 target)
        {   
            _questCar.WheelRollingForward();
            _soundSystem.PlaySound(ARRIVING);
            MoveTo(target)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                _questCar.ReducedOscillation();
                _currentTask = _taskSpawner.SpawnRandomTask();
                SetTaskChildOfShopper(_currentTask, this);
                OnArrived?.Invoke();
            });
        }
        
        // make task child object of shopper
        private void SetTaskChildOfShopper(Task task, Shopper shopper)
            => task.transform.SetParent(shopper.transform);

        public void PlayLeavingSound()
            => _soundSystem.PlaySound(LEAVING);

        // move shopper to scene and destroys task if it is available
        public Tween MoveTo(Vector3 target)
        {
            if (_currentTask != null)
                Destroy(_currentTask.gameObject);
            return transform.DOMove(target, _movingDuration).OnComplete(() => ReduceOscillation()).SetEase(Ease.InBack);
        }

        // get type of product from config
        public void SetProduct(SceneData _config, Transform basketSpawnPoint)
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                _basketSpawnPoint = basketSpawnPoint;
                _productSpawner.ProductSelection(_config, _basketSpawnPoint, GetBasketDestinationPoint());
                _productSpawner.OnAllProductsStored += StartCarAnimation;
                Debug.Log("Product type: " + _config);
            });
        }

        // create new random animal and set in config
        public void SetRandomAnimal(QuestLevelConfig _config)
            => _animalSelector.SetRandomAnimal(_config, _offSet);
        
        // take available animal from config
        public void GetAnimal(QuestLevelConfig _config)
            => _animalSelector.GetAnimalFromConfig(_config, _offSet);

        // create car skin in skin container
        public void SetSkin(int skinId, SoundSystem soundSystem)
        {
            _soundSystem = soundSystem;
            ApplyOffSet(skinId);
            Instantiate(_skins[skinId], _skinContainer);
            GetCar();
            FindBasketDestinationPoint(_skinContainer);
        }

        // get [QuestCar] script from Car in [_skinContainer]
        private QuestCar GetCar()
            => _questCar = _skinContainer.GetComponentInChildren<QuestCar>();
        
        // starts rolling wheels and shaking car
        public void StartCarAnimation()
        {
            if(_playAgain == false)
            {
                _questCar.WheelRolling();
                _questCar.NormalOscillation();
                _playAgain = true;
            }
            _productSpawner.OnAllProductsStored -= StartCarAnimation;
        }
        
        // reduce car oscillation
        private void ReduceOscillation()
        {
            _questCar.ReducedOscillation();
        }

        // start looking for "BasketDestinationPoint", if found, set it to [_basketDestinationPoint]
        private void FindBasketDestinationPoint(Transform parent)
        {
            GameObject basketDestinationPointObject = GameObject.Find("BasketDestinationPoint");
            if (basketDestinationPointObject != null)
            {
                _basketDestinationPoint = basketDestinationPointObject.transform;
            }
        }

        public Transform GetBasketDestinationPoint()
        {
            return _basketDestinationPoint;
        }
    }
}
using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UI;
using UnityEngine;
using UsefulComponents;

namespace CowScene
{
    public class Cow : MonoBehaviour, IProgressWriter
    {
        private const string MILK_STREAM_SOUND = "MilkStream";
        private const string CATCH_SOUND = "Catch";
        private const string EAT_HAY_SOUND = "EatHay";
        private const string SUCCESS_SOUND = "Success";

        private const string IDLE_ANIM = "Idle";
        private const string OPEN_MOUTH_ANIM = "OpenMouth";
        private const string EAT_ANIM = "Eat";
        private const string MOVE_ANIM = "Move";
        private const string MILKING_ANIM = "Milking";

        public event Action OnArrived;
        public event Action CowFed;
        public event Action CowMilked;
        public event Action OnProgressChanged;

        [Header("Observers")]
        [SerializeField] private HayTriggerObserver _hayEatTrigger;
        [SerializeField] private MilkJarTriggerObserver _jarTrigger;
        [Header("Components")]
        [SerializeField] private Collider2D _jarPositionCollider;
        [SerializeField] private Animator _animator;
        [Header("Positions")]
        [SerializeField] private Transform _jarPosition;
        [SerializeField] private Transform _milkingHintPosition;
        [SerializeField] private Transform _feedFxPosition;
        [SerializeField] private Transform _milkingFxPosition;
        [Header("Misc")]
        [SerializeField] private float _movingDuration;
        [SerializeField] private float _eatDelay;
        [SerializeField] private Udder _udder;
        [SerializeField] private CowLevelConfig _config;

        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;
        private Vector3 _destination;
        private Vector3 _end;
        private Jar _currentJar;
        private bool firstJar = true;
        private Sequence _hayProcess;

        public int CurrentProgress { get; private set; }
        public int MaxProgress => _config.HayCount + _config.JarCount;
        public Vector3 FeedingHintPoint => _feedFxPosition.position;
        public Vector3 MilkingHintPoint => _milkingHintPosition.position + Vector3.down;

        // It subscribes from events
        private void Awake()
        {
            _hayEatTrigger.OnTriggerEnter += ProcessHay;
        }

        // Constructs the cow with the specified destination, end position, sound system, and FX system
        public void Construct(Vector3 destination, Vector3 end, ISoundSystem soundSystem, FxSystem fxSystem)
        {
            _destination = destination;
            _end = end;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;

            MoveTo(_destination)
                .OnComplete(() =>
                {
                    SetAnimatorTrigger(IDLE_ANIM);
                    OnArrived?.Invoke();
                });
        }

        // Sets the cow's animation trigger for preparing to eat
        public void PreparingToEat(bool isPreparing)
        {
            SetAnimatorTrigger(isPreparing ? OPEN_MOUTH_ANIM : IDLE_ANIM);
        }

        // Processes the hay by making the cow eat it and updating the progress
        public void ProcessHay(Hay hay)
        {
            if (_hayProcess != null)
                return;

            var startEatDelay = _eatDelay / 5f;
            _hayProcess = DOTween.Sequence();
            _hayProcess.AppendInterval(startEatDelay)
                .AppendCallback(() =>
                {
                    hay.Hide();
                    SetAnimatorTrigger(EAT_ANIM);
                    _soundSystem.PlaySound(EAT_HAY_SOUND);
                })
                .AppendInterval(_eatDelay)
                .AppendCallback(() =>
                {
                    hay.GetEaten();
                    CurrentProgress++;
                    OnProgressChanged.Invoke();
                    CowFed?.Invoke();
                    _fxSystem.PlayEffect(SUCCESS_SOUND, _feedFxPosition.position);
                });
            _hayProcess.OnComplete(() =>
            {
                _hayProcess.Kill(); 
                _hayProcess = null;
            });
            _hayProcess.Play();
        }

        // Places the jar on the cow's udder
        private void PlaceJar(Jar jar)
        {
            if (jar == null) return;

            _soundSystem.PlaySound(CATCH_SOUND);
            _currentJar = jar;
            _currentJar.MakeNonInteractable();
            MoveJarToUdderPosition(_currentJar);
            _jarPositionCollider.enabled = false;

            _currentJar.transform.DOMove(_jarPosition.position, 1f)
                .OnComplete(() =>
                {
                    if (firstJar == true)
                        HintSystem.Instance.ShowPointerHint(_milkingHintPosition.position);

                    _currentJar.Open();
                    _udder.MakeInteractable();
                });
            jar.JarFilled += OnJarFilled;
        }

        // Called when the cow is fully fed
        public void CowFullyFed()
        {
            _hayEatTrigger.OnTriggerEnter -= ProcessHay;
            _jarTrigger.OnTriggerEnter += PlaceJar;

            _jarPositionCollider.enabled = true;
            _udder.gameObject.SetActive(true);

            _udder.CowMilked += GiveMilk;
            _udder.MilkingConcluded += MilkingConcluded;
        }

        // Gives milk when the cow is being milked
        private void GiveMilk()
        {
            _jarTrigger.OnTriggerEnter -= PlaceJar;

            CurrentProgress++;
            OnProgressChanged.Invoke();
            SetAnimatorTrigger(MILKING_ANIM);
            _soundSystem.PlaySound(MILK_STREAM_SOUND);

            if (firstJar == true)
            {
                HintSystem.Instance.HidePointerHint();
                firstJar = false;
            }

            _currentJar.MakeNonInteractable();
            _currentJar.FillWithMilk();
        }

        // Called when milking is concluded
        private void MilkingConcluded()
        {
            SetAnimatorTrigger(IDLE_ANIM);
            _soundSystem.PlaySound(SUCCESS_SOUND);
            _soundSystem.StopSound(MILK_STREAM_SOUND);
            _fxSystem.PlayEffect(SUCCESS_SOUND, _milkingFxPosition.position);
            CowMilked?.Invoke();
        }

        // Called when the jar is filled with milk
        private void OnJarFilled(Jar jar)
        {
            _jarTrigger.OnTriggerEnter += PlaceJar;
            _jarPositionCollider.enabled = true;
            _udder.StopMilking();
            _currentJar = null;
        }

        // Ends the milking process
        public void EndMilking()
        {
            // Reset animator trigger to "Idle" and deactivate udder
            _animator.ResetTrigger(IDLE_ANIM);
            _udder.gameObject.SetActive(false);
        }

        // Moves the cow to the end position and returns a tween
        public Tween GoToEnd()
        {
            _udder.DisableJarHint();
            return MoveTo(_end);
        }

        // Sets the animator trigger to the specified state name
        public void SetAnimatorTrigger(string stateName)
        {
            _animator.SetTrigger(stateName);
        }

        // Moves the jar to the udder position
        private void MoveJarToUdderPosition(Jar jar)
        {
            jar.transform.DOMove(_jarPosition.position, 1f);
        }

        // Moves the object to the specified end position
        public Tween MoveTo(Vector3 endPosition)
        {
            SetAnimatorTrigger(MOVE_ANIM);
            return transform.DOMove(endPosition, _movingDuration).SetEase(Ease.Linear);
        }

    }
}
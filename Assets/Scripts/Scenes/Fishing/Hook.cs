using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using System.Collections;
using UI;
using UnityEngine;

namespace Fishing
{
    public class Hook : MonoBehaviour, IProgressWriter
    {
        private const string PullingRodOut = "PullingRodOut"; 
        private const string CastingRod = "CastingRod";
        private const string CatchFish = "CatchFish";
        public event Action OnProgressChanged;
        public int CurrentProgress => _catchedFishCount;
        public int MaxProgress => _config.FishSpawnCount;

        [SerializeField] private FishLevelConfig _config;
        [SerializeField] private float _movingDuration;
        [SerializeField] private float _movingToStartPointDuration;
        [SerializeField] private float _fishUnHookTime;
        [SerializeField] private Transform _heightBoundPosition;
        [SerializeField] private float _hookCooldownTime;

        private InputSystem _inputSystem;

        private bool _catchingFish;
        private bool _isFirstFish;
        private int _catchedFishCount;
        private bool _isHooking;
        private bool _isCatching = false;


        private UnhookFish _unhookFish;
        private Vector3 _startPosition;
        private Collider2D _hookCollider;
        private Vector3 _hookPoint;
        private Sequence _sequence;
        private Transform _previousUnhookFishParent;
        private ISoundSystem _soundSystem;
        private FishingNet _fishingNet;
        private Coroutine _hintRoutine;
        private Animator _boatAnimator;

        public event Action OnDoHook;

        private float LastHookTime = 0;
        public bool IsHookReady => Time.time - LastHookTime > _hookCooldownTime;

        /// <summary>
        /// Коли вимикається елемент видаляються ф-ції з події "OnTapped" системи вводу
        /// </summary>
        private void OnDisable()
        {
            _inputSystem.OnTapped -= Catch;
            _inputSystem.OnTapped -= ActivateHint;
        }

        /// <summary>
        /// Вводимо систему вводу, звуку та аніматор -
        /// присвоюємо у відповідні поля значення параметрів, додаємо ф-ції до події "OnTapped" системи вводу,
        /// запам'ятовуємо позицію появи елементу [_startPosition]
        /// </summary>
        public void Construct(InputSystem inputSystem, ISoundSystem soundSystem, Animator animator)
        {
            _soundSystem = soundSystem;
            _inputSystem = inputSystem;
            _isFirstFish = true;
            _boatAnimator = animator;
            _hookCollider = GetComponent<Collider2D>();
            _startPosition = transform.position;
            _inputSystem.OnTapped += Catch;
            _inputSystem.OnTapped += ActivateHint;
            LastHookTime = Time.time;
        }

        /// <summary>
        /// Вводимо об'єкт зі скриптом "FishingNet" [net] -
        /// запам'ятовуємо параметр у полі "_fishingNet"
        /// </summary>        
        public void NetSpawned(FishingNet net)
            => _fishingNet = net;

        /// <summary>
        /// Вводимо об'єкт "Collider2D" [target] -
        /// Перевіряємо чи об'єкт параметру має скрипт "UnhookFish", якщо так, 
        /// запам'ятовує об'єкт з типом "UnhookFish" у полі "_unhookFish", викликає метод "Hooked";
        /// перевіряємо чи об'єкт параметру має скрипт "Fish", якщо так, виконуємо метод "Caught"
        /// збільшує к-сть спійманої риби [_catchedFishCount] та оновлює час ловлі [LastHookTime]
        /// </summary>        
        private void OnTriggerEnter2D(Collider2D target)
        {
            if (_isCatching) return;

            if (target.TryGetComponent(out UnhookFish unhook))
            {
                _isCatching = true;
                _unhookFish = unhook;
                _unhookFish.Hooked();

                _previousUnhookFishParent = _unhookFish.transform.parent;

                _catchingFish = true;
                _hookCollider.enabled = false;

                PutBack();
            }

            if (!target.TryGetComponent(out Fish fish) || !CanCatch(fish))
            {
                _isCatching = true;
                if (target.TryGetComponent<IUncaughtable>(out var noFish))
                {
                    noFish.Caught();
                }
                return;
            }

            _isCatching = true;

            if (_isFirstFish)
            {
                _isFirstFish = false;
            }

            _hookCollider.enabled = false;
            LastHookTime = Time.time;

            _soundSystem.PlaySound(CatchFish);

            fish.Hooked();
            MakeChildOfHook(fish.gameObject);

            _catchingFish = true;
            

            PutInNet(fish);
        }

        /// <summary>
        /// Повертає на місце гачок
        /// </summary>
        private void PutBack()
        {
            _hookCollider.enabled = false;
            Vector3 cachedPosition = transform.position;
            Vector3 destination = CalculateMidOfHookPoint();

            _sequence.Kill();
            _sequence = DOTween.Sequence();
            _sequence.AppendCallback(MakeChildUnhookFish);
            _sequence.Append(MoveTo(destination, _movingDuration));
            _sequence.Append(MoveTo(cachedPosition, _movingDuration));
            _sequence.AppendCallback(_unhookFish.UnHooked);
            _sequence.AppendCallback(MakeUnChildUnhookFish);
            _sequence.Append(MoveToStartPoint());
            _sequence.AppendCallback(ResetHook);
        }

        /// <summary>
        /// Повертає позицію середини гачку
        /// </summary>        
        private Vector3 CalculateMidOfHookPoint()
            => (_hookPoint + _startPosition) / 2;

        /// <summary>
        /// Присвоює непійманій рибі [_unhookFish] "parent"-a об'єкт "_previousUnhookFishParent"
        /// </summary>
        private void MakeUnChildUnhookFish()
            => _unhookFish.transform.SetParent(_previousUnhookFishParent);

        /// <summary>
        /// Присвоює непійманій рибі [_unhookFish] "parent"-a об'єкт елементу
        /// </summary>
        private void MakeChildUnhookFish()
            => _unhookFish.transform.SetParent(transform);

        /// <summary>
        /// Вводимо об'єкт [obj] - 
        /// присвоює об'єкту "parent"-a об'єкт елементу та позицію елементу
        /// </summary>
        private void MakeChildOfHook(GameObject obj)
        {
            obj.transform.SetParent(transform);
            obj.transform.position = transform.position;
        }

        /// <summary>
        /// Вводимо рибу [fish]- перевіряємо чи можемо спйіймати рибу
        /// </summary>        
        private bool CanCatch(Fish fish)
            => !fish.IsCatch && !_catchingFish;

        /// <summary>
        /// Вводимо рибу [fish]-
        /// виконуємо ф-цію "MoveFishToNet" з параметром "fish" та переміщуємо назад гачок
        /// </summary>        
        private void PutInNet(Fish fish)
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            MoveFishToNet(fish);
            _sequence.Append(MoveToStartPoint());
            _sequence.AppendCallback(ResetHook);
        }

        /// <summary>
        /// Вводимо рибу [fish]-
        /// викликаємо ф-цію "MoveToNet" з параметром "fish"
        /// </summary>        
        private void MoveFishToNet(Fish fish)
        {
             _catchedFishCount++;
            var fishSequence = DOTween.Sequence();
            fishSequence.AppendInterval(_movingToStartPointDuration - _fishUnHookTime);
            fishSequence.Append(MoveToNet(fish));
        }

        /// <summary>
        /// Оновлюємо здатність гачка ловити рибу
        /// </summary>
        private void ResetHook()
        {
            _isCatching = false;
            _catchingFish = false;
            _hookCollider.enabled = true;
            _isHooking = false;

            if (_catchedFishCount < _config.FishSpawnCount) return;

            DOTween.Kill(_sequence);
            enabled = false;
        }

        /// <summary>
        /// Рухаємо до початкової позиції [_startPosition] з тривалістю [_movingToStartPointDuration]
        /// </summary>        
        private Tween MoveToStartPoint()
            => MoveTo(_startPosition, _movingToStartPointDuration);

        /// <summary>
        /// Вводимо рибу [fish]-
        /// риба виконує ф-цію "JumpTo" з параметром "posInNet"
        /// </summary>        
        private Tween MoveToNet(Fish fish)
        {
            var posInNet = _fishingNet.NetStorePosition;
            fish.SetPositionInNet(posInNet);
            return fish.JumpTo(posInNet).OnComplete(() => PlaySuccess(fish));
        }

        /// <summary>
        /// Вводимо рибу [fish]-
        /// викликаємо ефект та звук успіху в позиції риби
        /// </summary>        
        private void PlaySuccess(Fish fish)
        {                                  
            OnProgressChanged?.Invoke();
            FxSystem.Instance.PlayEffect("Success", fish.gameObject.transform.position);
            _soundSystem.PlaySound("Success");
        }

        /// <summary>
        /// Переміщує то точки гачка [_hookPoint] з тривалістю "_movingDuration"
        /// </summary>        
        private Tween MoveToHookPoint()
            => MoveTo(_hookPoint, _movingDuration);

        /// <summary>
        /// Вводимо позицію [point] та тривалість [duration] -
        /// рухає на позицію з тривалістю
        /// </summary>        
        private Tween MoveTo(Vector3 point, float duration)
            => transform.DOMove(point, duration);

        /// <summary>
        /// Вводимо позицію [targetPosition] -
        /// Рухаємо гачок до позиції та викликає ф-цію "DoHook"
        /// </summary>        
        private void Catch(Vector3 targetPosition)
        {
            if (!CanHook()) return;

            _hookPoint = targetPosition;

            if (!IsFitInBound()) return;

            _isHooking = true;

            DoHook();
        }

        /// <summary>
        /// Перевіряє чи можна зловити
        /// </summary>        
        private bool CanHook()
            => !_catchingFish && !_isHooking && IsHookReady;

        /// <summary>
        /// Перевіряє чи позиція гачка [_hookPoint] нижча за позицію сітки [_heightBoundPosition]
        /// </summary>        
        private bool IsFitInBound()
            => _hookPoint.y < _heightBoundPosition.position.y;

        /// <summary>
        /// Викликає подію "OnDoHook", та рухає гачок то точки натиску та повертає назад
        /// </summary>
        private void DoHook()
        {
            OnDoHook?.Invoke();

            DOVirtual.DelayedCall(0.25f, () =>
            {
                _soundSystem.PlaySound(PullingRodOut);
            });
            _soundSystem.PlaySound(CastingRod);

            _sequence.Kill();
            _sequence = DOTween.Sequence(_sequence);
            _sequence.Append(MoveToHookPoint());
            _sequence.Append(MoveToStartPoint());
            _sequence.AppendCallback(ResetHook);
        }

        /// <summary>
        /// Вводимо позицію [targetPos]-
        /// Викликає "Coroutine" "TimeToActivateHint"
        /// </summary>        
        private void ActivateHint(Vector3 targetPos)
        {
            if (_hintRoutine != null)
            {
                StopCoroutine(_hintRoutine);
            }

            _hintRoutine = StartCoroutine(TimeToActivateHint());
        }
        
        /// <summary>
        /// Через 6 секунд присврює значення "false" до параметру "IsFishing"
        /// </summary>
        private IEnumerator TimeToActivateHint()
        {
            yield return new WaitForSeconds(6);
            _boatAnimator.SetBool("IsFishing", false);
        }
    }
}
using DG.Tweening;
using AwesomeTools.Sound;
using AwesomeTools.Inputs;
using UnityEngine;
using AwesomeTools;
using UsefulComponents;

    /// <summary>
    /// Класс базы для лопаты
    /// </summary>
    public class ShovelBase : MonoBehaviour
    {
        [SerializeField] protected Collider2D _collider;
        [SerializeField] protected MoveStartDestination _move;
        [SerializeField] protected MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] protected DragAndDrop _dragAndDrop;
        [SerializeField] protected Animator _animator;

        protected bool _isDigging;
        /// <summary>
        /// Конструктор, что принимает данные и вызывает другие конструкторы
        /// </summary>
        /// <param name="soundSystem">система звуков</param>
        /// <param name="inputSystem">система нажатий</param>
        /// <param name="destinationPoint">местоположение позиции</param>
        /// <param name="spawnPoint">местоположение спавна</param>
        public void Construct(SoundSystem soundSystem, InputSystem inputSystem, Vector3 destinationPoint, Vector3 spawnPoint)
        {
            _dragAndDrop.Construct(inputSystem);
            _destinationOnDragEnd.Construct(destinationPoint);
            _move.Construct(destinationPoint, spawnPoint);
            Awake();
        }
        /// <summary>
        /// Подписка на события
        /// </summary>
        protected virtual void Awake()
        {
            _dragAndDrop.OnDragStart += DeactivateHint;
            _dragAndDrop.OnDragEnded += MoveToBaseDestination;
        }
        /// <summary>
        /// Отписка от событий при разрушении объекта
        /// </summary>
        protected virtual void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= DeactivateHint;
            _dragAndDrop.OnDragEnded -= MoveToBaseDestination;
        }
        /// <summary>
        /// Остановка анимации DOTween и отключение взаимодействия с управлением
        /// </summary>
        protected virtual void MakeNonInteractable()
        {
            DOTween.Kill(gameObject);
            _dragAndDrop.IsDraggable = false;
            _collider.enabled = false;
        }
        /// <summary>
        /// Перемещение к месту назначения при помощи Tween
        /// </summary>
        private void MoveToBaseDestination()
            => _destinationOnDragEnd.MoveToDestination();
        /// <summary>
        /// Перемещение в начальную позицию при помощи Tween
        /// </summary>
        /// <returns></returns>
        protected virtual Tween MoveToStartPoint()
            => _move.MoveToStart();
        /// <summary>
        /// Перемещение на местоположение при помощи Tween
        /// </summary>
        protected virtual void MoveToDestination()
            => _move.MoveToDestination();
        /// <summary>
        /// Управление параметром анимации
        /// </summary>
        /// <param name="value">булевое значение для аниматора</param>
        protected virtual void SetDig(bool value)
        {
            if (IsAnimatorAvailable())
            {
                _animator.SetBool("State", value);
            }
        }
        /// <summary>
        /// Скрыть подсказку
        /// </summary>
        protected virtual void DeactivateHint()
        {
            HintSystem.Instance.HidePointerHint();
        }
        /// <summary>
        /// Аниматор доступен
        /// </summary>
        /// <returns>возвращает аниматор, если ли он</returns>
        protected virtual bool IsAnimatorAvailable()
        {
            return _animator != null;
        }
        /// <summary>
        /// Показывает эффект
        /// </summary>
        /// <param name="fxSpawnPoint">положение для эффекта</param>
        protected virtual void ShowFx(Transform fxSpawnPoint)
            => FxSystem.Instance.PlayEffect(0, fxSpawnPoint.position);
        
    }

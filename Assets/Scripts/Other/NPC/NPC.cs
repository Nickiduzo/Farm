using System;
using UnityEngine;

namespace Props
{
    /// <summary>
    /// Абстрактный класс NPC(птица/бабочка), где происходит его контроль 
    /// </summary>
    public abstract class NPC : MonoBehaviour
    {
        protected const string FLYING = "Flying";

        [SerializeField] protected Animator Animator;
        [SerializeField] protected NPCScaler NPCScaller;

        protected Transform Target;

        public event Action<NPC, Transform> OnFlewToPoint;
        /// <summary>
        /// Переход на позицию вызвав событие
        /// </summary>
        protected void OnPosition() => OnFlewToPoint?.Invoke(this, Target);
        /// <summary>
        /// Остановка для режима ожидания анимации
        /// </summary>
        protected void Idle()
        {
            NPCScaller.StopScale();
            Animator.SetBool(FLYING, false);
        }
        /// <summary>
        /// Начать менять размер и включение параметра полёта
        /// </summary>
        protected void Fly()
        {
            NPCScaller.StartScale();
            Animator.SetBool(FLYING, true);
        }
        /// <summary>
        /// Отправка цели
        /// </summary>
        /// <param name="target">цель</param>
        public abstract void SetTarget(Transform target);
    }
}
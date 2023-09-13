using System;
using UnityEngine;

namespace Adveritisement
{
    /// <summary>
    /// Базовый обработчик рекламы
    /// </summary>
    public abstract class BaseAdvertisementHandler
    {
        #region EVENTS

        private Action _showSuccess;

        private Action _showFail;

        #endregion

        protected AdvertisementPlacement CurrentPlacement { get; private set; }

        protected abstract string AdvertisementType { get; }

        public abstract bool IsReady { get; }
        /// <summary>
        /// Инициализация
        /// </summary>
        public void Initialize()
        {
            OnInitialize();
        }
        /// <summary>
        /// Поздняя инициализация
        /// </summary>
        public void PostInitialize()
        {
            OnPostInitialize();
        }
        /// <summary>
        /// При инициализации
        /// </summary>
        protected virtual void OnInitialize() { }
        /// <summary>
        /// При поздней инициализации
        /// </summary>
        protected virtual void OnPostInitialize() { }
        /// <summary>
        /// Показывает объявление об открытии приложения.
        /// </summary>
        /// <param name="placement">местоположение рекламы</param>
        /// <param name="successCallback">событие при успешном вызове</param>
        /// <param name="failCallback">событие при неудачном вызове</param>
        public void Show(AdvertisementPlacement placement, Action successCallback = null, Action failCallback = null)
        {
            CurrentPlacement = placement;

            _showSuccess = successCallback;
            _showFail = failCallback;

            Log($"Try Show: {CurrentPlacement};");

            if (IsReady)
            {
                Log($"Show: {CurrentPlacement};");
                OnShow(CurrentPlacement);
                return;
            }

            Log($"Show not ready: {CurrentPlacement};");
            OnNotReady();
            OnShowFail();
        }
        /// <summary>
        /// При показе объявлении.
        /// </summary>
        /// <param name="placement"></param>
        protected abstract void OnShow(AdvertisementPlacement placement);
        /// <summary>
        /// Если реклама всё ещё не готова
        /// </summary>

        protected virtual void OnNotReady() { }
        /// <summary>
        /// При успешном показе вызывается событие
        /// </summary>
        protected void OnShowSuccess()
        {
            _showSuccess?.Invoke();
        }
        /// <summary>
        /// При неудачно показе вызывается событие
        /// </summary>
        protected void OnShowFail()
        {
            _showFail?.Invoke();
        }
        /// <summary>
        /// Меотод для вывода типа рекламы и сообщения
        /// </summary>
        /// <param name="message">сообщение в консоль</param>
        protected void Log(string message)
        {
            Debug.Log($"{AdvertisementType}: {message}");
        }
        /// <summary>
        /// Утилизация
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }
        /// <summary>
        /// При утилизации
        /// </summary>
        protected abstract void OnDispose();
    }

}

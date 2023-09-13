using UnityEngine;
using AwesomeTools;

namespace Tomato
{
    public class ObserverChange : MonoBehaviour
    {
        [SerializeField] private TomatoHoleTriggerObserver _tomatoHoleTriggerObserver;
        [SerializeField] private DragAndDrop _dragAndDrop;

        /// <summary>
        /// Додає ф-цію до події "OnDragEnded"
        /// </summary>
        private void Start()
        {
            _tomatoHoleTriggerObserver.enabled = false;
            _dragAndDrop.OnDragEnded += EnableObserver;
        }

        /// <summary>
        /// Видаляє ф-цію з події "OnDragEnded"
        /// </summary>
        private void OnDestroy()
        {
            EnableObserver();
            _dragAndDrop.OnDragEnded -= EnableObserver;
        }

        /// <summary>
        /// Дозволяє взаємодіяти з ямою для саженця томатів
        /// </summary>
        public void EnableObserver()
        {
            _tomatoHoleTriggerObserver.enabled = true;
        }
    }
}

using DG.Tweening;
using UnityEngine;

namespace Tomato
{
    public class SpawnAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject[] _triggeredGameObjects;
        [SerializeField] private Vector3 _cachedScale;
        [SerializeField] private float _durationAnimation;

        [Space, SerializeField] private Vector3 _cachedScaleDefault;
        [SerializeField] private float _durationAnimationDefault;
        [SerializeField] private bool _selfSpawnTrigger;

        /// <summary>
        /// Викликає ф-цію "CacheValues"
        /// </summary>
        private void Awake()
        {
            CacheValues();
        }

        /// <summary>
        /// Викликає ф-цію "PlaySetSpawnAnimation"
        /// </summary>
        private void Start()
        {
            PlaySetSpawnAnimation();
        }

        /// <summary>
        /// Викликає ф-цію "PlaySpawnAnimation" до кожного об'єкту зі списку "_triggeredGameObjects" та до елементу
        /// </summary>
        public void PlaySetSpawnAnimation()
        {
            if (_selfSpawnTrigger)
            {
                if (_triggeredGameObjects.Length > 0)
                {
                    PlaySpawnAnimation(_triggeredGameObjects);
                    return;
                }
                PlaySpawnAnimation();
            }
        }

        /// <summary>
        /// Вводимо список об'єктів сцени [gameObjects]- повертає до початкових розмірів об'єкти зі списку
        /// </summary>        
        private void PlaySpawnAnimation(GameObject[] gameObjects)
        {
            foreach (GameObject currentObject in gameObjects)
            {
                currentObject.transform.localScale = Vector3.zero;
                currentObject.transform.DOScale(_cachedScale, _durationAnimation);
            }
        }

        /// <summary>
        /// Повертає до початкових розмірів елемент
        /// </summary>
        private void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(_cachedScale, _durationAnimation);
        }

        /// <summary>
        /// Запам'ятовує початковий розмір елементу 
        /// </summary>
        private void CacheValues()
        {
            if (_cachedScaleDefault == Vector3.zero)
            {
                _cachedScaleDefault = transform.localScale;
            }

            if (_cachedScale == Vector3.zero && _durationAnimation == 0f)
            {
                _cachedScale = _cachedScaleDefault;
                _durationAnimation = _durationAnimationDefault;
            }
        }
    }
}

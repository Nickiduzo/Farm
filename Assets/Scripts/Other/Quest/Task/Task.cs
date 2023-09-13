using DG.Tweening;
using Scene;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Quest
{
    /// <summary>
    /// Задание покупателя
    /// </summary>
    public class Task : MonoBehaviour
    {
        [SerializeField] private SceneData _type;
        [SerializeField] private float _hintDelay = 15f;
        [SerializeField] private Transform _itemsTransform;
        [SerializeField] private QuestLevelConfig _config;
        [SerializeField] private MouseTrigger _mouseTrigger;

        private SaveLoadSystem _saveLoad;
        private SceneLoader _sceneLoader;
        private SoundSystem _soundSystem;
        private bool _interactable;

        public SceneData Type => _type;

        /// <summary>
        /// Появление задания
        /// </summary>
        private void Awake()
        {
            _mouseTrigger.OnUp += TaskPicked;
            _interactable = false;
            _itemsTransform.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1f, 0.7f).SetEase(Ease.OutBack));
            sequence.Append(_itemsTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
            sequence.AppendCallback(() => _interactable = true);
            sequence.Play();
        }

        /// <summary>
        /// Констуктор получения компонентов и показа подсказки
        /// </summary>
        /// <param name="soundSystem">звуковая система</param>
        /// <param name="sceneLoader">загрузчик уровня</param>
        /// <param name="saveLoad">сохранение данных</param>
        public void Construct(SoundSystem soundSystem, SceneLoader sceneLoader, SaveLoadSystem saveLoad)
        {
            _sceneLoader = sceneLoader;
            _saveLoad = saveLoad;
            _soundSystem = soundSystem;
            Invoke(nameof(ShowHint), _hintDelay);
        }

        /// <summary>
        /// На задание кликнули и обработало параметры 
        /// </summary>
        private void TaskPicked()
        {
            if (!_interactable) return;
            CancelInvoke(nameof(ShowHint));
            SetSceneTypeInConfig();
            _saveLoad.Save(this);
            _interactable = false;
            ShowClickFx();
            _sceneLoader.LoadScene(_type.Key);
        }

        /// <summary>
        /// Сохраняет новый тип сцены в конфиге (используется для того, чтобы в будущем покупатель забрал тот же товар, который он заказал)
        /// </summary>
        private void SetSceneTypeInConfig()
            => _config.sceneType = _type;
        /// <summary>
        /// Показать подсказку
        /// </summary>
        private void ShowHint()
            =>HintSystem.Instance.ShowPointerHint(transform.position + Vector3.up + Vector3.right);
        /// <summary>
        /// Скрыть подсказку
        /// </summary>
        private void HideHint()
        {
            if (HintSystem.Instance != null)
            {
                HintSystem.Instance.HidePointerHint();
            }
        }

        /// <summary>
        /// Появление FX/SFX и скрывает подсказку
        /// </summary>
        private void ShowClickFx()
        {
            HideHint();
            _soundSystem.PlaySound("PlaybuttonUIClick");
            Vector3 originalScale = transform.localScale;
            transform.DOScale(transform.localScale.x - 0.1f, 0.1f).OnComplete(() =>
            {
                transform.DOScale(originalScale, 0.1f);
            });
        }
        /// <summary>
        /// Отписка от события и скрывает подсказку
        /// </summary>
        private void OnDestroy()
        {
            HideHint();
            _mouseTrigger.OnUp -= TaskPicked;
        }
    }
}
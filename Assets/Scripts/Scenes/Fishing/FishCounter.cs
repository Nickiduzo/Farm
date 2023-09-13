using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Класс, что отвечает за количество рыбы в сетке
    /// </summary>
    public class FishCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private int _fishCount;
        /// <summary>
        /// При использовани подписывается на события
        /// </summary>
        private void OnEnable()
        {
            FishActorUI.OnAppearFishCounter += Reset;
            FishActorUI.OnUpdateFishCounter += AddFish;
            FishActorUI.OnWithDrawFish += WithdrawFish;
        }
        /// <summary>
        /// При разрушении отписывается от событий
        /// </summary>
        private void OnDisable()
        {
            FishActorUI.OnAppearFishCounter -= Reset;
            FishActorUI.OnUpdateFishCounter -= AddFish;
            FishActorUI.OnWithDrawFish -= WithdrawFish;
        }
        /// <summary>
        /// Добавляет численность рыбе и отображает в тексте
        /// </summary>
        public void AddFish()
        {
            _fishCount++;
            _text.text = _fishCount.ToString();
        }
        /// <summary>
        /// Уменьшает численность рыбы и отображает в тексте
        /// </summary>
        public void WithdrawFish()
        {
            _fishCount--;
            _text.text = _fishCount.ToString();
        }
        /// <summary>
        /// Сбрасывает численность рыбы
        /// </summary>
        internal void Reset()
        {
            _fishCount = 0;
            _text.text = _fishCount.ToString();
        }
    }
}
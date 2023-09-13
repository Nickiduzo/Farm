using UnityEngine;

namespace UI
{
    /// <summary>
    /// Тот же класс HUD, только со своим значением
    /// </summary>
    public class FishHud : Hud
    {
        [SerializeField] private FishCounter _fishCounter;

        public override void Disappear()
        {
            base.Disappear();
            //_fishCounter.Disappear();
        }
    }

}
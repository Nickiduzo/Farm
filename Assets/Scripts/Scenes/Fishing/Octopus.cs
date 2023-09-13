using UnityEngine;

namespace Fishing
{
    public class Octopus : MonoBehaviour, IUncaughtable
    {
        private const string CaughtKey = "Caught";
        [SerializeField] private Animator _anim;
        [SerializeField] private FxSystem _fxSystem;

        /// <summary>
        /// Викликаємо тригер "CaughtKey" та ф-цію "InitParticles"
        /// </summary>
        public void Caught()
        {
            _anim.SetTrigger(CaughtKey);
            InitParticles();
        }

        /// <summary>
        /// Викликаємо ефект "Octopus" в позиції елементу
        /// </summary>
        private void InitParticles()
        {
            _fxSystem.PlayEffect("Octopus", transform.position);
        }
        
    }
}
using UnityEngine;

namespace Fishing
{
    public class UnhookFish : MonoBehaviour
    {
        [SerializeField] private RandomMover _mover;
        [SerializeField] private NoFishesMover _noFishesMover;

        /// <summary>
        /// Вимикає можливість руху
        /// </summary>
        public void Hooked()
        {
            if (_mover != null)
            {
                _mover.enabled = false;
            }
            _noFishesMover.PauseMovement();
        }

        /// <summary>
        /// Вмикає можливість руху
        /// </summary>
        public void UnHooked()
        {
            if (_mover != null)
            {
                _mover.enabled = true;
            }
            _noFishesMover.ResumeMovement();
        }
    }
}
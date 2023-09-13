using AwesomeTools.Sound;
using UnityEngine;

namespace SunflowerScene
{
    public class SuccessEffectsHandler
    {
        private const string Success = "Success";
        private readonly SoundSystem _soundSystem;
        private readonly FxSystem _fxSystem;

        public SuccessEffectsHandler(SoundSystem soundSystem, FxSystem fxSystem)
        {
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
        }

        // Plays the success sound.
        public void PlaySuccessSound()
        {
            _soundSystem.PlaySound(Success);
        }

        // Plays the success effect at the specified position.
        public void PlaySuccessEffect(Vector3 position)
        {
            _fxSystem.PlayEffect(Success, position);
        }
    }
}
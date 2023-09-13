using AwesomeTools.Sound;

namespace SunflowerScene
{
    public class SoundPlayer
    {
        private readonly SoundSystem _soundSystem;
        private readonly string _soundName;

        // Creates a new instance of the SoundPlayer class
        public SoundPlayer(SoundSystem soundSystem, string soundName)
        {
            _soundSystem = soundSystem;
            _soundName = soundName;
        }

        // Plays the sound associated with this SoundPlayer instance.
        public void Play()
        {
            _soundSystem.PlaySound(_soundName);
        }

        // Stops the currently playing sound associated with this SoundPlayer instance.
        public void Stop()
        {
            _soundSystem.StopSound(_soundName);
        }
    }
}
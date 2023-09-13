using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace Apple
{
    public class AppleWaterPumpStream : IDisposable
    {
        private SpriteRenderer _sprite;
        private SpriteRenderer _spritePipe;

        private readonly string _waterSound = "Water";

        // Initializes the AppleWaterPumpStream
        public AppleWaterPumpStream(SpriteRenderer sprite, SpriteRenderer spritePipe, BaseSprinkler appleSprinkler)
        {
            _sprite = sprite;
            _spritePipe = spritePipe;

            appleSprinkler.InitStream();
        }

        // Disposes of the AppleWaterPumpStream
        public void Dispose()
        {
            SoundSystemUser.Instance.StopSound(_waterSound);
            _sprite.enabled = false;
            _spritePipe.enabled = false;
        }
    }
}
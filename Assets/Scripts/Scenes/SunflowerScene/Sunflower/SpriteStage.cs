using System;
using UnityEngine;

namespace SunflowerScene
{
    [Serializable]
    public class SpriteStage
    {
        public Sprite Sprite;
        public Vector3 Scale;
        public Vector3 Position;
        [Range(1, 100)][SerializeField] private float _viewValue;
        public float Value => _viewValue / 100f;
    }
}
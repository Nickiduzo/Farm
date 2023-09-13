using UnityEngine;

namespace SunflowerScene
{
    public interface ITarget
    {
        public Vector3 Position();
        public bool LastOnWay { get; }
    }
}
using System;

namespace SunflowerScene
{
    public interface ICountable <T>
    {
        public event Action<T> CountUp;
    }
}
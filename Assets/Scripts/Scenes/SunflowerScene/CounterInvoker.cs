using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunflowerScene
{
    public class CounterInvoker<T> where T : ICountable<Transform>
    {
        private readonly List<T> _objects;
        private readonly int _need;

        private int _counted;

        public event Action CountedUp;
        public event Action<Vector3> CountedUpIn;
        public event Action AllCountedUp;

        // Constructor that takes an array of countable objects and subscribe to event
        public CounterInvoker(T[] countables)
        {
            _objects = new List<T>(countables);
            _need = _objects.Count;

            foreach (var countable in _objects)
            {
                countable.CountUp += HandleCount;
            }
        }

        // Constructor that takes the desired count of objects
        public CounterInvoker(int need)
        {
            _need = need;
            _objects = new List<T>();
        }
        
        // Adds an object to the collection
        public void Add(T obj)
        {
            obj.CountUp += HandleCount;
            _objects.Add(obj);
        }

        // Handles the CountUp event of an object by invoking events and updating the count
        private void HandleCount(Transform obj)
        {
            OnCountedUpIn(obj.transform.position);
            Update();
        }

        // Updates the count and checks if all objects have been counted up, triggering the appropriate event
        private void Update()
        {
            _counted++;
            if (_counted >= _need)
            {
                OnAllCountedUp();
            }
        }


        // Handles the event when all objects have been counted up by invoking events
        private void OnAllCountedUp()
        {
            AllCountedUp?.Invoke();
            foreach (var countable in _objects)
            {
                countable.CountUp -= HandleCount;
            }
        }

        // Handles the event
        private void OnCountedUpIn(Vector3 position)
        {
            CountedUp?.Invoke();
            CountedUpIn?.Invoke(position);
        }
    }
}
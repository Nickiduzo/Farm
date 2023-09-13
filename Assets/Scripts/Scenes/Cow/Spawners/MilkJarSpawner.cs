using CowScene.Spawners.Pools;
using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools;
using AwesomeTools.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace CowScene.Spawners
{
    public class MilkJarSpawner : MonoBehaviour
    {
        [Header("Positions")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _spawnPoint;
        [Header("Misc")]
        [SerializeField] private float _spawnOffset;
        [Header("Systems")]
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [Header("Pool")]
        [SerializeField] private MilkJarPool _pool;

        private float _currentOffSet;
        private Vector3 _destination;
        private List<Jar> _jars = new();


        // Spawns jars with the specified count.
        public void SpawnJars(int jarsCount)
        {
            for (var i = 0; i < jarsCount; i++)
            {
                CalculateJarDestination();

                Jar jar = _pool.Pool.GetFreeElement();
                _jars.Add(jar);
                jar.Construct(_destination, _spawnPoint.position, _soundSystem);
                jar.GetComponent<DragAndDrop>().Construct(_inputSystem);
                jar.MoveTo(_destination).OnComplete(jar.MakeInteractable);

                _currentOffSet += _spawnOffset;
            }
        }

        // Calculates the destination point for the jar based on screen dimensions and the current offset.
        private void CalculateJarDestination()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(0.11f * Screen.width, 0.13f * Screen.height, 1));
            var destinationX = new Vector3(destination.x + _currentOffSet, destination.y, destination.z);
            _destination = destinationX;
        }

        // Makes all jars in the list interactable.
        public void MakeAllJarsInteractable()
        {
            foreach (var jar in _jars)
            {
                _destination = jar.transform.position;
                jar.ChangeDestinationPoint(_destination);
                jar.MakeInteractable();
                jar.SetSortingLayer();
            }
        }

    }
}

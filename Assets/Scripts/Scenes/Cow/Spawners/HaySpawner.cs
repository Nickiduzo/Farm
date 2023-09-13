using CowScene.Spawners.Pools;
using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using AwesomeTools;
using UnityEngine;

namespace CowScene.Spawners
{
    public class HaySpawner : MonoBehaviour
    {
        [Header("Positions")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _spawnPoint;
        [Header("Systems")]
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [Header("Pool")]
        [SerializeField] private HayPool _pool;
        [Header("Mix")]
        [SerializeField ]private float _spawnXOffset;
        [SerializeField ]private HayController _hayController;

        private float _currentXOffset;
        private Vector3 _destination;
        public Vector3 HintPointPosition => _destination;
        public Vector3 OtherHintPointPosition;

        // Spawns hay elements at the calculated destination points.
        public void SpawnHay(int spawnCount)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                CalculateHayDestination();

                Hay hay = _pool.Pool.GetFreeElement();
                _hayController.InitHay(hay);
                hay.Construct(_destination, _spawnPoint.position, _soundSystem);
                hay.GetComponent<DragAndDrop>().Construct(_inputSystem);
                hay.MoveTo(_destination).OnComplete(() => hay.SetInteractable(true));
                hay.AddRenderOrder(i);

                _currentXOffset += _spawnXOffset;
            }
        }

        // Calculates the destination point for the hay based on screen dimensions and the current X offset.
        private void CalculateHayDestination()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(0.11f * Screen.width, 0.14f * Screen.height, 1));
            var destinationX = new Vector3(destination.x + _currentXOffset, destination.y, destination.z);
            _destination = destinationX;
            CalculateHintPoint();
        }

        // Calculates the hint point position based on the destination point.
        private void CalculateHintPoint()
        {
            var newEndPoint = new Vector3(-_destination.x, _destination.y, _destination.z);
            OtherHintPointPosition = newEndPoint;
        }
    }
}


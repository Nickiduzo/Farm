using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using AwesomeTools;
using UnityEngine;

namespace SunflowerScene
{
    public class WaterPumpSpawner : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private Transform _destination;
        [SerializeField] private WaterPumpPool _waterPumpPool;
        [SerializeField] private InputSystem _inputSystem;

        // Spawns a WaterPump object and initializes its position, destination, and input system.
        public WaterPump SpawnWaterPump(SoundSystem soundSystem)
        {
            WaterPump waterPump = _waterPumpPool.Pool.GetFreeElement();
            waterPump.transform.position = _spawnPoint.position;
            _destination.position = _camera.ScreenToWorldPoint(new Vector3(0.8f * Screen.width, 0.25f * Screen.height, 1));
            waterPump.Construct(_destination.position, _inputSystem);
            waterPump.GetComponentInChildren<WaterPumpStreamPrefab>().Construct(soundSystem);
            return waterPump;
        }

        // Hides the WaterPump by disabling its draggable behavior and destroying the associated GameObject.
        public void HideWaterPump(WaterPump waterPump)
        {
            waterPump.IsDraggable = false;
            waterPump.GetComponent<DragAndDrop>().DestroyGameObject();
        }

    }
}
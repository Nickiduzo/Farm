using System.Linq;
using AwesomeTools.Inputs;
using UnityEngine;

namespace SunflowerScene
{
    public class SunflowerSpawner : MonoBehaviour
    {
        [SerializeField] private SunflowerPool _sunflowerPool;

        private Transform[] _sunflowerSpawnPoints;
        private Sunflower[] _sunflowerSprouts;
        private InputSystem _inputSystem;
        [SerializeField] private bool setBackLast3xSunfl;

        //init systems
        public void Construct(InputSystem inputSystem, Transform[] sunflowerSpawnPoints)
        {
            _inputSystem = inputSystem;
            _sunflowerSpawnPoints = sunflowerSpawnPoints;
        }

        // Spawns all the sunflowers at the designated spawn points.
        public void SpawnAllSunflowers()
        {
            _sunflowerSprouts = new Sunflower[_sunflowerSpawnPoints.Length];
            for (var index = 0; index < _sunflowerSpawnPoints.Length; index++)
            {
                var point = _sunflowerSpawnPoints[index];
                var sunflower = _sunflowerPool.Pool.GetFreeElement();
                Transform sunflowerT = sunflower.transform;
                sunflowerT.SetParent(point);
                sunflowerT.position = point.position;
                sunflower.Construct(point, _inputSystem);
                _sunflowerSprouts[index] = sunflower;
                if (setBackLast3xSunfl && index >=3)
                {
                    SpriteRenderer[] spriteRenderers = sunflower.GetComponentsInChildren<SpriteRenderer>();

                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        spriteRenderer.sortingOrder -= 20;
                    }
                }
                else
                {
                    SpriteRenderer[] spriteRenderers = sunflower.GetComponentsInChildren<SpriteRenderer>();

                    foreach (var spriteRenderer in spriteRenderers)
                    {
                        spriteRenderer.sortingOrder += 20;
                    }
                }
                sunflowerT.GetChild(sunflowerT.childCount-1).gameObject.SetActive(false);
                
            }
        }

        // Returns an array of all the spawned sunflowers.
        public Sunflower[] GetObjects()
        {
            return _sunflowerSprouts;
        }
    }
}
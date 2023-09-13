using AwesomeTools.Inputs;
using System;
using UnityEngine;
using UsefulComponents;

namespace Carrot.Spawners
{
    public class CarrotSpawner : MonoBehaviour
    {
        public event Action<Carrot> OnSpawn;

        [SerializeField] private PoolObject _pool;
        [SerializeField] private InputSystem _inputSystem;

        // get carrot from pool and invoke Action [OnSpawn] in which we pass "Carrot"
        public Carrot SpawnCarrot(Vector3 at)
        {
            HintSystem.Instance.HidePointerHint();
            GameObject carrotPrefab = _pool.Pool.GetFreeElement();
            Carrot carrot = carrotPrefab.GetComponent<Carrot>();
            carrot.transform.position = at;
            carrot.Construct(_inputSystem);
            OnSpawn?.Invoke(carrot);
            return carrot;
        }
    }
}
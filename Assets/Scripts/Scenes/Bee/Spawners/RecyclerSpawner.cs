using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;

namespace Bee.Spawners
{
    public class RecyclerSpawner : MonoBehaviour
    {
        [SerializeField] private RecyclerPool _pool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _desinationPoint;

        public Vector3 HintPosition =>  _desinationPoint.position;

        //Spawns recycler
        public HoneyRecycler SpawnRecycler(SoundSystem soundSystem, InputSystem inputSystem)
        {
            HoneyRecycler recycler = _pool.Pool.GetFreeElement();
            recycler.transform.position = _spawnPoint.position;
            recycler.GetComponentInChildren<HoneyGrinder>().Construct(soundSystem, inputSystem);
            recycler.GetComponentInChildren<HoneyTap>().Construct(soundSystem);
            recycler.GetComponent<MoveStartDestination>().Construct(_desinationPoint.position, _spawnPoint.position);
            return recycler;
        }
    }
}
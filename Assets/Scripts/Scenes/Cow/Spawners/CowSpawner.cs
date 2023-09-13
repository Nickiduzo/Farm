using CowScene.Spawners.Pools;
using AwesomeTools.Sound;
using UnityEngine;

namespace CowScene.Spawners
{
    public class CowSpawner : MonoBehaviour
    {
        [Header("Positions")]
        [SerializeField] private Transform _cowSpawnPoint;
        [SerializeField] private Transform _cowDestinationPoint;
        [SerializeField] private Transform _cowEndPoint;
        [Header("Pool")]
        [SerializeField] private CowPool _pool;

        //spawns cow
        public Cow SpawnCow(SoundSystem soundSystem, FxSystem fxSystem)
        {
            Cow cow = _pool.Pool.GetFreeElement();
            cow.transform.position = _cowSpawnPoint.position;

            cow.Construct(_cowDestinationPoint.position, _cowEndPoint.position, soundSystem, fxSystem);
            return cow;
        }

    }
}


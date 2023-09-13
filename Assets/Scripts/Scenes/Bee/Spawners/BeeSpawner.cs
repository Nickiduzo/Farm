using System.Collections.Generic;
using UnityEngine;
using UsefulComponents;

namespace Bee.Spawners
{
    public class BeeSpawner : MonoBehaviour
    {
        [SerializeField] private BeePool _pool;
        [SerializeField] private List<Transform> _positionsToSpawn;
        [SerializeField] private List<Transform> _positionsToDestinate;

        private int _index = 0;

        //Spawns bee
        public Bee SpawnBee()
        {
            Bee bee = _pool.Pool.GetFreeElement();
            bee.transform.position = GetRandomPosition(_positionsToSpawn);
            bee.IncreaseSortingOrder(_index);
            _index += 5;
            bee.GetComponent<LoopMoving>()
                .Construct(GetRandomPosition(_positionsToDestinate));
            bee.GetComponent<MoveStartDestination>()
                .Construct(Vector3.zero, bee.transform.position);
            return bee;
        }

        //returns random position from list
        private Vector3 GetRandomPosition(List<Transform> pos)
            => pos[Random.Range(0, pos.Count)].position;
    }
}
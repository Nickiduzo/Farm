using System.Collections.Generic;
using UnityEngine;

namespace Carrot.Spawners
{
    public abstract class HolesContainer<T> : MonoBehaviour where T : BaseHole
    {
        [SerializeField] protected List<T> _holesOnScene;

        public List<T> HolesOnScene => _holesOnScene;

        // get Collider2D in each hole and enable it
        public void EnableHolesCollider()
        {
            foreach (var hole in _holesOnScene)
                hole.GetComponent<Collider2D>().enabled = true;
        }

        // set hole in List
        public void SetHole(T carrotHole)
            => _holesOnScene.Add(carrotHole);

        // remove all holes from List
        public void Clear()
            => _holesOnScene.Clear();
    }
}
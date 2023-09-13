using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Apple.Spawners
{
    public abstract class HolesContainer<T> : MonoBehaviour where T : BaseHole
    {
        [SerializeField] protected List<T> _holesOnScene;

        public List<T> HolesOnScene => _holesOnScene;

        // Enables the colliders of all the holes on the scene
        public void EnableHolesCollider()
        {
            foreach (var hole in _holesOnScene)
            {
                hole.GetComponent<Collider2D>().enabled = true;
            }
        }

        // Adds a hole to the list of holes on the scene
        public void SetHole(T carrotHole)
            => _holesOnScene.Add(carrotHole);

        // Clears the list of holes on the scene
        public void Clear()
            => _holesOnScene.Clear();

        // Fades out each hole and deactivates its game object when the animation is complete
        public void DisappearHoles()
        {
            foreach (var hole in _holesOnScene)
            {
                hole.Disappear()
                    .OnComplete(() => hole.gameObject.SetActive(false));
            }
        }
    }
}
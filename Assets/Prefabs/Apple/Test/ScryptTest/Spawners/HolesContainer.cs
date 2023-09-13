using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apple.Spawners
{
    public class HolesContainer : MonoBehaviour
    {
        [SerializeField] private List<Hole> _holesOnScene;

        public List<Hole> HolesOnScene => _holesOnScene;

        public void EnableHolesCollider()
        {
            foreach (var hole in _holesOnScene)
                hole.GetComponent<Collider2D>().enabled = true;
        }

        public void SetHole(Hole hole)
            => _holesOnScene.Add(hole);

        public void Clear()
            => _holesOnScene.Clear();

        public void DisappearHoles()
        {
            foreach (var hole in _holesOnScene)
            {
                hole.Disappear()
                    .OnComplete(() => hole.gameObject.SetActive(false));
            }
        }

        public bool IsAllHolesFillWithWater()
            => _holesOnScene.All(hole => hole.IsFillWithWater());

    }
}
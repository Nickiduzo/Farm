using System.Collections.Generic;
using UnityEngine;

namespace Tomato.Spawners
{
    public abstract class HolesContainer<T> : MonoBehaviour where T : BaseHole
    {
        [SerializeField] protected List<T> _holesOnScene;

        public List<T> HolesOnScene => _holesOnScene;
        
         /// <summary>
         /// Вводимо яму для моркви [carrotHole] - додаємо яму для моркви в список ям на сцені [_holesOnScene]
         /// </summary>
         /// <param name="carrotHole">яма для моркви</param>
        public void SetHole(T carrotHole)
            => _holesOnScene.Add(carrotHole);

        /// <summary>
        /// Очищає список ям для сцени[_holesOnScene]
        /// </summary>
        public void Clear()
            => _holesOnScene.Clear();

    }
}
using Carrot;
using Carrot.Spawners;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Класс взаимодействия с контейнером CarrotHolesContainer
    /// </summary>
    [CustomEditor(typeof(CarrotHolesContainer))]
    public class CarrotCollectData : UnityEditor.Editor
    {
        private const string CollectLabel = "Collect";
        private const string Clear = "Clear";

        /// <summary>
        /// Перезаписываемый метод для кнопок очистки контейнера и добавления ямы в контейнер
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CarrotHolesContainer container = (CarrotHolesContainer)target;

            if (GUILayout.Button(CollectLabel))
                foreach (var hole in FindObjectsOfType<CarrotHole>())
                    container.SetHole(hole);

            if (GUILayout.Button(Clear))
                container.Clear();

            EditorUtility.SetDirty(target);
        }
    }

}
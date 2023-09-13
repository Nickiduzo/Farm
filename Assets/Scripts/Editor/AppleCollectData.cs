using Apple;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Класс взаимодействия с контейнером AppleHolesContainer
    /// </summary>
    [CustomEditor(typeof(AppleHoleContainer))]
    public class AppleCollectData : UnityEditor.Editor
    {
        private const string CollectLabel = "Collect";
        private const string Clear = "Clear";
        /// <summary>
        /// Перезаписываемый метод для кнопок очистки контейнера и добавления ямы в контейнер
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AppleHoleContainer container = (AppleHoleContainer)target;

            if (GUILayout.Button(CollectLabel))
                foreach (var hole in FindObjectsOfType<AppleHole>())
                    container.SetHole(hole);

            if (GUILayout.Button(Clear))
                container.Clear();

            EditorUtility.SetDirty(target);
        }
    }
}


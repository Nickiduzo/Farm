using Tomato;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TomatoHolesContainer))]
    public class TomatoCollectData : UnityEditor.Editor
    {
        private const string CollectLabel = "Collect";
        private const string Clear = "Clear";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TomatoHolesContainer container = (TomatoHolesContainer)target;

            if (GUILayout.Button(CollectLabel))
                foreach (var hole in FindObjectsOfType<TomatoHole>())
                    container.SetHole(hole);

            if (GUILayout.Button(Clear))
                container.Clear();

            EditorUtility.SetDirty(target);
        }
    }
}
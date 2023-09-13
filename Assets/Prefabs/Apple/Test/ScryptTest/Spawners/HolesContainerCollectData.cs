using UnityEditor;
using UnityEngine;

namespace Apple.Spawners
{
#if UNITY_EDITOR
    [CustomEditor(typeof(HolesContainer))]
    public class HolesContainerCollectData : UnityEditor.Editor
    {
        private const string CollectLabel = "Collect";
        private const string Clear = "Clear";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HolesContainer container = (HolesContainer)target;

            if (GUILayout.Button(CollectLabel))
                foreach (var hole in FindObjectsOfType<Hole>())
                    container.SetHole(hole);

            if (GUILayout.Button(Clear))
                container.Clear();

            EditorUtility.SetDirty(target);
        }
    }
#endif
}
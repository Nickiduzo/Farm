using Carrot.Spawners;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MoleSpawner))]
    public class MoleCollectData : UnityEditor.Editor
    {
        private const string CollectLabel = "Collect";
        private const string Clear = "Clear";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            MoleSpawner spawner = (MoleSpawner)target;

            if (GUILayout.Button(CollectLabel))
                foreach (var hole in FindObjectsOfType<Carrot.CarrotHole>())
                    spawner.SetMoleSpawnPosition(hole.MoleSpawnPosition);

            if (GUILayout.Button(Clear))
                spawner.Clear();

            EditorUtility.SetDirty(target);
        }
    }

}
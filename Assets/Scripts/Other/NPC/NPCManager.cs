using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Props
{
    //реализация NPS (бабочка + птица):

    //бабочка
    //-создать контейнер который будет родителем для спот точек, создать спот точки
    //-создать объект Butterflyes для всех бабочек и  добавить на него скрипт NPCManager
    //-к объекту Butterflyes в children добавить префаб Buterfly (объект со скриптом Butterfly)
    //-в объекте Butterflyes в полях скрипта NPCManager указать NPS (префаб Buterfly (объект со скриптом Butterfly)) и в Positions добавить Transform всех спот точек для перемещения

    //птица
    //-создать контейнер который будет родителем для спот точек, создать спот точки
    //-создать объект Birds для всех бабочек и  добавить на него скрипт NPCManager
    //-к объекту Birds в children добавить префаб Bird (объект со скриптом Bird)
    //-в объекте Birds в полях скрипта NPCManager указать NPS (префаб Bird (объект со скриптом Bird)) и в Positions добавить Transform всех спот точек для перемещения


    public class NPCManager : MonoBehaviour
    {
        [SerializeField] private List<NPC> npcs = new();
        [SerializeField] private List<Transform> _points = new();

        private bool[] _pointUsedFlag;

        // Initialize the NPC objects and their event handlers
        private void Start()
        {
            _pointUsedFlag = new bool[_points.Count];
            InitNPCs();
        }

        // Unsubscribe from the event handlers when the script is destroyed
        private void OnDestroy()
        {
            foreach (var npc in npcs)
            {
                npc.OnFlewToPoint -= SetNewTarget;
            }
        }

        // Initialize the NPCs, subscribe to the event handler, and set the initial target
        private void InitNPCs()
        {
            foreach (var npc in npcs)
            {
                npc.OnFlewToPoint += SetNewTarget;
                npc.SetTarget(GetRandomPoint());
            }
        }

        // Set a new target for the NPC and reset the flag for the old target
        private void SetNewTarget(NPC npc, Transform oldTarget)
        {
            npc.SetTarget(GetRandomPoint());
            ResetFlag(_points.IndexOf(oldTarget));
        }

        // Get a random available point
        private Transform GetRandomPoint()
        {
            int pointIndex = GetFreeIndex();

            if (pointIndex == -1)
            {
                return null;
            }

            _pointUsedFlag[pointIndex] = true;
            return _points[pointIndex];
        }

        // Get the index of a free (unused) point
        private int GetFreeIndex()
        {
            // отримує лист невикористаних індексів
            List<int> spotsIndexes = new List<int>();
            for (int i = 0; i < _pointUsedFlag.Length;i++){
                if(!_pointUsedFlag[i])
                    spotsIndexes.Add(i);
            }
           // повертає випадковий індекс
            return spotsIndexes[Random.Range(0,spotsIndexes.Count)];
        }
        // Reset the flag for the specified index
        public void ResetFlag(int index)
            => _pointUsedFlag[index] = false;


    }
}
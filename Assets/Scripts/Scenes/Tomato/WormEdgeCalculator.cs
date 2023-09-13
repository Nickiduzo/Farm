using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Tomato
{
    public class WormEdgeCalculator : MonoBehaviour
    {
        [SerializeField] private List<Transform> _edges;
        [SerializeField] private List<int> _wormSpriteOrders;
        [SerializeField] private float _offset;

            private bool isMonitoring = false;
        private int index = 0;
        private Dictionary<Transform, List<Worm>> _wormsByEdge = new Dictionary<Transform, List<Worm>>();

        /// <summary>
        /// Вводимо черв'яка [worm]- призначає позиції кутів до черв'яку та повертає черв'яка [worm]
        /// </summary>
        public Worm CalculateBounceEdges(Worm worm)
        {
            if (index  < _edges.Count)
            {
                int wormSpriteOrder = _wormSpriteOrders[index];
                Transform edgeTrans = _edges[index];

                worm.GetComponent<SpriteRenderer>().sortingOrder = wormSpriteOrder;
                worm.SetNonDragSortingOrder(wormSpriteOrder);
                worm.transform.SetParent(edgeTrans);
                worm.transform.position = edgeTrans.position;
                worm.GetComponent<RandomMover>().Construct(edgeTrans.position.x - _offset, edgeTrans.position.x + _offset);
                index++;
            }
            return worm;
        }

        public void StartMonitoring()
        {
            if (isMonitoring)
                return;

            isMonitoring = true;
            InvokeRepeating("MonitoringNumbersOfWorms", 0.0f, 0.2f);
        }

        public void StopMonitoring()
        {
            isMonitoring = false;
            CancelInvoke("MonitoringNumbersOfWorms");
        }

        public void MonitoringNumbersOfWorms()
        {
            int edgesWithWormsCount = 0;

            foreach (Transform edge in _edges)
            {
                int numberOfChildren = edge.childCount;
                if (numberOfChildren > 0)
                {
                    edgesWithWormsCount++;
                }
            }

            foreach (Transform edge in _edges)
            {
                int numberOfChildren = edge.childCount;
                if (numberOfChildren > 0)
                {
                    foreach (Transform child in edge)
                    {
                        Worm wormScript = child.GetComponent<Worm>();
                        if (wormScript != null)
                        {
                            wormScript._onAdditionalAction = edgesWithWormsCount == 1;
                        }
                    }
                }
            }
        }
    }
}

using UnityEngine;

namespace SunflowerScene
{
    public class FlowView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _flow;
        [SerializeField] private SpriteRenderer _pipe;

        public void ShowFlow()
        {
            _flow.enabled = true;
            _pipe.enabled = true;
        }

        public void HideFlow()
        {
            _flow.enabled = false;
            _pipe.enabled = false;
        }
    }
}
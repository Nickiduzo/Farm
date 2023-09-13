using System;
using UnityEngine;

namespace SunflowerScene
{
    public class SpriteView : MonoBehaviour
    {
        [SerializeField] private Transform _body;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private SpriteStage[] _stages;

        //for the correct display of changes, you need to sort the list of stages by value from the largest to the smallest
        private void Awake()
        {
            SortSpriteStages();
        }

        //called to update the state based on the given progress value
        public void UpdateState(float progress)
        {
            foreach (var stage in _stages)
            {
                if (progress >= stage.Value)
                {
                    SetView(stage);
                    return;
                }
            }

        }

        //sets the view (sprite, scale, and position) based on the provided SpriteStage
        private void SetView(SpriteStage state)
        {
            _renderer.sprite = state.Sprite;
            _body.localScale = state.Scale;
            _body.localPosition = state.Position;
        }

        //sorts the _stages array in descending order based on the Value property of each stage
        private void SortSpriteStages()
        {
            Array.Sort(_stages, (a, b) => b.Value.CompareTo(a.Value));
        }
    }
}
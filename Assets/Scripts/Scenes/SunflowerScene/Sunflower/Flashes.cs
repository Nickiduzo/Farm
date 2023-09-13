using DG.Tweening;
using UnityEngine;

namespace SunflowerScene
{
    public class Flashes : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bodySpriteRenderer;
        [SerializeField] private SpriteRenderer _leavesSpriteRenderer;
        [SerializeField] private Color _color = Color.red;
        private Tween _tweenB,_tweenLeaf;
        private Color _defaultColor1;
        private Color _defaultColor2;
        //Sets default colors

        private void Awake()
        {
            _defaultColor1 = _bodySpriteRenderer.color;
            _defaultColor2 = _leavesSpriteRenderer.color;
        }
        // Sets the flashing effect of the sunflower based on the provided boolean value.

        public void SetFlashes(bool isFlash)
        {
            if(isFlash)
            {
                _tweenB = _bodySpriteRenderer.DOColor(_color, 1).SetLoops(-1, LoopType.Yoyo);
                _tweenLeaf = _leavesSpriteRenderer.DOColor(_color, 1).SetLoops(-1, LoopType.Yoyo);
                _tweenB.Play();
                _tweenLeaf.Play();
            }
            else
            {
                _tweenB.Kill();
                _tweenLeaf.Kill();
                _bodySpriteRenderer.DOColor(_defaultColor1, 1);
                _leavesSpriteRenderer.DOColor(_defaultColor2, 1);
            }
        }
    }
}
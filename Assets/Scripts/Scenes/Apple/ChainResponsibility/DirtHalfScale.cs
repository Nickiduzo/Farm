using DG.Tweening;
using System;
using UnityEngine;

    public class DirtHalfScale : BaseAction
    {
        private readonly Action OnActionDone;
        private readonly SpriteRenderer _dirt;
        private bool _isExecuted;

        public static bool HalfScaleOn = false;


        public DirtHalfScale(SpriteRenderer dirt, Action OnDone = null)
        {
            _dirt = dirt;
            OnActionDone = OnDone;
        }

        public override void Execute()
        {
            if (_isExecuted)
            {
                base.Execute();
                return;
            }

            if(HalfScaleOn)
            {
                HalfScale();
            }
            else
            {
                Disappear();
            }

            OnActionDone?.Invoke();
            _isExecuted = true;
            HalfScaleOn = false;
        }

        private Tween Disappear()
            => _dirt.DOFade(0, 1f).SetLink(_dirt.gameObject);
        
        private void HalfScale()
            => _dirt.transform.DOScale(_dirt.transform.localScale / 2, 1).SetLink(_dirt.gameObject);
    }
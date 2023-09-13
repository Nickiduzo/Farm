using DG.Tweening;
using UnityEngine;

namespace Carrot.ChainResponsibility
{
    public class AppearDirt : BaseAction
    {
        private readonly SpriteRenderer _dirt;
        private bool _isExecuted;

        // set sprite for dirt in hole
        public AppearDirt(SpriteRenderer dirt)
            => _dirt = dirt;

        // invoke "Execute" or [Appear();]
        public override void Execute()
        {
            if (_isExecuted)
            {
                base.Execute();
                return;
            }

            Appear();

            _isExecuted = true;
        }

        // appear dirt in hole when we water the hole
        private void Appear()
            => _dirt.DOFade(1, 0.6f);
    }
}
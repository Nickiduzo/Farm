using DG.Tweening;
using UnityEngine;

    public class DirtDisappear : BaseAction
    {
        private bool _isExecuted;
        private readonly SpriteRenderer _dirt;
        private readonly IDiggable _diggable;

        /// <summary>
        /// Вводимо "SpriteRenderer" ями [dirt] та елемент інтерфейсу "IDiggable" [diggable] -
        /// запам'ятовуємо параметри в відповідних полях
        /// </summary>        
        public DirtDisappear(SpriteRenderer dirt, IDiggable diggable)
        {
            _diggable = diggable;
            _dirt = dirt;
        }

        /// <summary>
        /// Викликає перегружену ф-цію "Execute()" або виконує ф-цію "Disappear"
        /// </summary>
        public override void Execute()
        {
            if (_isExecuted)
            {
                base.Execute();
                return;
            }

            Disappear()
                .OnComplete(_diggable.Dig);

            _isExecuted = true;
        }

        /// <summary>
        /// Ховає "SpriteRenderer" ями [_dirt]
        /// </summary>        
        private Tween Disappear()
            => _dirt.DOFade(0, 1f);
    }
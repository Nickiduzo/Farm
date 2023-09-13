using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SunflowerScene
{
    public class Sun : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private float _alpha;
        [SerializeField] private Transform _sunActivePosition;
        [SerializeField] private Transform _sunStartPosition;
        [SerializeField] List<SpriteRenderer> sunBeams = new List<SpriteRenderer>();
        [SerializeField] private float _sunshineDuration = 5f;

        public Action CallbackAction { get; private set; }

        // Initiates the sunshine effect on the sunflower, triggers the appearance and shine routine, and invokes the callback action when the shine is complete.
        public void Sunshine(Action OnEndRipe)
        {
            CallbackAction = OnEndRipe;
            Appear();
            StartCoroutine(ShineRoutine(_sunshineDuration));
        }

        // Coroutine that handles the shine routine for a specified duration.
        private IEnumerator ShineRoutine(float duration)
        {
            transform.DORotate(new Vector3(0, 0, 180), 100);
            foreach (var beam in sunBeams)
            {
                Shine(beam, duration, _alpha);
            }
            yield return new WaitForSeconds(duration);
            EndRipe();
        }

        // Hides the sunflower by moving it to the starting position and fading out the sun beams.
        private void Hide()
        {
            transform.DOMove(_sunStartPosition.position, 1f);
            foreach (var beam in sunBeams)
            {
                Shine(beam, 1f, 0);
            }
        }

        // Moves the sunflower to the active position, making it visible.
        private void Appear() => transform.DOMove(_sunActivePosition.position, 1f);

        // Shines a sun beam by adjusting its alpha value over a specified duration.
        private void Shine(SpriteRenderer beam, float duration, float alpha)
        {
            var color = beam.color;
            beam.DOColor(new Color(color.r, color.g, color.b, alpha), duration);
        }

        // Ends the ripening process by hiding the sunflower, and invoking the callback action.
        private void EndRipe()
        {
            Hide();

            CallbackAction?.Invoke();
        }
    }
}
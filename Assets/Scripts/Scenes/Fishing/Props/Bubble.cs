using DG.Tweening;
using System.Collections;
using UnityEngine;
namespace Fishing.Props
{
    public class Bubble : MonoBehaviour
    {
        private SpriteRenderer Renderer { get; set; }
        private Vector3 StartPos { get; set; }
        private Sequence sequence { get; set; }

        /// <summary>
        /// Запам'ятовуємо Renderer [Renderer] та початкову позиції [StartPos], викликає ф-цію "Play"
        /// </summary>
        private void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
            StartPos = this.transform.position;

            Play();
        }

        /// <summary>
        /// Викликає появу бульбашки з тривалістю 3 секунди, потім викликає ф-цію "Reset"
        /// </summary>
        private void Play()
        {
            sequence.Kill();
            sequence = DOTween.Sequence().SetLink(gameObject);
            sequence.Append(Renderer.DOFade(0.25f, 0.2f));
            sequence.Append(transform.DOMove(StartPos + RandomEndPoint(), 3)).Insert(1.2f, Renderer.DOFade(0, 1f));
            sequence.AppendCallback(Reset);
        }

        /// <summary>
        /// Повертає випадкову позицію
        /// </summary>        
        private Vector3 RandomEndPoint() => new(0, Random.Range(4, 6), 0);

        /// <summary>
        /// Повертає елементу значення появи та викликає "Coroutine" "RandomWaitRoutine"
        /// </summary>
        private void Reset()
        {
            this.transform.position = StartPos;
            if (this.isActiveAndEnabled)
                StartCoroutine(RandomWaitRoutine());
        }

        /// <summary>
        /// Очікує від 1 до 4 секунд і знову викликає ф-цію "Play"
        /// </summary>        
        private IEnumerator RandomWaitRoutine()
        {
            yield return new WaitForSeconds(Random.Range(1, 4));
            Play();
        }
    }
}
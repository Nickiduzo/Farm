using DG.Tweening;
using System;
using UnityEngine;

namespace SunflowerScene
{
    public class PlantGrowing : MonoBehaviour, ICountable<Transform>
    {
        [SerializeField] private float _progress;
        [SerializeField] private float _step;
        [SerializeField] private SpriteView _plantView;
        [SerializeField] private SpriteRenderer _grownedBody;
        [SerializeField] private Flashes _flashes;
        public bool Growing => _progress < 1;
        public Flashes Flashes => _flashes;
        public event Action<Transform> CountUp;

        // Processes the watering of the plant, advancing its growth progress.
        public void ProcessWatering()
        {
            _progress += _step;
            _plantView.UpdateState(_progress);

            if (Growing == false)
            {
                OnPlantGrew();
            }
        }

        // Handles the event when the plant has fully grown.
        private void OnPlantGrew()
        {
            _plantView.gameObject.SetActive(false);
            transform.localScale = Vector3.one * 0.7f;
            _grownedBody.gameObject.SetActive(true);
            transform.DOScale(Vector3.one, 0.3f);
            CountUp?.Invoke(transform);
        }
    }
}
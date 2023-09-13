using UnityEngine;
using System.Collections;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace ChickenScene.Entities
{
    public class ChickenEyesAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _eyesSpriteRenderer;
        [SerializeField] private GameObject _bothEyes;
        [SerializeField] private Transform _rightEye;
        [SerializeField] private Transform _leftEye;
        [SerializeField] private Transform _eyeRightDestinationPoint;
        [SerializeField] private Transform _eyeLeftDestinationPoint;
        [SerializeField] private Sprite[] _closedEyesSprites;
        [SerializeField] private Sprite[] _openEyesSprites;

        private Vector3 _defaultPosLeftEye;
        private Vector3 _defaultPosRightEye;
        private Sprite _chosenSprite;

        public bool _canPlayOtherAnim = true;


        // select random delay to start blinking
        private void Start()
        {
            SetDefaultPosForEyes();
            _eyesSpriteRenderer = GetComponent<SpriteRenderer>();
            Invoke("CoroutineController", Random.Range(1, 8));
        }

        // save default eyes position
        private void SetDefaultPosForEyes()
        {
            _defaultPosLeftEye = _leftEye.localPosition;
            _defaultPosRightEye = _rightEye.localPosition;
        }

        // uses to avoid animations overlap and restart blink anim
        private void CoroutineController()
        {
            StopCoroutine(StartBlinking());
            StartCoroutine("StartBlinking");
        }

        // change direction of chicken's eyes
        public void LookToCenter()
        { 
            _canPlayOtherAnim = false;
            _leftEye.DOLocalMove(_eyeLeftDestinationPoint.localPosition, 1f);
            _rightEye.DOLocalMove(_eyeRightDestinationPoint.localPosition, 1f);
        }

        // return eyes to default position
        public void MoveToDefaultEyesPosition()
        {
            _canPlayOtherAnim = true;
            _leftEye.DOLocalMove(_defaultPosLeftEye, 1f);
            _rightEye.DOLocalMove(_defaultPosRightEye, 1f);
        }

        private IEnumerator StartBlinking()
        {
            string objectName = _eyesSpriteRenderer.name;

            // choose right eye color for close/open eyes animation based on eyes color
            switch (objectName)
            {
                case "EyesBrown":
                    _eyesSpriteRenderer.sprite = _closedEyesSprites[0];
                    _chosenSprite = _openEyesSprites[0];
                    break;
                case "EyesGray":
                    _eyesSpriteRenderer.sprite = _closedEyesSprites[1];
                    _chosenSprite = _openEyesSprites[1];
                    break;
                case "EyesWhite":
                    _eyesSpriteRenderer.sprite = _closedEyesSprites[2];
                    _chosenSprite = _openEyesSprites[2];
                    break;
                case "EyesOrange":
                    _eyesSpriteRenderer.sprite = _closedEyesSprites[3];
                    _chosenSprite = _openEyesSprites[3];
                    break;
            }

            _bothEyes.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            _eyesSpriteRenderer.sprite = _chosenSprite; // apply chosen sprite for open eyes
            _bothEyes.SetActive(true);

            yield return new WaitForSeconds(Random.Range(1, 6)); // wait for next anim
            CoroutineController();
        }
    }
}


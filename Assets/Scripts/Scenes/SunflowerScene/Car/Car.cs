using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace SunflowerScene
{
    public class Car : MonoBehaviour
    {
        [SerializeField] private MoverToTarget _moverToTarget;
        [SerializeField] private Transform[] _sunflowerSpawnPoints;
        [SerializeField] private SpriteRenderer[] eyes;

        public MoverToTarget Mover => _moverToTarget;
        public Transform[] SunflowerSpawnPoint => _sunflowerSpawnPoints;
        [SerializeField] bool eventEyes;
        private int _eyeNumPos = -1;
        private Vector2[] _localPosXY;
        private float _moveDistX = .068f, _moveDistY = .065f;
        private WaitForSeconds _waitForSeconds = new WaitForSeconds(2.2f);
        private bool _isMoving = true;
        private IEnumerator _enumeratorEyes,_eWheels;
        public void Construct(Transform endPoint, Transform rotatePoint) => Mover.Construct(endPoint, rotatePoint);
        private void Start()
        {
            if (eventEyes)
            {
                _localPosXY = new Vector2[2];
                for (int i = 0; i < 2; i++)
                {
                    _localPosXY[i].x = eyes[i].transform.localPosition.x;
                    _localPosXY[i].y = eyes[i].transform.localPosition.y;
                }

                _enumeratorEyes = EyeIdleMove();
                StartCoroutine(_enumeratorEyes);

                
                _moverToTarget.EndMove += IsIdle;
                _moverToTarget.StartMove += IsMoving;
            }
        }

        private void IsMoving() => _isMoving = true;
        private void IsIdle() => _isMoving = false;

        private void NextEyePose()
        {
            switch (_eyeNumPos)
            {
                case 0:
                    EyesDoMoveXY(_moveDistX);
                    break;
                case 1:
                    EyesDoMoveXY();
                    _eyeNumPos = -1;
                    break;
            }

            _eyeNumPos++;
        }

        private IEnumerator EyeIdleMove()
        {
            NextEyePose();
            yield return _waitForSeconds;
            StartCoroutine(EyeIdleMove());
        }

        private void EyesDoMoveXY(float offset = 0)
        {
            float randx = Random.Range(.03f, offset);
            if (!_isMoving)
                randx *= -1;
            float randy = Random.Range(-_moveDistY, _moveDistY);
            for (int i = 0; i < 2; i++)
            {
                eyes[i].transform.DOLocalMoveX(_localPosXY[i].x + randx, 1.4f);
                eyes[i].transform.DOLocalMoveY(_localPosXY[i].y + randy, 1.4f);
            }
        }

        private void OnDestroy()
        {
            StopCoroutine(_enumeratorEyes);

            _moverToTarget.EndMove -= IsIdle;
            _moverToTarget.StartMove -= IsMoving;
        }
    }
}
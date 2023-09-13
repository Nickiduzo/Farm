using DG.Tweening;
using AwesomeTools.Inputs;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Tomato
{
    public class WaterPump : MonoBehaviour
    {
        private const float WAIT_ROTATION_TIME = 0.15f;

        [SerializeField] private Animator _waterStreamAnimator;
        [SerializeField] private float _tomatoSpawnValue;
        [SerializeField] private WaterPumpStreamPrefab _waterStream;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private TomatoSprinkler _tomatoSprinkler;

        private Vector3 _destination;
        private Vector3 _start;
        private bool _isRotateFinished = true;

        private TomatoMediator _tomatoMediator;

        private Tween _enableWaterTween;

        public void Construct(Vector3 destination, Vector3 start, InputSystem input)
        {
            _tomatoMediator = GameObject.Find("TomatoLevelController").GetComponent<TomatoMediator>();

            _start = start;
            _destination = destination;
            _dragAndDrop.Construct(input);
            _destinationOnDragEnd.Construct(destination);

            _dragAndDrop.OnDragStart += HideHint;
            _dragAndDrop.OnDragEnded += DeActivateWaterStreamCollider;

            _tomatoSprinkler.Initialize(_dragAndDrop, StartDrag, EndDrag, SeedTree);
            _tomatoSprinkler.InitStream();
        }

        private void OnDestroy()
        {
            _dragAndDrop.OnDragEnded -= DeActivateWaterStreamCollider;
        }

        // The HideHint method hides the pointer hint in the hint system
        private void HideHint()
        {
            HintSystem.Instance.HidePointerHint();
            _dragAndDrop.OnDragStart -= HideHint;
        }

        // The DeActivateWaterStreamCollider method disables the water stream and resets the rotation of the object
        private void DeActivateWaterStreamCollider()
        {
            _enableWaterTween.Kill();
            transform.DORotate(new Vector3(0, 0, 0), WAIT_ROTATION_TIME);
            _isRotateFinished = true;
            _waterStream.DisableWater();
        }

        public void EndLifeCycle()
        {
            DOTween.Kill(gameObject);
            MoveTo(_start);
            DeActivateWaterStreamCollider();
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
        }

        // The MoveToDestination method moves the object to its destination point
        public void MoveToDestination()
        {
            MoveTo(_destination);
            DeActivateWaterStreamCollider();
        }
        // The MoveTo method moves the object to the specified target point
        private void MoveTo(Vector3 targetPoint)
        {
            transform.DOMove(targetPoint, 1f);
        }

        // The StartDrag method handles the start of dragging, including rotation, enabling water flow, and animation
        private void StartDrag(TomatoSeed hole = null)
        {
            if (_dragAndDrop.IsDraggable == false)
            {
                _isRotateFinished = true;
                _waterStream.DisableWater();
                return;
            }

            if (!_isRotateFinished)
            {
                return;
            }

            _isRotateFinished = false;
            _enableWaterTween = DOTween.Sequence().Append(transform.DORotate(new Vector3(0, 0, 35f), WAIT_ROTATION_TIME))
                .OnComplete(() =>
                {
                    _waterStream.EnableWater();
                    _isRotateFinished = true;
                });
        }

        // The EndDrag method handles the end of dragging, including disabling water flow, animation, and rotation reset
        private void EndDrag(TomatoSeed hole = null)
        {
            //if (_dragAndDrop.IsDraggable)
            //{
            //    DeActivateWaterStreamCollider();
            //    return;
            //}


            if (!_isRotateFinished)
            {
                return;
            }
            DeActivateWaterStreamCollider();

            //_isRotateFinished = false;
            //_waterStream.DisableWater();
            //_disableWaterTween = DOTween.Sequence().Append(transform.DORotate(new Vector3(0, 0, 0), WAIT_ROTATION_TIME))
            //    .OnComplete(() =>
            //    {
            //        _isRotateFinished = true;
            //    });
        }

        // The SeedTree method seeds a tomato tree if the water stream is working
        private void SeedTree(TomatoSeed tomatoSeed)
        {
            if (!_waterStream.IsWorking())
            {
                return;
            }

            _tomatoMediator.CreateTomato(tomatoSeed, _tomatoSpawnValue * Time.fixedDeltaTime);
        }

    }
}
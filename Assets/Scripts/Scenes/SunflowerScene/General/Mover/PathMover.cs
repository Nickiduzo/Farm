using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;

namespace SunflowerScene
{
    public class PathMover : MonoBehaviour
    {
        [SerializeField] private float _duration = 5;
        [SerializeField] private float _fleeDuration = 1f;

        private Path _path;
        private Vector3 _startPosition;
        [HideInInspector] public Vector2 endPath;
        private TweenerCore<Vector3, DG.Tweening.Plugins.Core.PathCore.Path, PathOptions> _moveTween;

        public event Action UltimateGoalAchieved;

        //Init path for PathMover
        public void Construct(Path path)
        {
            _path = path;
        }

        // Initiates the movement along the specified path and triggers an action upon reaching the ultimate goal.
        public void Move()
        {
            MoveInPath(_path.GetPath(), OnUltimateGoalAchieved);
        }


        // Moves the object along the provided path and executes the specified action upon completion.
        private void MoveInPath(Vector3[] path, TweenCallback action)
        {
            Flip(path[^1]);
            _startPosition = path[0];
            _moveTween = transform.DOPath(path, _duration).OnComplete(action);
            endPath = path[^1];
        }

        // Stops the current movement and exits the scene.
        public void StopMove()
        {
            if (_moveTween != null)
            {
                _moveTween.Kill();
            }
            
            ExitScene();
        }

        // Exits the scene by moving back to the start position and deactivates the game object.
        private void ExitScene()
        {
            Flip(_startPosition);
            transform.DOMove(_startPosition, _fleeDuration/.4f)
                .OnComplete(() => gameObject.SetActive(false));
        }

        // Flips the object's rotation towards the target position.
        private void Flip(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            float angle = Vector3.Angle(direction, Vector3.right);
            if (direction.y < 0)
            {
                angle = 360f - angle;
            }

            transform.DORotate(new Vector3(0f, angle, 0f), 0.1f);
        }

        // Invokes the event for achieving the ultimate goal.
        protected virtual void OnUltimateGoalAchieved()
        {
            UltimateGoalAchieved?.Invoke();
        }
    }
}
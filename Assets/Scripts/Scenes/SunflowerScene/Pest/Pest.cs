using System;
using UnityEngine;
using AwesomeTools;

namespace SunflowerScene
{
    public class Pest : MonoBehaviour, ICountable<Transform>
    {
        [SerializeField] private PathMover _pathMover;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private VerticalPositionLayerSwitcher _layerSwitcher;
        [SerializeField] private Animator anim;
        [SerializeField] private AudioSource audio;

        private readonly int Eat = Animator.StringToHash("eat");
        private int _sortingLayerID;
        
        public PathMover PathMover => _pathMover;
        public event Action<Transform> CountUp;

        // set start position, order in layer, sorting layer and subscribe to a few Actions 
        public void Construct(Path path)
        {
            _sortingLayerID = SortingLayer.NameToID("Boat");
            _mouseTrigger.OnDown += Die;
            transform.position = path.GetStartPosition();
            _pathMover.Construct(path);
            _pathMover.Move();
            _pathMover.UltimateGoalAchieved += PathCompletedHandle;
            _layerSwitcher.SwitchSortingOrder(path.VerticalPosition);
            
        }

        // invoke when pest is caught, stop eat animation and pest will leave scene
        private void Die()
        {
            GetComponent<Collider2D>().enabled = false;
            _pathMover.StopMove();
            OnDied();
            _layerSwitcher.ChangeSortingLayer(_sortingLayerID);
            anim.SetBool(Eat, false);
        }

        // invoke Action [CountUp]
        private void OnDied()
        {
            CountUp?.Invoke(transform);
        }

        // set new "_layerSwitcher._index" based on index 
        public void StartChangingSortingOrder(int index)
        {
            _layerSwitcher._index = index;
            _layerSwitcher.ChangeSortingOrder();
        }

        //if a crow is added and here you can toggle the animation for it
        private void PathCompletedHandle()
        {
            Debug.Log("Change behaviour");
            anim.SetBool(Eat, true);
            audio.Stop();
        }

        private void OnDisable()
        {
            _pathMover.UltimateGoalAchieved -= PathCompletedHandle;
            _mouseTrigger.OnDown -= Die;
        }
    }
}
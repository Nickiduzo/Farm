using DG.Tweening;
using System;
using UnityEngine;

namespace CowScene
{
    public class CowController : MonoBehaviour
    {
        public event Action CowFullyFed;
        public event Action CowFullyMilked;
        public event Action CowWalkedAway;

        [SerializeField] private CowLevelConfig _config;

        private Cow _cow;
        private int _countOfAction;
        private int _goalFeedingCount;
        private int _goalMilkingCount;

        public Cow Cow => _cow;

        // Initializes the cow controller with the specified count of actions and cow
        public void InitiateCowController(int countOfAction, Cow cow)
        {
            _cow = cow;
            _countOfAction = countOfAction;
            _cow.CowFed += OnCowFed;
        }

        // Event handler for when the cow is fed
        public void OnCowFed()
        {
            if (++_goalFeedingCount == _countOfAction)
            {
                CowFullyFed?.Invoke();
                _cow.CowFullyFed();

                _cow.CowFed -= OnCowFed;
                _cow.CowMilked += OnCowMilked;
            }
        }

        // Event handler for when the cow is milked
        public void OnCowMilked()
        {
            if (++_goalMilkingCount == _countOfAction)
            {
                CowFullyMilked?.Invoke();
                _cow.EndMilking();
                _cow.GoToEnd().OnComplete(() =>
                {
                    CowWalkedAway?.Invoke();
                });
            }
        }

    }
}

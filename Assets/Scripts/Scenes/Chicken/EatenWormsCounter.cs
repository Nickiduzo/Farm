using System;
using UI;

namespace ChickenScene
{
    public class EatenWormsCounter : IProgressWriter
    {
        private int _warmsToEat;
        private int _currentEatenWarmAmount;

        public int CurrentProgress => _currentEatenWarmAmount;
        public int MaxProgress => _warmsToEat;
        
        public event Action OnProgressChanged;
        public event Action OnHalfWormsEaten;
        public event Action OnAllWormsEaten;
        

        // set how many Eaten Worms are need to start spawn eggs (numbers is get from config)
        public EatenWormsCounter(int warmsToEat)
        {
            _warmsToEat = warmsToEat;
        }
        
        // after invoke [OnAllWormsEaten] chickens become ready to spawn eggs
        public void UpdateProgress()
        {
            _currentEatenWarmAmount++;
            OnProgressChanged?.Invoke();
            if (_currentEatenWarmAmount == _warmsToEat/2)
            {
                OnHalfWormsEaten?.Invoke();
            }
            else if(_currentEatenWarmAmount == _warmsToEat)
            {
                OnAllWormsEaten?.Invoke();
            }
        }
    }
}
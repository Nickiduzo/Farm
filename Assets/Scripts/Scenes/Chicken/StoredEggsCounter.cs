using System;
using UI;

namespace ChickenScene
{
    public class StoredEggsCounter : IProgressWriter,IWinInvoker
    {
        private int _eggsToStore;
        private int _currentEggsStoreAmount;
        
        public int CurrentProgress => _currentEggsStoreAmount;
        public int MaxProgress  => _eggsToStore;
        
        public event Action OnAllEggsStored;
        public event Action OnProgressChanged;
        public event Action OnWin;


        // set how many stored eggs need to win (numbers is get from config)
        public StoredEggsCounter(int eggsToStore) 
        {
            _eggsToStore = eggsToStore;
        }
        
        // invoke WinPanel if number of collected eggs is enough
        public void UpdateProgress()
        {
            _currentEggsStoreAmount++;
            OnProgressChanged?.Invoke();
            if(_currentEggsStoreAmount == _eggsToStore)
            {
                OnAllEggsStored?.Invoke();
                OnWin?.Invoke();
            }
        }
    }
}
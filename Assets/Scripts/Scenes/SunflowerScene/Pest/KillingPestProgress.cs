using System;
using UI;

namespace SunflowerScene
{
    public class KillingPestProgress : IProgressWriter
    {
        private int _allKilledPest;
        public event Action OnProgressChanged;
        public int CurrentProgress => _allKilledPest;
        public int MaxProgress { get; }

        public KillingPestProgress(int needKill)
        {
            MaxProgress = needKill;
        }

        // Increases the count of killed pests and triggers the progress changed event.
        public void AddKilledPest()
        {
            _allKilledPest++;
            ProgressChanged();
        }

        // Triggers the progress changed event.
        private void ProgressChanged()
        {
            OnProgressChanged?.Invoke();
        }
    }
}
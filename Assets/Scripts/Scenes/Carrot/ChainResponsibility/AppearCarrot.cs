namespace Carrot.ChainResponsibility
{
    public class AppearCarrot : BaseAction
    {
        private readonly IWaterFillable _fillWater;
        private bool _isExecuted;

        // set interface 
        public AppearCarrot(IWaterFillable fillWater)
        {
            _fillWater = fillWater;
        }

        // invoke "Execute" or [FillWater()]
        public override void Execute()
        {
            if (_isExecuted)
            {
                base.Execute();
                return;
            }

            _fillWater.FillWater();
            _isExecuted = true;
        }
    }
}
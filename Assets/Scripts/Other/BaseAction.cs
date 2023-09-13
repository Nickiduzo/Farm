public abstract class BaseAction : IChainAction
{   
    // is common to such scenes as CarrotScene, TomatoeScene, Apple
    private IChainAction _nextAction;

    /// <summary>
    /// Вводимо подію [action] - запам'ятовуємо подію в полі "_nextAction"
    /// </summary>    
    public IChainAction SetNextAction(IChainAction action)
        => _nextAction = action;

    /// <summary>
    /// Подія виконує ф-цію "Execute" 
    /// </summary>
    public virtual void Execute()
    {
        if (_nextAction != null)
            _nextAction.Execute();
    }
}

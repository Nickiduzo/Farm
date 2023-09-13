public interface IChainAction
{
    // uses to move to next action while digging or watering, for example, carrot appears or dirt disappears while digging 
    IChainAction SetNextAction(IChainAction action);        
    void Execute();
}
namespace Practice
{
    public interface ICommand
    {
        void Execute();
        void Undo();
        float GetEffectDuration();
    }
}
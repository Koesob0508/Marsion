namespace Marsion
{
    public interface ITrigger
    {
        void Register();
        void Unregister();
        void Execute();
    }
}
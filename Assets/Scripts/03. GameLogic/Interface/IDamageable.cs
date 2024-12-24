namespace Marsion.Logic
{
    public interface IDamageable
    {
        public int Attack { get; }
        void TakeDamage(int amount);
    }
}
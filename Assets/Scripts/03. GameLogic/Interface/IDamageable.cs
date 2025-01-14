namespace Marsion.Logic
{
    public interface IDamageable
    {
        public int Power { get; }
        void TakeDamage(int amount);
    }
}
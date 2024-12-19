namespace Marsion.Logic
{
    public interface IDamageable
    {
        public int Attack { get; }
        void Damage(int amount);
    }
}
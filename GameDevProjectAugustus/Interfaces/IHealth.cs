namespace GameDevProjectAugustus.Interfaces
{
    public interface IHealth
    {
        int CurrentHealth { get; }
        bool IsAlive { get; }
        void TakeDamage(int amount);
        void Heal(int amount);
    }
}
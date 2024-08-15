namespace GameDevProjectAugustus.Interfaces
{
    public interface IHealth
    {
        int CurrentHealth { get; }
        int MaxHealth { get; } // Add this property
        bool IsAlive { get; }
        void TakeDamage(int amount);
        void Heal(int amount);
    }
}
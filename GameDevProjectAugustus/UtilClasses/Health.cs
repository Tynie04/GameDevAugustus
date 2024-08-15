using GameDevProjectAugustus.Interfaces;

namespace GameDevProjectAugustus.Classes
{
    public class Health : IHealth
    {
        private int _currentHealth;
        private readonly int _maxHealth;

        public int CurrentHealth => _currentHealth;
        public bool IsAlive => _currentHealth > 0;

        public Health(int maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
        }

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;
            if (_currentHealth < 0) _currentHealth = 0;
        }

        public void Heal(int amount)
        {
            _currentHealth += amount;
            if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
        }
    }
}
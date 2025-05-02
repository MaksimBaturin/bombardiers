namespace Game.Scripts
{
    public interface IHealth
    {
        public void TakeDamage(float damage);

        public void Die();

        public float GetHealthPercentage();
    }
}
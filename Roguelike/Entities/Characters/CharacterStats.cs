namespace Roguelike.Entities.Characters
{
    public struct CharacterStats
    {
        public float MaxHealth;
        public float Health;
        public float KnockbackScaling;
        public bool Invulnerable;
        public float TimeScale;

        public CharacterStats() : this(1, 1, 1) { }
        public CharacterStats(float maxHealth, float health, float knockbackScaling = 1, bool invulnerable = false, float timescale = 1)
        {
            MaxHealth = maxHealth;
            Health = health;
            KnockbackScaling = knockbackScaling;
            Invulnerable = invulnerable;
            TimeScale = timescale;
        }
        public CharacterStats(float maxHealth, float knockbackScaling = 1, bool invulnerable = false, float timescale = 1) : this(maxHealth, maxHealth, knockbackScaling, invulnerable, timescale) { }
    }
}

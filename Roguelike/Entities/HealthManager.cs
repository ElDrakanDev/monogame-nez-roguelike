using System;
using Microsoft.Xna.Framework;
using Nez;
using Roguelike.Entities.Stats;

namespace Roguelike.Entities
{
    public class DamageInfo
    {
        public float Damage;
        public Vector2 Knockback;
        public object Source;
        public bool Canceled = false;

        public DamageInfo(float damage, Vector2 knockback, object source)
        {
            Damage = damage;
            Knockback = knockback;
            Source = source;
        }
    }

    public class HealInfo
    {
        public float Amount;
        public object Source;

        public HealInfo(float amount, object source)
        {
            Amount = amount;
            Source = source;
        }
    }
    public class DeathInfo
    {
        public bool Canceled = false;
        public object Source;

        public DeathInfo(object source)
        {
            Source = source;
        }
    }
    public class HealthManager : Component
    {
        float _health;
        public readonly Stat MaxHealth;
        public float Health
        {
            get => _health;
            private set
            {
                if (value < 0) _health = 0;
                else if (value > MaxHealth) _health = MaxHealth;
                else _health = value;
            }
        }
        public HealthManager(float health, float maxHealth, float minMaxHealth = 1)
        {
            MaxHealth = new Stat(maxHealth, Entity, minMaxHealth);
            Health = health;
        }
        public HealthManager(float health, Stat maxHealth)
        {
            MaxHealth = maxHealth;
            Health = health;
        }
        public HealthManager(float maxHealth, float minMaxHealth = 1)
        {
            MaxHealth = new Stat(maxHealth, Entity, minMaxHealth);
            Health = MaxHealth;
        }
        public HealthManager(Stat maxHealth)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
        }

        /// <summary>
        /// Pre damage taken event.
        /// </summary>
        public event Action<DamageInfo> preDamageTaken;
        /// <summary>
        /// Event invoked after taking damage
        /// </summary>
        public event Action<DamageInfo> onDamageTaken;
        /// <summary>
        /// Pre healing event
        /// </summary>
        public event Action<HealInfo> preHeal;
        /// <summary>
        /// Pre death event
        /// </summary>
        public event Action<DeathInfo> preDeath;
        /// <summary>
        /// Event invoked when entity dies
        /// </summary>
        public event Action<DeathInfo> onDeath;

        public void Hit(DamageInfo info)
        {
            preDamageTaken?.Invoke(info);
            if (info.Canceled is false) {
                Health -= info.Damage;
                onDamageTaken?.Invoke(info);
                if (Health == 0) Die(new DeathInfo(info.Source));
            }
        }
        public void Heal(HealInfo info)
        {
            preHeal?.Invoke(info);
            Health += info.Amount;
        }
        public void Die(DeathInfo info)
        {
            preDeath?.Invoke(info);
            if(info.Canceled is false) onDeath?.Invoke(info);
        }
    }
}

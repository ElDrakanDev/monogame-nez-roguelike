using Microsoft.Xna.Framework;

namespace Roguelike.Entities.Projectiles
{
    public struct ProjectileStats
    {
        public float Damage;
        public int Bounces;
        public int Pierces;
        public Vector2 Velocity;
        public float TimeScale = 1;
        public bool GroundCollide = true;
        public float LifeTime;
        public float Force;
        public int TargetTeams;
        public float Speed { get => Velocity.Magnitude(); set => Velocity = Velocity.Normalized() * value; }
        public Vector2 KnockBack { get => Velocity.Normalized() * Force; }

        public ProjectileStats(float damage, Vector2 velocity, float lifeTime, float force, int targetTeams, bool groundCollide = true, int bounces = 0, int pierces = 0)
        {
            Damage = damage;
            Velocity = velocity;
            Bounces = bounces;
            Pierces = pierces;
            GroundCollide = groundCollide;
            LifeTime = lifeTime;
            Force = force;
            TargetTeams = targetTeams;
        }
    }
}

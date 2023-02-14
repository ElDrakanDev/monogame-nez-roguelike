using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Roguelike.World;

namespace Roguelike.Entities.Projectiles
{
    public class Projectile : Entity, IUpdatable, ITimeScalable, ILifeTimed
    {
        public static List<Projectile> Projectiles = new List<Projectile>();
        public readonly List<Collider> HitEntities = new();
        public ProjectileStats Stats;
        public Vector2 Size { get; set; }
        public float LifeTime { get => Stats.LifeTime; set => Stats.LifeTime = value; }
        public Vector2 Velocity { get => Stats.Velocity; set => Stats.Velocity = value; }
        public float TimeScale { get => Stats.TimeScale; set => Stats.TimeScale = value; }
        public BoxCollider Collider { get; private set; }

        TiledMapMover.CollisionState collisionState = new();

        public Projectile(Vector2 position, ProjectileStats stats, Vector2 size)
        {
            Position = position;
            Size = size;
            Stats = stats;
            collisionState.ShouldTestPlatforms = false;
        }
        public override void OnAddedToScene()
        {
            base.OnAddedToScene();
            Collider = AddComponent(new BoxCollider(Size.X, Size.Y));
            Collider.PhysicsLayer = (int)LayerMask.Projectile;
            Collider.CollidesWithLayers = (int)LayerMask.Character;
            Projectiles.Add(this);
        }
        public override void OnRemovedFromScene()
        {
            base.OnRemovedFromScene();

            Projectiles.Remove(this);
        }
        public override void Update()
        {
            base.Update();

            TickLifeTime();
            Move();
            NotifyCollisions();
        }

        void Move()
        {
            if (Stats.GroundCollide)
            {
                Level.Instance.TiledMapMover.Move(Velocity, Collider, collisionState);
                if (collisionState.HasCollision) OnWallCollision();
            }
            else
            {
                Position += Velocity * Time.DeltaTime * TimeScale;
            }
        }

        protected virtual void OnWallCollision()
        {
            if(Stats.Bounces == 0)
                Destroy();
            else
            {
                HitEntities.Clear();
                Stats.Bounces--;
                if (collisionState.Left || collisionState.Right) Velocity = new Vector2(-Velocity.X, Velocity.Y);
                if (collisionState.Above || collisionState.Below) Velocity = new Vector2(Velocity.X, -Velocity.Y);
            }
        }

        public void TickLifeTime()
        {
            LifeTime -= Time.DeltaTime * TimeScale;
            if (LifeTime <= 0) OnLifeTimeEnd();
        }

        public void OnLifeTimeEnd()
        {
            Destroy();
        }

        protected virtual void NotifyCollisions()
        {
            var neighbors = Physics.BoxcastBroadphase(Collider.Bounds, Collider.CollidesWithLayers);
            foreach (var neighbor in neighbors)
            {
                if (neighbor.Enabled && HitEntities.Contains(neighbor) is false && Collider.Overlaps(neighbor) && neighbor.Entity.TryGetComponent(out HealthManager healthManager))
                {
                    healthManager.Hit(new DamageInfo(Stats.Damage, Stats.KnockBack, this));
                    HitEntities.Add(neighbor);
                }
            }
        }
    }
}

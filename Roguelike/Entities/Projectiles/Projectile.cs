﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using Roguelike.World;

namespace Roguelike.Entities.Projectiles
{
    public class Projectile : Component, IUpdatable, ITimeScalable, ILifeTimed
    {
        public static readonly List<Projectile> Projectiles = new();
        public readonly List<Collider> HitEntities = new();
        public ProjectileStats Stats;
        public Vector2 Size { get; set; }
        public float LifeTime { get => Stats.LifeTime; set => Stats.LifeTime = value; }
        public Vector2 Velocity { get => Stats.Velocity; set => Stats.Velocity = value; }
        public float TimeScale { get => Stats.TimeScale; set => Stats.TimeScale = value; }
        public BoxCollider Collider { get; private set; }

        TiledMapMover _mapMover => Level.Instance.TiledMapMover;
        public TiledMapMover.CollisionState CollisionState = new();

        public Projectile(ProjectileStats stats, Vector2 size)
        {
            Stats = stats;
            Size = size;
        }
        public static Projectile Create(Projectile projectile, Vector2 position)
        {
            Core.Scene.AddEntity(new()).AddComponent(projectile);
            projectile.Entity.Position = position;
            projectile.CollisionState.ShouldTestPlatforms = false;
            return projectile;
        }
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            Collider = Entity.AddComponent(new BoxCollider(Size.X, Size.Y));
            Collider.PhysicsLayer = (int)LayerMask.Projectile;
            Collider.CollidesWithLayers = (int)LayerMask.Character;
            Projectiles.Add(this);
        }
        public override void OnRemovedFromEntity()
        {
            base.OnRemovedFromEntity();

            Projectiles.Remove(this);
        }
        public void Update()
        {
            TickLifeTime();
            Move();
            NotifyCollisions();
        }

        void Move()
        {
            if (Stats.GroundCollide)
            {
                _mapMover.Move(Velocity, Collider, CollisionState);
                if (CollisionState.HasCollision) OnWallCollision();
            }
            else
            {
                Entity.Position += Velocity * Time.DeltaTime * TimeScale;
            }
        }

        protected virtual void OnWallCollision()
        {
            if (Stats.Bounces <= 0)
                Entity.Destroy();
            else
            {
                HitEntities.Clear();
                if (CollisionState.Left || CollisionState.Right)
                {
                    Velocity = new Vector2(-Velocity.X, Velocity.Y);
                    Stats.Bounces--;

                }
                if (CollisionState.Above || CollisionState.Below)
                {
                    Velocity = new Vector2(Velocity.X, -Velocity.Y);
                    Stats.Bounces--;
                }
            }
        }

        public void TickLifeTime()
        {
            LifeTime -= Time.DeltaTime * TimeScale;
            if (LifeTime <= 0) OnLifeTimeEnd();
        }

        public void OnLifeTimeEnd()
        {
            Entity.Destroy();
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
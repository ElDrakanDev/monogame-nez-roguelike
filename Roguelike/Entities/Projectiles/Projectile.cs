﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using Roguelike.Entities.Characters;
using Roguelike.World;

namespace Roguelike.Entities.Projectiles
{
    public class Projectile : Component, IUpdatable, ITimeScalable, ILifeTimed
    {
        public static readonly List<Projectile> Projectiles = new();
        public readonly List<Collider> HitEntities = new();
        public ProjectileStats Stats;
        public Vector2 Size { get => Collider.Bounds.Size; set => Collider.SetSize(value.X, value.Y); }
        public float LifeTime { get => Stats.LifeTime; set => Stats.LifeTime = value; }
        public Vector2 Velocity { get => Stats.Velocity; set => Stats.Velocity = value; }
        public float TimeScale { get => Stats.TimeScale; set => Stats.TimeScale = value; }
        public int Pierces { get => Stats.Pierces; set { Stats.Pierces = value; if (value < 1) Die(); } }
        public int TargetTeams
        {
            get => Stats.TargetTeams;
            set
            {
                Stats.TargetTeams = value;
                Core.StartCoroutine(SwitchTeamMaterial());
            }
        }
        public BoxCollider Collider { get; private set; }
        public Character Owner;

        TiledMapMover _mapMover => Level.Instance.TiledMapMover;
        public TiledMapMover.CollisionState CollisionState = new();

        public Projectile() { }
        public Projectile(ProjectileStats stats, Character owner)
        {
            Stats = stats;
            Owner = owner;
        }
        public static Projectile Create(Projectile projectile, Vector2 position, Vector2 size)
        {
            Core.Scene.AddEntity(new()).AddComponent(projectile);
            projectile.Entity.Position = position;
            projectile.CollisionState.ShouldTestPlatforms = false;
            projectile.Collider = projectile.Entity.AddComponent(new BoxCollider(size.X, size.Y));
            projectile.Collider.PhysicsLayer = (int)LayerMask.Projectile;
            projectile.Collider.CollidesWithLayers = (int)LayerMask.Character;
            return projectile;
        }
        public override void Initialize()
        {
            base.Initialize();
            TargetTeams = Stats.TargetTeams;
        }
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
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
                _mapMover.Move(Time.DeltaTime * TimeScale * Velocity, Collider, CollisionState);
                if (CollisionState.HasCollision) OnWallCollision();
            }
            else
            {
                Entity.Position += Time.DeltaTime * TimeScale * Velocity;
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
        public void Die()
        {
            Entity.Destroy();
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
                if (
                    neighbor.Enabled &&
                    HitEntities.Contains(neighbor) is false &&
                    neighbor.Entity.TryGetComponent(out Character character) &&
                    Collider.Overlaps(neighbor) &&
                    character != Owner &&
                    Flags.IsFlagSet(Stats.TargetTeams, (int)character.Team)
                )
                {
                    character.HealthManager.Hit(new DamageInfo(Stats.Damage, Stats.KnockBack, this));
                    Pierces--;
                    HitEntities.Add(neighbor);
                }
            }
        }

        IEnumerator SwitchTeamMaterial()
        {
            yield return null;
            if (Entity != null && Entity.TryGetComponent(out SpriteRenderer renderer))
            {
                if (Flags.IsFlagSet(Stats.TargetTeams, (int)Teams.Player))
                {
                    if (renderer.Material == null)
                        renderer.Material = new Material(BlendState.NonPremultiplied);
                    Vector2 spriteSize = renderer.Sprite.SourceRect.Size.ToVector2();
                    OutlineEffect outlineEffect = new OutlineEffect(spriteSize);
                    outlineEffect.OutlineColor = Color.Red;
                    renderer.Material.Effect = outlineEffect;
                }
                else if (renderer.Material != null)
                    renderer.Material.Effect = null;
            }
        }
    }
}

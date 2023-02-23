using Nez;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez.Tiled;
using Roguelike.World;
using Nez.Sprites;
using System;

namespace Roguelike.Entities.Characters
{
    public class Character : Component, IUpdatable, ITimeScalable
    {
        public static readonly List<Character> Characters = new();
        public Vector2 Size { get; set; }
        public Vector2 Velocity;
        public CharacterStats Stats;
        public TiledMapMover.CollisionState CollisionState = new();
        public float TimeScale { get => Stats.TimeScale; set => Stats.TimeScale = value; }
        public float DeltaTime => Stats.TimeScale * Time.DeltaTime;
        public BoxCollider Collider { get; private set; }
        public HealthManager HealthManager { get; private set; }
        protected SpriteAnimator _spriteAnimator;
        TiledMapMover _mapMover => Level.Instance.TiledMapMover;
        public Teams Team;
        public int TargetTeams;
        public Character()
        {
            SetDefaults();
        }
        /// <summary>
        /// Set default entity values
        /// </summary>
        public virtual void SetDefaults() { }
        public static T Create<T>(T character, Vector2 position) where T : Character
        {
            Core.Scene.AddEntity(new()).AddComponent(character);
            character.Entity.Position = position;
            return character;
        }
        #region Lifecycle
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            Characters.Add(this);

            _spriteAnimator = Entity.AddComponent(new SpriteAnimator());
            HealthManager = Entity.AddComponent(new HealthManager(Stats.MaxHealth, Stats.Health, 1));
            Collider = Entity.AddComponent(new BoxCollider(Size.X, Size.Y));
            Collider.PhysicsLayer = (int)LayerMask.Character;

            HealthManager.onDamageTaken += OnDamageTaken;
            HealthManager.onDeath += Die;
        }
        public override void OnRemovedFromEntity()
        {
            base.OnRemovedFromEntity();
            Characters.Remove(this);

            HealthManager.onDamageTaken -= OnDamageTaken;
            HealthManager.onDeath -= Die;
        }
        public virtual void Update() => Move();
        #endregion
        public virtual void OnDamageTaken(DamageInfo damageInfo)
        {
            Color initial = _spriteAnimator.Color == Color.Red ? Color.White : _spriteAnimator.Color;
            _spriteAnimator.Color = Color.Red;
            Velocity += damageInfo.Knockback;
            Core.Schedule(0.1f, _ => _spriteAnimator.Color = initial);
        }
        public virtual void Die(DeathInfo deathInfo)
        {
            OnCharacterDeath(this);
            Entity.Destroy();
        }
        public virtual void Move() => _mapMover.Move(Time.DeltaTime * TimeScale * Velocity, Collider, CollisionState);
        #region Events
        public static event Action<Character> onCharacterDeath;
        public static void OnCharacterDeath(Character c) => onCharacterDeath?.Invoke(c);
        #endregion
    }
}

using Nez;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez.Tiled;
using Roguelike.World;
using Nez.Sprites;

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
        public BoxCollider Collider { get; private set; }
        protected HealthManager _healthManager;
        protected SpriteAnimator _spriteAnimator;
        TiledMapMover _mapMover => Level.Instance.TiledMapMover;
        public Character()
        {
            SetDefaults();
        }
        /// <summary>
        /// Set default entity values
        /// </summary>
        public virtual void SetDefaults() { }
        public static Character Create(Character character, Vector2 position)
        {
            Core.Scene.AddEntity(new()).AddComponent(character);
            character.Entity.Position = position;
            return character;
        }
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            Characters.Add(this);

            _spriteAnimator = Entity.AddComponent(new SpriteAnimator());
            _healthManager = Entity.AddComponent(new HealthManager(Stats.MaxHealth, Stats.Health, 1));
            Collider = Entity.AddComponent(new BoxCollider(Size.X, Size.Y));
            Collider.PhysicsLayer = (int)LayerMask.Character;

            _healthManager.onDamageTaken += OnDamageTaken;
            _healthManager.onDeath += Die;
        }
        public override void OnRemovedFromEntity()
        {
            base.OnRemovedFromEntity();
            Characters.Remove(this);

            _healthManager.onDamageTaken -= OnDamageTaken;
            _healthManager.onDeath -= Die;
        }
        public virtual void Update() => Move();
        public virtual void OnDamageTaken(DamageInfo damageInfo)
        {
            Color initial = _spriteAnimator.Color == Color.Red ? Color.White : _spriteAnimator.Color;
            _spriteAnimator.Color = Color.Red;
            Velocity += damageInfo.Knockback;
            Core.Schedule(0.1f, _ => _spriteAnimator.Color = initial);
        }
        public virtual void Die(DeathInfo deathInfo) => Entity.Destroy();
        public virtual void Move() => _mapMover.Move(Velocity, Collider, CollisionState);
    }
}

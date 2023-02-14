using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using Roguelike.World;

namespace Roguelike.Entities
{
    public class GameCharacter : Entity, ITimeScalable
    {
        public BoxCollider Collider { get; set; }
        public SpriteRenderer SpriteRenderer { get; set; }
        public float Health { get => _health; set { _health = value; if (value <= 0) OnDeath(); } }
        public float TimeScale { get; set; } = 1;
        public bool WallCollide { get; set; }
        public Vector2 Velocity;
        float _health;
        TiledMapMover _mapMover => Level.Instance.TiledMapMover;
        protected TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();

        public GameCharacter(Vector2 position) : base()
        {
            Position = position;
        }
        public override void Update()
        {
            base.Update();
            Move();
        }

        void Move()
        {
            if (WallCollide)
            {
                _mapMover.Move(Velocity, Collider, _collisionState);
            }
            else
            {
                Position += Velocity * Time.DeltaTime;
            }
        }
        public void SetColliderSize(Vector2 size)
        {
            if (Collider is null) Collider = new BoxCollider(size.X, size.Y);
            else Collider.SetSize(size.X, size.Y);
        }
        public virtual void Hit(float damage, Vector2 knockback, object source)
        {
            Health -= damage;
            Velocity += knockback;
        }

        /// <summary>
        /// Called when Entity's health reaches 0 or lower.
        /// </summary>
        public virtual void OnDeath() => Destroy();
    }
}

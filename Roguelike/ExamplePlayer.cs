using Microsoft.Xna.Framework.Input;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using Nez;
using System;
using Microsoft.Xna.Framework;
using Roguelike.World;
using Roguelike.Entities.Projectiles;
using Roguelike.Entities;

namespace Roguelike
{
    [Flags]
    public enum LayerMask
    {
        None = 1,
        Ground = 2,
        Character = 4,
        Projectile = 8,
    }
    public class ExamplePlayer : Component, IUpdatable
    {
        const string IDLE_ANIM = "IDLE";
        const string RUN_ANIM = "RUN";
        const string AIR_ANIM = "AIR";

        float speed = 8f;
        float acceleration = 40f;
        float gravity = 30f;
        float jumpForce = -14f;

        TiledMapMover _mapMover => Level.Instance.TiledMapMover;
        public BoxCollider Collider { get; private set; }
        TiledMapMover.CollisionState _collisionState = new TiledMapMover.CollisionState();
        public Vector2 Velocity;
        SpriteAnimator _animator;
        public override void Initialize()
        {
            base.Initialize();

            var texture = Entity.Scene.Content.LoadTexture(ContentPath.MM35_gb_Megaman);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);

            var healthManager = Entity.AddComponent(new HealthManager(50));
            healthManager.preDamageTaken += damageInfo => damageInfo.Damage = 0;
            healthManager.onDeath += _ => Entity.Destroy();
            healthManager.onDamageTaken += damageInfo => {
                _animator.Color = Color.Red;
                Velocity += damageInfo.Knockback;
                Core.Schedule(0.1f, _ => _animator.Color = Color.White);
            };
            Collider = Entity.AddComponent(new BoxCollider(32, 32));
            Collider.PhysicsLayer = (int)LayerMask.Character;
            Collider.CollidesWithLayers = (int)LayerMask.Ground;
            _animator = Entity.AddComponent(new SpriteAnimator(sprites[0]));
            _animator.SetLocalOffset(new Vector2(0, -2));

            _animator.AddAnimation(
                IDLE_ANIM, new[] { sprites[0], sprites[1], sprites[2] }
            );
            _animator.AddAnimation(
                RUN_ANIM, new[] { sprites[3], sprites[4], sprites[5] }
            );
            _animator.AddAnimation(AIR_ANIM, new[] { sprites[6] });
        }
        public void Update()
        {
            var xInput = 0f;
            if (Input.IsKeyDown(Keys.A)) xInput += -1;
            if (Input.IsKeyDown(Keys.D)) xInput += 1;

            Move(xInput);
            UpdateAnimation(xInput);
            Shoot();
        }
        void Move(float xInput)
        {
            if (!_collisionState.Below && !_collisionState.Above)
            {
                Velocity.Y += gravity * Time.AltDeltaTime;
                Velocity.Y = Math.Min(gravity, Velocity.Y);
            }
            else
                Velocity.Y = 0f;
            if (Input.IsKeyPressed(Keys.Space) && _collisionState.Below)
                Velocity.Y = jumpForce;

            if (_collisionState.Left || _collisionState.Right) Velocity.X = 0f;

            float frameAcceleration = acceleration * Time.AltDeltaTime * xInput;

            if (xInput == 0) frameAcceleration = -Velocity.X * 0.3f;
            else if (xInput > 0 && Velocity.X < 0 || xInput < 0 && Velocity.X > 0) frameAcceleration *= 3;

            if (Math.Abs(Velocity.X + frameAcceleration) < speed)
                Velocity.X += frameAcceleration;

            _collisionState.ShouldTestPlatforms = !Input.IsKeyDown(Keys.S);
            _mapMover.Move(Velocity, Collider, _collisionState);
        }
        void UpdateAnimation(float xInput)
        {
            if (xInput < 0f) _animator.FlipX = true;
            else if (xInput > 0f) _animator.FlipX = false;
            string animation;
            if (_collisionState.Below)
            {
                if ((int)xInput == 0) animation = IDLE_ANIM;
                else animation = RUN_ANIM;
            }
            else
                animation = AIR_ANIM;

            if (animation != null && !_animator.IsAnimationActive(animation))
                _animator.Play(animation);
        }
        float _cooldown = 0.01f;
        float _counter;
        void Shoot()
        {
            int shots = 1;
            float spread = 0f;
            _counter -= Time.DeltaTime;
            if (Input.LeftMouseButtonDown && _counter < 0)
            {
                _counter = _cooldown;
                for (int i = 1; i <= shots; i++)
                {
                    float percentage = i / (float)shots;
                    float spreadAngle = spread * percentage - spread * 0.5f;
                    var direction = (Entity.Scene.Camera.MouseToWorldPoint() - Entity.Position).Normalized();
                    var angle = direction.GetDirectionAngle() + spreadAngle;
                    direction = Vector2Ext.FromDirectionAngle(angle);
                    var projectileSprite = Entity.Scene.Content.LoadTexture(ContentPath.Exampleball);
                    var projectileEntity = new Projectile(
                        Entity.Position + direction * 35, new ProjectileStats(5f, direction * 6f, 15f, 2f, bounces: 5), projectileSprite.Bounds.Size.ToVector2() - new Vector2(2, 2)
                    );
                    projectileEntity.AddComponent(new SpriteRenderer(projectileSprite));
                    projectileEntity.AddComponent(new ProjectileRotator());
                    Entity.Scene.AddEntity(projectileEntity);
                }
            }
        }
    }
}

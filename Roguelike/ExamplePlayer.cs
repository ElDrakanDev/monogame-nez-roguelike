using Microsoft.Xna.Framework.Input;
using Nez.Sprites;
using Nez.Textures;
using Nez;
using System;
using Microsoft.Xna.Framework;
using Roguelike.Entities.Projectiles;

namespace Roguelike.Entities.Characters
{
    public class ExamplePlayer : Character
    {
        const string IDLE_ANIM = "IDLE";
        const string RUN_ANIM = "RUN";
        const string AIR_ANIM = "AIR";

        float speed = 8f;
        float acceleration = 40f;
        float gravity = 30f;
        float jumpForce = -14f;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Size = new(28, 28);
            Stats = new CharacterStats(50);
            Team = Teams.Player;
            TargetTeams = (int)Teams.Enemy;
        }
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var texture = Entity.Scene.Content.LoadTexture(ContentPath.MM35_gb_Megaman);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);
            //healthManager.onDeath += _ => Entity.Destroy();
            //healthManager.onDamageTaken += damageInfo => {
            //    _animator.Color = Color.Red;
            //    Velocity += damageInfo.Knockback;
            //    Core.Schedule(0.1f, _ => _animator.Color = Color.White);
            //};
            _spriteAnimator.SetLocalOffset(new Vector2(0, -2));

            _spriteAnimator.AddAnimation(
                IDLE_ANIM, new[] { sprites[0], sprites[1], sprites[2] }
            );
            _spriteAnimator.AddAnimation(
                RUN_ANIM, new[] { sprites[3], sprites[4], sprites[5] }
            );
            _spriteAnimator.AddAnimation(AIR_ANIM, new[] { sprites[6] });
        }
        public override void Update()
        {
            base.Update();

            var xInput = 0f;
            if (Input.IsKeyDown(Keys.A)) xInput += -1;
            if (Input.IsKeyDown(Keys.D)) xInput += 1;

            Movement(xInput);
            UpdateAnimation(xInput);
            Shoot();
        }
        void Movement(float xInput)
        {
            if (!CollisionState.Below && !CollisionState.Above)
            {
                Velocity.Y += gravity * Time.AltDeltaTime;
                Velocity.Y = Math.Min(gravity, Velocity.Y);
            }
            else
                Velocity.Y = 0f;
            if (Input.IsKeyPressed(Keys.Space) && CollisionState.Below)
                Velocity.Y = jumpForce;

            if (CollisionState.Left || CollisionState.Right)
                Velocity.X = 0f;

            float frameAcceleration = acceleration * Time.AltDeltaTime * xInput;

            if (xInput == 0)
                frameAcceleration = -Velocity.X * 0.3f;
            else if (xInput > 0 && Velocity.X < 0 || xInput < 0 && Velocity.X > 0)
                frameAcceleration *= 3;

            if (Math.Abs(Velocity.X + frameAcceleration) < speed)
                Velocity.X += frameAcceleration;

            CollisionState.ShouldTestPlatforms = !Input.IsKeyDown(Keys.S);
        }
        void UpdateAnimation(float xInput)
        {
            if (xInput < 0f) _spriteAnimator.FlipX = true;
            else if (xInput > 0f) _spriteAnimator.FlipX = false;
            string animation;
            if (CollisionState.Below)
            {
                if ((int)xInput == 0) animation = IDLE_ANIM;
                else animation = RUN_ANIM;
            }
            else
                animation = AIR_ANIM;

            if (animation != null && !_spriteAnimator.IsAnimationActive(animation))
                _spriteAnimator.Play(animation);
        }
        float _cooldown = 0.5f;
        float _counter;
        void Shoot()
        {
            int shots = 3;
            float spread = 45 * Mathf.Deg2Rad;
            _counter -= DeltaTime;
            if (Input.LeftMouseButtonDown && _counter < 0)
            {
                var aimDirection = (Entity.Scene.Camera.MouseToWorldPoint() - Entity.Position).Normalized();
                _counter = _cooldown;
                for (int i = 0; i < shots; i++)
                {
                    float percentage = i / (float)(shots - 1);
                    float spreadAngle = spread * percentage - spread * 0.5f;
                    var angle = aimDirection.GetDirectionAngle() + spreadAngle;
                    var direction = Vector2Ext.FromDirectionAngle(angle);
                    var projectileSprite = Entity.Scene.Content.LoadTexture(ContentPath.Exampleball);
                    var projectile = Projectile.Create(
                        new(new ProjectileStats(5f, direction * 6f, 15f, 2f, (int)Teams.Enemy, bounces: 3), projectileSprite.Bounds.Size.ToVector2() - new Vector2(2, 2)),
                        Entity.Position + direction * 35 
                    );
                    projectile.Entity.AddComponent(new SpriteRenderer(projectileSprite));
                    projectile.Entity.AddComponent(new ProjectileRotator());
                }
            }
        }
    }
}

using Nez.Textures;
using Nez;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez.Sprites;
using Roguelike.Entities.Projectiles;
using System;

namespace Roguelike.Entities.Characters.Players
{
    public class Player : Character
    {
        const string IDLE_ANIM = "IDLE";
        const string RUN_ANIM = "RUN";
        const string AIR_ANIM = "AIR";

        float speed = 8f * 60;
        float acceleration = 40f * 60;
        float gravity = 30f * 60;
        float jumpForce = -14f * 60;

        float _cooldown = 0.5f;
        float _counter;

        protected InputHandler _inputHandler;

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

            _inputHandler = Entity.AddComponent(new InputHandler());
           
            var texture = Entity.Scene.Content.LoadTexture(ContentPath.MM35_gb_Megaman);
            var sprites = Sprite.SpritesFromAtlas(texture, 32, 32);

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

            Movement();
            UpdateAnimation();
            Shoot();
        }
        void Movement()
        {
            float xInput = _inputHandler.MoveDirection.X;
            if (!CollisionState.Below && !CollisionState.Above)
            {
                Velocity.Y += gravity * DeltaTime;
                Velocity.Y = Math.Min(gravity, Velocity.Y);
            }
            else
                Velocity.Y = 0f;
            

            if (CollisionState.Left || CollisionState.Right)
                Velocity.X = 0f;

            float frameAcceleration = acceleration * DeltaTime * xInput;

            if (xInput == 0)
                frameAcceleration = -Velocity.X * 0.3f;
            else if (xInput > 0 && Velocity.X < 0 || xInput < 0 && Velocity.X > 0)
                frameAcceleration *= 3;

            if (Math.Abs(Velocity.X + frameAcceleration) < speed)
                Velocity.X += frameAcceleration;

            if (_inputHandler.JumpButton.IsPressed && CollisionState.Below)
            {
                if (CollisionState.IsGroundedOnOneWayPlatform && _inputHandler.MoveDirection.Y > 0d)
                    CollisionState.ShouldTestPlatforms = false;
                else
                    Velocity.Y = jumpForce;
            }
            else if(!_inputHandler.JumpButton.IsDown) CollisionState.ShouldTestPlatforms = true;
        }
        void UpdateAnimation()
        {
            float xInput = _inputHandler.MoveDirection.X;

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

        void Shoot()
        {
            int shots = 3;
            float spread = 45 * Mathf.Deg2Rad;
            _counter -= DeltaTime;
            if (_inputHandler.AttackButton.IsDown && _counter < 0)
            {
                var aimDirection = _inputHandler.AimDirection;
                _counter = _cooldown;
                for (int i = 0; i < shots; i++)
                {
                    float percentage = i / (float)(shots - 1);
                    float spreadAngle = spread * percentage - spread * 0.5f;
                    var angle = aimDirection.GetDirectionAngle() + spreadAngle;
                    var direction = Vector2Ext.FromDirectionAngle(angle);
                    var projectileSprite = Entity.Scene.Content.LoadTexture(ContentPath.Exampleball);
                    var projectile = Projectile.Create(
                        new(
                            new ProjectileStats(5f, direction * 6f * 60, 15f, 2f, (int)Teams.Enemy, bounces: 3),
                            projectileSprite.Bounds.Size.ToVector2() - new Vector2(2, 2),
                            this
                        ),
                        Entity.Position
                    );
                    projectile.Entity.AddComponent(new SpriteRenderer(projectileSprite));
                    projectile.Entity.AddComponent(new ProjectileRotator());
                }
            }
        }
    }
}

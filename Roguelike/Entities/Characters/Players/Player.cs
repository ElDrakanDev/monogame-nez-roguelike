using Nez.Textures;
using Nez;
using System;
using System.Collections.Generic;
using Roguelike.Weapons;
using Roguelike.Helpers;

namespace Roguelike.Entities.Characters.Players
{
    public class Player : Character
    {
        public static List<Player> Players { get; private set; } = new();

        const string IDLE_ANIM = "IDLE";
        const string RUN_ANIM = "RUN";
        const string AIR_ANIM = "AIR";

        float speed = 6f * 60;
        float acceleration = 32f * 60;
        float gravity = 24f * 60;
        float jumpForce = -12f * 60;

        protected InputHandler _inputHandler;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Size = new(28, 28);
            Stats = new CharacterStats(50);
            Team = Teams.Player;
            TargetTeams = (int)Teams.Enemy;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            Players.Add(this);
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            Players.Remove(this);
        }
        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            _inputHandler = Entity.AddComponent(new InputHandler());
            Entity.AddComponent(new ExampleWeapon());
           
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
            Interact();
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

            if (_inputHandler.JumpButton.IsPressed)
            {
                if(CollisionState.Below && (_inputHandler.MoveDirection.Y <= 0 || !CollisionState.IsGroundedOnOneWayPlatform))
                    Velocity.Y = jumpForce;
                else if (_inputHandler.MoveDirection.Y > 0)
                    CollisionState.ShouldTestPlatforms = false;
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

        void Interact()
        {
            var neighbors = Physics.BoxcastBroadphaseExcludingSelf(Collider, (int)LayerMask.Interactable);
            List<Entity> interactables = new List<Entity>();
            foreach (var neighbor in neighbors)
            {
                if (Collider.Overlaps(neighbor))
                {
                    interactables.Add(neighbor.Entity);
                }
            }
            var closest = interactables.Closest(Entity.Position);
            if(closest != null)
            {
                if (_inputHandler.InteractButton.IsPressed is false)
                    Interactable.OnHover(Entity, closest);
                else
                    Interactable.OnInteract(Entity, closest);
            }
        }

        [Nez.Console.Command("godmode", "Toggles player invulnerability")]
        public static void Godmode()
        {
            foreach(var player in Players)
            {
                _godmode = !_godmode;
                if(_godmode is true)
                {
                    player.HealthManager.preDamageTaken += IgnoreHit;
                }
                else
                {
                    player.HealthManager.preDamageTaken -= IgnoreHit;
                }
            }
        }
        static bool _godmode = false;
        static void IgnoreHit(DamageInfo info) => info.Canceled = true;
    }
}

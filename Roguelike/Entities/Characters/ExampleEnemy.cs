﻿using Nez.Textures;
using Nez;
using Microsoft.Xna.Framework;
using Roguelike.Entities.Projectiles;
using Nez.Sprites;

namespace Roguelike.Entities.Characters
{
    public class ExampleEnemy : Character
    {
        const float MAX_COOLDOWN = 1.5f;
        float _cooldown = MAX_COOLDOWN;
        Color _shootColor = Color.Red;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Size = new(96, 96);
            Stats = new CharacterStats(50);
            Team = Teams.Enemy;
            TargetTeams = (int)Teams.Player;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var texture = Entity.Scene.Content.LoadTexture(ContentPath.MM35_gb_Megaman);
            _spriteAnimator.Sprite = new Sprite(texture);
            if (Random.Chance(0.3f))
            {
                _spriteAnimator.Color = Color.Green;
                Team = Teams.Player;
                TargetTeams = (int)Teams.Enemy;
                _shootColor = Color.White;
            }
        }
        public override void Update()
        {
            base.Update();

            _cooldown -= DeltaTime;
            if(_cooldown < 0)
            {
                Character closest = null;
                float closestDistance = float.MaxValue;

                foreach(var character in Character.Characters)
                {
                    if (character == this || Flags.IsFlagSet(TargetTeams, (int)character.Team) is false) continue;
                    float distance = Vector2.Distance(Entity.Position, character.Entity.Position);
                    if(distance < closestDistance)
                    {
                        closest = character;
                        closestDistance = distance;
                    }
                }

                if(closest != null)
                {
                    _cooldown = MAX_COOLDOWN;
                    var direction = (closest.Entity.Position - Entity.Position).Normalized();
                    var projectileSprite = Entity.Scene.Content.LoadTexture(ContentPath.Exampleball);
                    var projectile = Projectile.Create(
                        new(
                            new ProjectileStats(5, direction * 5 * 60, 5, 5, TargetTeams),
                            this
                        ),
                        Entity.Position,
                        projectileSprite.Bounds.Size.ToVector2()
                    );
                    projectile.Entity.AddComponent(new SpriteRenderer(projectileSprite) { Color = _shootColor});
                    projectile.Entity.AddComponent(new ProjectileRotator());
                }
            }
        }
    }
}

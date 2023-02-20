using Nez.Textures;
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
            _spriteAnimator.Color = Color.Yellow;
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
                    if (character == this) continue;
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
                        new(new ProjectileStats(5, direction * 5, 5, 5, (int)Teams.Player), projectileSprite.Bounds.Size.ToVector2()),
                        Entity.Position
                    );
                    projectile.Entity.AddComponent(new SpriteRenderer(projectileSprite));
                    projectile.Entity.AddComponent(new ProjectileRotator());
                }
            }
        }
    }
}

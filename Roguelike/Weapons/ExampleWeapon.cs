using Nez.Sprites;
using Nez;
using Roguelike.Entities.Projectiles;

namespace Roguelike.Weapons
{
    public class ExampleChargedWeapon : Weapon
    {
        public override void SetDefaults()
        {
            _baseStats.Damage = 3.5f;
            _baseStats.AttacksPerSecond = 0.8f;
            _baseStats.Spread = 60 * Mathf.Deg2Rad;
            _baseStats.Shots = 30;
            _baseStats.Bounces = 1;
            _baseStats.ProjectileSpeed = 600f;
            _baseStats.KnockBack = 5;
            _baseStats.UseMode = WeaponUseMode.Charged;
            _baseStats.SpreadMode = SpreadMode.Random;
            AutoAttack = false;
        }
        public override void Attack()
        {
            foreach (var projectile in Shoot())
            {
                var projectileSprite = Entity.Scene.Content.LoadTexture(ContentPath.Exampleball);
                projectile.Entity.AddComponent(new SpriteRenderer(projectileSprite));
                projectile.Entity.AddComponent(new ProjectileRotator());
                projectile.Size = projectileSprite.Bounds.Size.ToVector2();
            }
        }
    }
    public class ExampleWeapon : Weapon
    {
        public override void SetDefaults()
        {
            _baseStats.Damage = 0.1f;
            _baseStats.AttacksPerSecond = 1000f;
            _baseStats.Spread = 50 * Mathf.Deg2Rad;
            _baseStats.Shots = 3;
            _baseStats.Bounces = 1;
            _baseStats.ProjectileSpeed = 1000f;
            _baseStats.KnockBack = 5;
            _baseStats.ProjectileLifetime = 0.5f;
        }

        public override void Attack()
        {
            foreach(var projectile in Shoot())
            {
                var projectileSprite = Entity.Scene.Content.LoadTexture(ContentPath.Exampleball);
                projectile.Entity.AddComponent(new SpriteRenderer(projectileSprite));
                projectile.Entity.AddComponent(new ProjectileRotator());
                projectile.Size = projectileSprite.Bounds.Size.ToVector2();
            }
        }
    }
}

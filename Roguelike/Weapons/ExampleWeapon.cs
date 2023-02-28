using Nez.Sprites;
using Nez;
using Roguelike.Entities.Projectiles;
using Microsoft.Xna.Framework;

namespace Roguelike.Weapons
{
    public class ExampleWeapon : Weapon
    {
        // ========= NORMAL ===========
        public override void SetDefaults()
        {
            _baseStats.Damage = 1;
            _baseStats.AttacksPerSecond = 100f;
            _baseStats.Spread = 45 * Mathf.Deg2Rad;
            _baseStats.Shots = 1;
            _baseStats.Bounces = 1;
            _baseStats.ProjectileSpeed = 1000f;
            _baseStats.KnockBack = 5;
            _baseStats.ProjectileLifetime = 0.5f;
        }

        // ========= CHARGED ==========
        //public override void SetDefaults()
        //{
        //    _baseStats.Damage = 1;
        //    _baseStats.AttacksPerSecond = 0.8f;
        //    _baseStats.Spread = 60 * Mathf.Deg2Rad;
        //    _baseStats.Shots = 15;
        //    _baseStats.Bounces = 1;
        //    _baseStats.ProjectileSpeed = 600f;
        //    _baseStats.KnockBack = 5;
        //    _baseStats.UseMode = WeaponUseMode.Charged;
        //    _baseStats.SpreadMode = SpreadMode.Random;
        //    AutoAttack = false;
        //}

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

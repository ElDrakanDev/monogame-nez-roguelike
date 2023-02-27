using Nez.Sprites;
using Nez;
using Roguelike.Entities.Projectiles;
using Microsoft.Xna.Framework;

namespace Roguelike.Weapons
{
    public class ExampleWeapon : Weapon
    {
        // ========= NORMAL ===========
        //public override void SetDefaults()
        //{
        //    _baseStats.Damage = 1;
        //    _baseStats.AttacksPerSecond = 2.73f;
        //    _baseStats.Inaccuracy = 15 * Mathf.Deg2Rad;
        //    _baseStats.Spread = 45 * Mathf.Deg2Rad;
        //    _baseStats.Shots = 1;
        //    _baseStats.Bounces = 1;
        //    _baseStats.ProjectileSpeed = 300f;
        //    _baseStats.KnockBack = 5;
        //}

        // ========= CHARGED ==========
        public override void SetDefaults()
        {
            _baseStats.Damage = 1;
            _baseStats.AttacksPerSecond = 0.5f;
            _baseStats.Inaccuracy = 15 * Mathf.Deg2Rad;
            _baseStats.Spread = 45 * Mathf.Deg2Rad;
            _baseStats.Shots = 5;
            _baseStats.Bounces = 2;
            _baseStats.ProjectileSpeed = 500f;
            _baseStats.KnockBack = 5;
            AutoAttack = true;
            _baseStats.UseMode = WeaponUseMode.Charged;
        }

        public override void Attack()
        {
            int shots = _baseStats.Shots;
            float spread = _baseStats.Spread;
            var baseAngle = _inputHandler.AimDirection.GetDirectionAngle() + Random.Range(-_baseStats.Inaccuracy, _baseStats.Inaccuracy);

            for (int i = 0; i < shots; i++)
            {
                float percentage = shots == 1 ? 0.5f : i / (float)(shots - 1);
                float spreadAngle = spread * percentage - spread * 0.5f;
                var angle = baseAngle + spreadAngle;
                var direction = Vector2Ext.FromDirectionAngle(angle);
                var projectileSprite = Entity.Scene.Content.LoadTexture(ContentPath.Exampleball);
                var projectile = Projectile.Create(
                    new(
                        new ProjectileStats(
                            _baseStats.Damage,
                            direction * _baseStats.ProjectileSpeed,
                            _baseStats.ProjectileLifetime,
                            _baseStats.KnockBack,
                            Owner.TargetTeams, 
                            _baseStats.GroundCollide,
                            _baseStats.Bounces,
                            _baseStats.Pierces
                        ),
                        projectileSprite.Bounds.Size.ToVector2(),
                        Owner
                    ),
                    Entity.Position
                );
                projectile.Entity.AddComponent(new SpriteRenderer(projectileSprite));
                projectile.Entity.AddComponent(new ProjectileRotator());
            }
        }
    }
}

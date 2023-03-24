using Microsoft.Xna.Framework;
using Nez;
using Nez.Console;
using Nez.Tiled;
using Roguelike.Entities.Characters.Players;
using Roguelike.Entities.Projectiles;
using System.Collections.Generic;

namespace Roguelike.Weapons
{
    public abstract class Weapon : Component, IUpdatable
    {
        float _cooldown;
        public Player Owner;
        protected InputHandler _inputHandler;
        protected WeaponStats _baseStats = new();
        public bool AutoAttack = true;
        public Weapon() { }
        public abstract void SetDefaults();

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            SetDefaults();
            Owner = Entity.GetComponent<Player>();
            _inputHandler = Entity.GetComponent<InputHandler>();
        }
        public override void OnRemovedFromEntity()
        {
            base.OnRemovedFromEntity();
            Owner = null;
            _inputHandler = null;
        }
        public virtual void Update()
        {
            if(Owner != null)
            {
                if (_baseStats.UseMode == WeaponUseMode.Normal)
                    InputNormalAttack();
                else if(_baseStats.UseMode == WeaponUseMode.Charged)
                    InputChargedAttack();
            }
        }
        void InputNormalAttack()
        {
            _cooldown -= Time.DeltaTime;

            bool isAttackPressed = AutoAttack ? _inputHandler.AttackButton.IsDown : _inputHandler.AttackButton.IsPressed;

            if (isAttackPressed && _cooldown < 0)
            {
                _cooldown = _baseStats.UseTime;
                Attack();
            }
        }
        void InputChargedAttack()
        {
            bool isAttackDown = _inputHandler.AttackButton.IsDown;

            if (isAttackDown)
                _cooldown -= Time.DeltaTime;

            if(_cooldown < 0)
            {
                if (AutoAttack)
                {
                    _cooldown = _baseStats.UseTime;
                    Attack();
                }
                else if(_inputHandler.AttackButton.IsReleased)
                {
                    _cooldown = _baseStats.UseTime;
                    Attack();
                }
            }

            if (isAttackDown is false && AutoAttack is false)
                _cooldown = _baseStats.UseTime;
        }
        public abstract void Attack();
        protected IEnumerable<Projectile> Shoot()
        {
            int shots = _baseStats.Shots;
            float spread = _baseStats.Spread;
            var baseAngle = _inputHandler.AimDirection.GetDirectionAngle();
            Vector2 direction;

            if(shots > 1)
            {
                for (int i = 0; i < shots; i++)
                {
                    if(_baseStats.SpreadMode == SpreadMode.Even)
                    {
                        float percentage = shots == 1 ? 0.5f : i / (float)(shots - 1);
                        float spreadAngle = spread * percentage - spread * 0.5f;
                        var angle = baseAngle + spreadAngle;
                        direction = Vector2Ext.FromDirectionAngle(angle);
                    }
                    else{
                        float halfSpread = spread * 0.5f;
                        var angle = baseAngle + Random.Range(-halfSpread, halfSpread);
                        direction = Vector2Ext.FromDirectionAngle(angle);
                    }
                    var projectile = Projectile.Create(
                        new Projectile(
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
                            Owner
                        ),
                        Entity.Position,
                        Vector2.One
                    );
                    yield return projectile;
                }
            }
            else
            {
                float halfSpread = spread * 0.5f;
                var angle = baseAngle + Random.Range(-halfSpread, halfSpread);
                direction = Vector2Ext.FromDirectionAngle(angle);
                var projectile = Projectile.Create(
                    new Projectile(
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
                        Owner
                    ),
                    Entity.Position,
                    Vector2.One
                );
                yield return projectile;
            }
        }

        public static Weapon FromTmxObject(TmxObject obj, TmxMap map)
        {
            try
            {
                System.Type weaponType;
                if (obj.Template == string.Empty)
                    weaponType = System.Type.GetType(obj.Type);
                else
                {
                    TmxTemplate template = Core.Scene.Content.LoadTmxTemplate(obj.Template, map);
                    weaponType = System.Type.GetType(template.Type);
                }
                Weapon weapon = System.Activator.CreateInstance(weaponType) as Weapon;
                return weapon;

                // TODO: Load stats
            }
            catch (System.ArgumentException ex)
            {
                string msg = $"Error creating instance of Weapon subclass with full name {obj.Type}. {ex.Message}";
                Debug.Error(msg);
                DebugConsole.Instance.Log(msg);
            }
            return null;
        }
    }
}

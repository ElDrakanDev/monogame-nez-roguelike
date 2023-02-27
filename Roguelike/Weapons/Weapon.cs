using Microsoft.Xna.Framework;
using Nez;
using Roguelike.Entities.Characters.Players;


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
    }
}

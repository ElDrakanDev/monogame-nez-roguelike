namespace Roguelike.Weapons
{
    public enum WeaponUseMode
    {
        Normal,
        Charged,
    }
    public enum SpreadMode
    {
        Even,
        Random,
    }
    public struct WeaponStats
    {
        public float Damage;
        public float AttacksPerSecond;
        public float ProjectileLifetime = 5f;
        public int Bounces = 0;
        public int Pierces = 0;
        public int Shots = 1;
        public float ProjectileSpeed = 1;
        public float KnockBack = 1;
        public float Spread = 0;
        public bool GroundCollide = true;
        public WeaponUseMode UseMode = WeaponUseMode.Normal;
        public SpreadMode SpreadMode = SpreadMode.Random;

        public float UseTime => 1 / AttacksPerSecond;

        public WeaponStats()
        {
            Damage = 1;
            AttacksPerSecond = 1;
        }
    }
}

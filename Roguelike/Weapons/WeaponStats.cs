namespace Roguelike.Weapons
{
    public enum WeaponUseMode
    {
        Normal,
        Charged,
    }

    public struct WeaponStats
    {
        public float Damage;
        public float AttacksPerSecond;
        public float ProjectileLifetime = 5f;
        public float Inaccuracy = 0;
        public int Bounces = 0;
        public int Pierces = 0;
        public int Shots = 1;
        public float ProjectileSpeed = 1;
        public float KnockBack = 1;
        public float Spread = 0;
        public bool GroundCollide = true;
        public WeaponUseMode UseMode = WeaponUseMode.Normal;

        public float UseTime => 1 / AttacksPerSecond;

        public WeaponStats()
        {
            Damage = 1;
            AttacksPerSecond = 1;
        }
    }
}

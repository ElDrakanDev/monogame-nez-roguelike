using System;

namespace Roguelike
{
    [Flags]
    public enum LayerMask
    {
        None = 1,
        Ground = 2,
        Character = 4,
        Projectile = 8,
        Interactable = 16,
    }
}

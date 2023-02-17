using System;

namespace Roguelike
{
    [Flags]
    public enum Teams
    {
        None = 0,      // 00000000000000000000000000000000
        Player = 1,    // 00000000000000000000000000000001
        Enemy = 2,     // 00000000000000000000000000000010
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike
{
    [Flags]
    public enum LayerMask
    {
        None = 1,
        Ground = 2,
        Character = 4,
        Projectile = 8,
    }
}

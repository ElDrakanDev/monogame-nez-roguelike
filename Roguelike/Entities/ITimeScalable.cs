using Roguelike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike.Entities
{
    public interface ITimeScalable
    {
        public float TimeScale { get; set; }
    }
}
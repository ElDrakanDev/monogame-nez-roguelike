using System;

namespace Roguelike
{
    /// <summary>
    /// RNG class for individual RNG entities
    /// </summary>
    public class RNG
    {
        Random _rand;
        ulong _state = 0;
        public ulong State { get => _state; }
        public static RNG ItemRng { get; private set; }
        public static RNG RoomRng { get; private set; }
        public RNG(int seed, ulong state = 1)
        {
            _rand = new Random(seed);
            for (ulong i = 0; i < state - 1; i++)
            {
                _state++;
                _rand.Next(0, 2);
            }
        }

        public static void Initialize(int seed)
        {
            ItemRng = new RNG(seed);
            RoomRng = new RNG(seed);
        }

        public int Range(int min, int exMax)
        {
            _state++;
            return _rand.Next(min, exMax);
        }

        public float FRange(float min, float max)
        {
            _state++;
            return ((float)_rand.NextDouble() + min) * max;
        }
    }
}

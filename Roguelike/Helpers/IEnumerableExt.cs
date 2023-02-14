using Nez;
using System.Collections.Generic;


namespace Roguelike
{
    public static class IEnumerableExt
    {
        public static void Shuffle<T>(this IList<T> listlike)
        {
            for (int i = 0; i < listlike.Count; i++)
            {
                var t1 = listlike[i];
                var t2Index = Random.Range(0, listlike.Count);
                listlike[i] = listlike[t2Index];
                listlike[t2Index] = t1;
            }
        }
        public static void Shuffle<T>(this IList<T> listlike, RNG rng)
        {
            for (int i = 0; i < listlike.Count; i++)
            {
                var t1 = listlike[i];
                var t2Index = rng.Range(0, listlike.Count);
                listlike[i] = listlike[t2Index];
                listlike[t2Index] = t1;
            }
        }
    }
}

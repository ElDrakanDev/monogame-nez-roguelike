using System.Collections.Generic;
using Nez;
using Microsoft.Xna.Framework;

namespace Roguelike.Helpers
{
    public static class IListEntityExt
    {
        public static Entity Closest(this IList<Entity> list)
        {
            Entity closest = list.Count > 0 ? list[0] : null;
            float closestDistance = float.MaxValue;

            foreach(var entity in list)
            {
                float distance = Vector2.Distance(closest.Position, entity.Position);
                if(distance < closestDistance)
                {
                    closest = entity;
                    closestDistance = distance;
                }
            }

            return closest;
        }
    }
}

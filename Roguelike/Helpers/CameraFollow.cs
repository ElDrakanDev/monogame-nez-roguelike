using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;

namespace Roguelike.Helpers
{
    public class CameraFollow : Component, IUpdatable
    {
        List<Transform> _targets = new List<Transform>();

        public void Update()
        {
            if (_targets.Count == 0) return;
            Vector2 finalPosition = Vector2.Zero;
            foreach (Transform t in _targets)
            {
                finalPosition += t.Position;
            }
            finalPosition /= _targets.Count;
            Entity.Position = finalPosition;
        }

        public void AddTarget(Transform t) => _targets.Add(t);
        public void RemoveTarget(Transform t) => _targets.Remove(t);
    }
}

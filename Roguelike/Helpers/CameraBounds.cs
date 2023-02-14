using System;
using Microsoft.Xna.Framework;


namespace Nez
{
    public class CameraBounds : Component, IUpdatable
    {
        public Vector2 Min, Max;
        bool _checkX, _checkY = true;


        public CameraBounds()
        {
            // make sure we run last so the camera is already moved before we evaluate its position
            SetUpdateOrder(int.MaxValue);
        }


        public CameraBounds(Vector2 min, Vector2 max) : this()
        {
            Min = min;
            Max = max;
        }


        public override void OnAddedToEntity()
        {
            Entity.UpdateOrder = int.MaxValue;
        }


        void IUpdatable.Update()
        {
            var cameraBounds = Entity.Scene.Camera.Bounds;

            if (_checkX)
            {
                if (cameraBounds.Left < Min.X)
                    Entity.Scene.Camera.Position += new Vector2(Min.X - cameraBounds.Left, 0);
                if (cameraBounds.Right > Max.X)
                    Entity.Scene.Camera.Position += new Vector2(Max.X - cameraBounds.Right, 0);
            }
            if (_checkY)
            {
                if (cameraBounds.Top < Min.Y)
                    Entity.Scene.Camera.Position += new Vector2(0, Min.Y - cameraBounds.Top);
                if (cameraBounds.Bottom > Max.Y)
                    Entity.Scene.Camera.Position += new Vector2(0, Max.Y - cameraBounds.Bottom);
            }
        }

        public void SetBounds(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
            _checkX = Math.Abs(max.X - min.X) > Entity.Scene.Camera.Bounds.Width;
            _checkY = Math.Abs(max.Y - min.Y) > Entity.Scene.Camera.Bounds.Height;
        }
    }
}
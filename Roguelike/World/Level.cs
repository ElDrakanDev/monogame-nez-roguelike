using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using System.Collections.Generic;

namespace Roguelike.World
{
    public class Level : Component
    {
        public static Level Instance { get; private set; }

        public Point ActivePoint { get; private set; }
        public Room ActiveRoom => Rooms[ActivePoint];
        public TmxMap Tilemap => ActiveRoom.Tilemap;
        public TiledMapRenderer TilemapRenderer => ActiveRoom.TilemapRenderer;
        public TiledMapMover TiledMapMover => ActiveRoom.TilemapMover;
        public Dictionary<Point, Room> Rooms { get; private set; }

        public Level(Dictionary<Point, Room> rooms)
        {
            Rooms = rooms;
        }

        public override void Initialize()
        {
            base.Initialize();
            Instance = this;
            foreach (var (point, room) in Rooms)
            {
                Entity.Scene.CreateEntity(point.ToString()).AddComponent(room);
                room.Transform.SetParent(Transform);
            }
        }
        public bool CanMove(Point direction) => Rooms.ContainsKey(ActivePoint + direction);
        public bool Move(Point direction)
        {
            var newPoint = ActivePoint + direction;
            if(Rooms.ContainsKey(newPoint))
            {
                _changeRoom(newPoint);
                return true;
            }
            return false;
        }

        void _changeRoom(Point point)
        {
            ActiveRoom?.Exit();
            ActivePoint = point;
            ActiveRoom.Enter();
        }

        public void EnterStartRoom()
        {
            Rooms[Point.Zero].Enter();
        }
    }
}

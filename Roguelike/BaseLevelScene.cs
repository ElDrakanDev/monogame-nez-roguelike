using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.UI;
using Roguelike.Helpers;
using Roguelike.World;
using Roguelike.Entities;

namespace Roguelike
{
    public class BaseLevelScene : Scene
    {
        Level _level;
        ExamplePlayer _player;
        public override void Initialize()
        {
            base.Initialize();

            SetDesignResolution(600, 400, SceneResolutionPolicy.BestFit);
            Screen.SetSize(800, 600);
            RNG.Initialize(1);
            Camera.AddComponent(new CameraBounds());
            var cameraFollow = Camera.AddComponent(new CameraFollow());
            _level = LevelGenerator.GenerateLevel(ContentPath.Tiled.Directory, new() { { RoomType.Fight, 2}, {RoomType.Boss, 1 }, {RoomType.Shop, 1 } });
            CreateEntity("level").AddComponent(_level);
            _player = CreateEntity("player1").AddComponent(new ExamplePlayer());
            _player.Entity.Position = new Vector2(200, 200);
            cameraFollow.AddTarget(_player.Transform);
        }
        public override void Begin()
        {
            base.Begin();
            _level.EnterStartRoom();
        }
        public override void Update()
        {
            if (Input.IsKeyPressed(Keys.Down))
            {
                if(_level.CanMove(PointExt.Down))
                    SwitchRoom(PointExt.Down);
            }
            else if (Input.IsKeyPressed(Keys.Up))
            {
                if(_level.CanMove(PointExt.Up))
                    SwitchRoom(PointExt.Up);
            }
            else if (Input.IsKeyPressed(Keys.Left))
            {
                if(_level.CanMove(PointExt.Left))
                    SwitchRoom(PointExt.Left);
            }
            else if (Input.IsKeyPressed(Keys.Right))
            {
                if (_level.CanMove(PointExt.Right))
                    SwitchRoom(PointExt.Right);
            }
            base.Update();
        }

        void SwitchRoom(Point direction)
        {
            var transition = new SquaresTransition();
            Time.AltTimeScale = 0;
            float transitionDuration = 0.2f;
            transition.SquaresInDuration = transitionDuration;
            transition.SquaresOutDuration = transitionDuration;
            Core.Schedule(transitionDuration, timer => {
                _level.Move(direction);
                _player.Entity.Position = _level.ActiveRoom.EntranceDoor.Position;
                Core.Schedule(transitionDuration, timer => {Time.AltTimeScale = 1;});
            });
            Core.StartSceneTransition(transition);
        }
    }
}

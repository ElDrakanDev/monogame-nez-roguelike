using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Roguelike.Helpers;
using Roguelike.World;
using Roguelike.Entities.Characters;
using Roguelike.Entities.Characters.Players;

namespace Roguelike
{
    public class BaseLevelScene : Scene
    {
        Level _level;
        Player _player;
        public override void Initialize()
        {
            base.Initialize();

            SetDesignResolution(800, 600, SceneResolutionPolicy.BestFit);
            Screen.SetSize(800, 600);
            RNG.Initialize(1);
            Camera.AddComponent(new CameraBounds());
            _level = LevelGenerator.GenerateLevel(ContentPath.Tiled.Directory, new() { { RoomType.Fight, 2}, {RoomType.Boss, 1 }, {RoomType.Shop, 1 } });
            CreateEntity("level").AddComponent(_level);
    
        }
        public override void Begin()
        {
            base.Begin();
            _player = Character.Create(new Player(), new Vector2(200, 200));
            var cameraFollow = Camera.AddComponent(new CameraFollow());
            cameraFollow.AddTarget(_player.Transform);
            SwitchRoom(Point.Zero);
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Roguelike.Helpers;
using Roguelike.World;
using Roguelike.Entities.Characters;
using Roguelike.Entities.Characters.Players;
using Roguelike.Entities;
using Nez.Sprites;
using Roguelike.Weapons;

namespace Roguelike
{
    public class BaseLevelScene : Scene
    {
        Level _level;
        Player _player;
        public override void Initialize()
        {
            base.Initialize();
            Camera.MaximumZoom = 2;
            Camera.MinimumZoom = 0.75f;
            Camera.Zoom = 0;
            SetDesignResolution(800, 600, SceneResolutionPolicy.None);
            Screen.SetSize(800, 600);
            RNG.Initialize(1);

            Camera.AddComponent(new CameraBounds());
            _level = LevelGenerator.GenerateLevel(ContentPath.Tiled.Directory, new() { { RoomType.Fight, 2}, {RoomType.Boss, 1 }, {RoomType.Shop, 1 } });
            CreateEntity("level").AddComponent(_level);
            CreateEntity("fps-counter").AddComponent(new FramesPerSecondCounter());

            var interactable = CreateEntity("PickableChargedWeapon")
                .AddComponent(new InteractableOutline())
                .AddComponent(new WeaponInteractable(new ExampleChargedWeapon()))
                .AddComponent(new SpriteRenderer(Content.LoadTexture(ContentPath.ExampleSword)) { Color = Color.Blue })
                .AddComponent(new BoxCollider());
            interactable.Entity.Position = new Vector2(200, 430);
            var interactable2 = interactable.Entity.Clone();
            interactable2.Position = new Vector2(300, 430);
            AddEntity(interactable2);
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

            if (Input.IsKeyPressed(Keys.Enter))
                Time.TimeScale = Time.TimeScale > 0 ? 0 : 1;

            Camera.Zoom = Camera.Zoom += 0.0005f * Input.MouseWheelDelta;

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

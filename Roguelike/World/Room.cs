using System;
using System.Collections.Generic;
using Nez.Tiled;
using Nez;
using Microsoft.Xna.Framework;
using Roguelike.Entities.Projectiles;
using Roguelike.Entities.Characters;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelike.World
{
    public enum RoomType
    {
        Start, Fight, Trap, Treasure, Shop, Curse, Boss
    }
    public class Room : Component
    {
        string _tiledMapPath;
        public TmxMap Tilemap { get; private set; }
        public TiledMapRenderer TilemapRenderer { get; private set; }
        public TiledMapMover TilemapMover { get; private set; }
        Vector2 _topLeft, _bottomRight;
        public RoomType Type { get; private set; }
        public Entity EntranceDoor { get; private set; }
        public Entity ExitDoor { get; private set; }
        Dictionary<TmxObject, Entity> _charactersByTmxObject = new();
        public Room(string tiledMapPath, RoomType type)
        {
            _tiledMapPath = tiledMapPath;
        }
        public override void Initialize()
        {
            base.Initialize();
            Tilemap = Entity.Scene.Content.LoadTiledMap(_tiledMapPath);
            TilemapRenderer = Entity.AddComponent(new TiledMapRenderer(Tilemap, "ground", false));
            TilemapMover = Entity.AddComponent(new TiledMapMover(TilemapRenderer.CollisionLayer));
            TilemapRenderer.LayerDepth = int.MaxValue;

            _topLeft = new Vector2(Tilemap.TileWidth, Tilemap.TileWidth);
            _bottomRight = new Vector2(
                Tilemap.TileWidth * (Tilemap.Width - 1),
                Tilemap.TileWidth * (Tilemap.Height - 1)
            );

            // Desactivar componentes agregados
            TilemapRenderer.Enabled = false;
            Enabled = false;
        }
        #region Events
        public override void OnEnabled()
        {
            base.OnEnabled();
            Character.onCharacterDeath += RemoveDeadCharacters;
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            Character.onCharacterDeath -= RemoveDeadCharacters;
        }
        void RemoveDeadCharacters(Character character)
        {
            TmxObject found = null;
            foreach(var pair in _charactersByTmxObject)
            {
                if (pair.Value == character.Entity)
                {
                    found = pair.Key;
                    break;
                }
            }
            if (found is not null && _charactersByTmxObject.Remove(found))
            {
                var charactersGroup = Tilemap.GetObjectGroup("characters");
                charactersGroup.Objects.Remove(found);
            }
        }
        #endregion
        public void Enter()
        {
            Entity.Scene.Camera.GetComponent<CameraBounds>().SetBounds(_topLeft, _bottomRight);
            TilemapRenderer.Enabled = true;
            Enabled = true;
            CreateDoorsIfNull();
            ResetChildCharacters();
        }
        public void Exit()
        {
            foreach (var projectile in Projectile.Projectiles)
                projectile.Entity.Destroy();
            foreach (var entity in _charactersByTmxObject.Values)
                entity.Destroy();
            _charactersByTmxObject.Clear();
            TilemapRenderer.RemoveColliders();
            TilemapRenderer.Enabled = false;
            Enabled = false;
        }
        TmxLayerTile _getSolidTileBelow(Vector2 position)
        {
            var startPoint = Tilemap.WorldToTilePosition(position);
            int searchLimit = 10;
            for (int i = 0; i < searchLimit; i++)
            {
                var checkPosition = new Point(startPoint.X, startPoint.Y + i);
                var tile = TilemapRenderer.CollisionLayer.GetTile(checkPosition.X, checkPosition.Y);
                if (tile != null) return tile;
            }
            return null;
        }
        #region Child Entities Management
        void CreateDoorsIfNull()
        {
            if (EntranceDoor is null)
            {
                var doorsGroup = Tilemap.GetObjectGroup("doors");
                var entranceObj = doorsGroup.Objects["entrance"];
                var exitObj = doorsGroup.Objects["exit"];
                float halfTileSize = Tilemap.TileWidth / 2f;

                Vector2 entrancePos = Tilemap.ToWorldPosition(new Vector2(entranceObj.X, entranceObj.Y));
                var entranceGround = _getSolidTileBelow(entrancePos);
                Insist.IsNotNull(entranceGround, "Couldn't find solid ground below entrance.");
                EntranceDoor = new Entity("entrance");
                EntranceDoor.Position = Tilemap.TileToWorldPosition(new Point(entranceGround.X, entranceGround.Y)) - Vector2.UnitY * halfTileSize;

                Vector2 exitPos = Tilemap.ToWorldPosition(new Vector2(exitObj.X, exitObj.Y));
                var exitGround = _getSolidTileBelow(exitPos);
                Insist.IsNotNull(exitGround, "Couldn't find solid ground below exit.");
                ExitDoor = new Entity("exit");
                ExitDoor.Position = Tilemap.TileToWorldPosition(new Point(exitGround.X, exitGround.Y)) - Vector2.UnitY * halfTileSize;
            }
        }
        void ResetChildCharacters()
        {
            CreateCharacters();
        }

        void CreateCharacters()
        {
            var charactersGroup = Tilemap.GetObjectGroup("characters");
            if (charactersGroup is null) return;
            foreach (var characterObject in charactersGroup.Objects)
            {
                try
                {
                    Type characterType = null;
                    if (characterObject.Template == string.Empty)
                        characterType = System.Type.GetType(characterObject.Type);
                    else
                    {
                        TmxTemplate template = Entity.Scene.Content.LoadTmxTemplate(characterObject.Template, Tilemap);
                        characterType = System.Type.GetType(template.Type);
                    }
                    Character character = Activator.CreateInstance(characterType) as Character;
                    Entity characterEntity = new();
                    characterEntity.AddComponent(character);
                    characterEntity.Position = Tilemap.ToWorldPosition(new Vector2(characterObject.X, characterObject.Y));
                    _charactersByTmxObject.Add(characterObject, characterEntity);
                    Entity.Scene.AddEntity(characterEntity);
                }
                catch (ArgumentException ex)
                {
                    Debug.Error($"Error creating instance of Character subclass with full name {characterObject.Type}. {ex.Message}");
                }
            }
        }
        #endregion
    }
}

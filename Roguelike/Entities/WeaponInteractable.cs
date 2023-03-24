using Nez;
using Nez.Tiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roguelike.Weapons;
using System;
using Nez.Sprites;
using Nez.Textures;
using System.Security.Cryptography;

namespace Roguelike.Entities
{
    public class WeaponInteractable : Component, IInteractListener
    {
        public Weapon Weapon { get; set; }
        public WeaponInteractable(Weapon weapon)
        {
            Weapon = weapon;
        }
        public override Component Clone()
        {
            WeaponInteractable interactable = base.Clone() as WeaponInteractable;
            interactable.Weapon = Weapon.Clone() as Weapon;
            return interactable;
        }
        public void OnHover(Entity source) { }
        public void OnInteract(Entity source)
        { 
            source.RemoveComponent<Weapon>();
            source.AddComponent(Weapon);
            Entity.Destroy();
        }

        public static Entity FromTmxObject(TmxObject obj, TmxMap map, TmxObjectGroup group)
        {
            var weapon = Weapon.FromTmxObject(obj, map);
            Insist.IsNotNull(weapon);
            if(weapon != null)
            {
                Entity weaponEntity = new();
                var tileset = group.Map.GetTilesetForTileGid(obj.Tile.Gid);
                var sourceRect = tileset.TileRegions[obj.Tile.Gid];
                var sprite = new Sprite(tileset.Image.Texture, sourceRect);
                weaponEntity.AddComponent(new WeaponInteractable(weapon))
                    .AddComponent(new SpriteRenderer(sprite))
                    .AddComponent(new InteractableOutline())
                    .AddComponent(new BoxCollider());
                return weaponEntity;
            }
            return null;
        }
    }
}

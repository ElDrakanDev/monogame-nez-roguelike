﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace Roguelike.Entities
{
    public interface IInteractListener
    {
        public void OnHover(Entity source);
        public void OnInteract(Entity source);
    }
    public class InteractableOutline : Component, IUpdatable, IInteractListener
    {
        public static Color InteractColor = Color.White;
        SpriteRenderer _renderer;
        OutlineEffect _outlineEffect;
        BoxCollider _collider;
        protected bool _isGettingHovered = false;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            _renderer = Entity.GetComponent<SpriteRenderer>();
            if (_renderer.Material is null)
                _renderer.Material = new Material(BlendState.NonPremultiplied);
            _outlineEffect = new OutlineEffect(_renderer.Sprite.SourceRect.Size.ToVector2());
            _collider = Entity.GetOrCreateComponent<BoxCollider>();
            _collider.PhysicsLayer = (int)LayerMask.Interactable;
        }
        public virtual void Update()
        {
            _outlineEffect.OutlineColor = InteractColor;
            if (_isGettingHovered)
                _renderer.Material.Effect = _outlineEffect;
            else
                _renderer.Material.Effect = null;

            _isGettingHovered = false;
        }
        
        public void OnHover(Entity source) => _isGettingHovered = true;
        public void OnInteract(Entity source) { }
    }
}

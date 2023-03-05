using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace Roguelike.Entities
{
    public interface IInteractSource
    {
        public bool IsInteracting { get; set; }
    }
    public interface IInteractListener
    {
        public void OnHover(Entity source);
        public void OnInteract(Entity source);
    }
    public abstract class Interactable : Component, IUpdatable
    {
        public static Color InteractColor = Color.White;
        SpriteRenderer _renderer;
        OutlineEffect _outlineEffect;
        Collider _collider;
        protected bool _isGettingHovered = false;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            _renderer = Entity.GetComponent<SpriteRenderer>();
            if (_renderer.Material is null)
                _renderer.Material = new Material(BlendState.NonPremultiplied);
            _outlineEffect = new OutlineEffect(_renderer.Sprite.SourceRect.Size.ToVector2());
            _collider = Entity.GetComponent<Collider>();
            _collider.PhysicsLayer = (int)LayerMask.Interactable;
        }
        public override void OnEnabled()
        {
            base.OnEnabled();
            Interactable.onHover += OnAnyHover;
            Interactable.onInteract += OnAnyInteract;
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            Interactable.onHover -= OnAnyHover;
            Interactable.onInteract -= OnAnyInteract;
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
        
        void OnAnyHover(Entity source, Entity hovered)
        {
            if(hovered == Entity)
            {
                _isGettingHovered = true;
                Hover(source);
            }
        }
        void OnAnyInteract(Entity source, Entity interacted)
        {
            if(interacted == Entity)
            {
                Interact(source);
            }
        }

        protected virtual void Hover(Entity source) { }
        protected abstract void Interact(Entity source);

        public static event Action<Entity, Entity> onHover;
        public static void OnHover(Entity source, Entity hovered) => onHover?.Invoke(source, hovered);

        public static event Action<Entity, Entity> onInteract;
        public static void OnInteract(Entity source, Entity interacted) => onInteract?.Invoke(source, interacted);

    }
}

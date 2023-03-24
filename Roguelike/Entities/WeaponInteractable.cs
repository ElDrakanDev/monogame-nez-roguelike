using Nez;
using Roguelike.Weapons;

namespace Roguelike.Entities
{
    public class WeaponInteractable : Component, IInteractListener
    {
        public Weapon Weapon { get; set; }
        public WeaponInteractable(Weapon weapon)
        {
            Weapon = weapon;
        }

        public void OnHover(Entity source) { }
        public void OnInteract(Entity source)
        {
            source.RemoveComponent<Weapon>();
            source.AddComponent(Weapon);
            Entity.Destroy();
        }
    }
}

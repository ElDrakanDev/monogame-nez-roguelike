using Nez;
using Roguelike.Weapons;

namespace Roguelike.Entities
{
    public class WeaponInteractable : Interactable
    {
        public Weapon Weapon { get; set; }
        public WeaponInteractable(Weapon weapon)
        {
            Weapon = weapon;
        }
        protected override void Interact(Entity source)
        {
            source.RemoveComponent<Weapon>();
            source.AddComponent(Weapon);
            Entity.Destroy();
        }
    }
}

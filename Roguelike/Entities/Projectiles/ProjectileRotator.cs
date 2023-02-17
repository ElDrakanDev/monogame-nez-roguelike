using Nez;

namespace Roguelike.Entities.Projectiles
{
    public class ProjectileRotator : Component, IUpdatable
    {
        Projectile _projectile;

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            _projectile = Entity.GetComponent<Projectile>();
        }
        public void Update()
        {
            Transform.Rotation = _projectile.Velocity.GetDirectionAngle();
        }
    }
}

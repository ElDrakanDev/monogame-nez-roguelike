using Nez.Textures;
using Nez;
using Microsoft.Xna.Framework;

namespace Roguelike.Entities.Characters
{
    public class ExampleEnemy : Character
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Size = new(96, 96);
            Stats = new CharacterStats(9999999999);
            Team = Teams.Enemy;
            TargetTeams = (int)Teams.Player;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            var texture = Entity.Scene.Content.LoadTexture(ContentPath.MM35_gb_Megaman);
            _spriteAnimator.Sprite = new Sprite(texture);
            _spriteAnimator.Color = Color.Yellow;
        }
        public override void Update()
        {
            base.Update();


        }
    }
}

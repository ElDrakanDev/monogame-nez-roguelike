using Microsoft.Xna.Framework.Input;
using Nez.Sprites;
using Nez.Textures;
using Nez;
using System;
using Microsoft.Xna.Framework;
using Roguelike.Entities.Projectiles;

namespace Roguelike.Entities.Characters
{
    public class ExamplePlayer : Character
    {
        

        public override void SetDefaults()
        {
            base.SetDefaults();
            Size = new(28, 28);
            Stats = new CharacterStats(50);
            Team = Teams.Player;
            TargetTeams = (int)Teams.Enemy;
        }
        
        
    }
}

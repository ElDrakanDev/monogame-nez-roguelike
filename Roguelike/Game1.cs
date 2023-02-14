using Nez;

namespace Roguelike
{
    public class Game1 : Core
    {
        protected override void Initialize()
        {
            base.Initialize();
            Window.AllowUserResizing = true;

            Scene = new BaseLevelScene();
        }
    }
}
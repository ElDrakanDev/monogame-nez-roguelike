using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace Roguelike.Entities.Characters.Players
{
    public class InputHandler : Component, IUpdatable
    {
        public Vector2 MoveDirection => new Vector2(_xInput, _yInput);
        public Vector2 AimDirection => _lastAimDirection;

        Vector2 _lastAimDirection;
        public VirtualButton JumpButton { get; private set; } = new VirtualButton(new VirtualButton.KeyboardKey(Keys.Space));
        public VirtualButton InteractButton { get; private set; } = new VirtualButton(new VirtualButton.KeyboardKey(Keys.E));
        public VirtualButton AttackButton { get; private set; } = new VirtualButton(new VirtualButton.KeyboardKey(Keys.J));

        VirtualAxis _xInput = new VirtualAxis(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.A, Keys.D));
        VirtualAxis _yInput = new VirtualAxis(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.CancelOut, Keys.W, Keys.S));

        public InputHandler()
        {
            UpdateOrder = int.MinValue;
        }

        public void Update()
        {
            if(MoveDirection != Vector2.Zero)
            {
                _lastAimDirection = MoveDirection;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez
{
    public class ColorTintEffect : Effect
    {
        public Color TintColor
        {
            get => _tintColor;
            set
            {
                if (_tintColor != value)
                {
                    _tintColor = value;
                    _tintColorParam.SetValue(value.ToVector4());
                }

            }
        }

        Color _tintColor = Color.White;

        EffectParameter _tintColorParam;

        public ColorTintEffect(Color tintColor) : base(Core.GraphicsDevice, EffectResource.ColorTintBytes)
        {
            _tintColor = tintColor;
            _tintColorParam = Parameters["_tintColor"];
            _tintColorParam.SetValue(_tintColor.ToVector4());
        }

        public ColorTintEffect() : this(Color.White) { }
    }
}

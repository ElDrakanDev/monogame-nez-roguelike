﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.UI;


namespace Nez
{
    public class OutlineEffect : Effect
    {
        public Color OutlineColor
        {
            get => _outlineColor;
            set
            {
                if (_outlineColor != value)
                {
                    _outlineColor = value;
                    _outlineColorParam.SetValue(value.ToVector4());
                }
                
            }
        }

        public Vector2 TextureSize
        {
            get => _textureSize;
            set
            {
                if(_textureSize != value)
                {
                    _textureSize = value;
                    _texelSizeParam.SetValue(GetTexelSize(value));
                }
            }
        }

        Vector2 GetTexelSize(Vector2 size) => new Vector2((float)(1 / (double)size.X), (float)(1 / (double)size.Y));

        Vector2 _textureSize;
        Color _outlineColor = Color.Red;
        EffectParameter _outlineColorParam;
        EffectParameter _texelSizeParam;

        public OutlineEffect(Vector2 textureSize) : base(Core.GraphicsDevice, EffectResource.OutlineBytes)
        {
            _texelSizeParam = Parameters["_texelSize"];
            _outlineColorParam = Parameters["_outlineColor"];


            _textureSize = textureSize;
            _texelSizeParam.SetValue(GetTexelSize(textureSize));
            _outlineColorParam.SetValue(_outlineColor.ToVector4());
        }
    }
}
using System;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Nez.Tiled
{
	public class TmxTemplate : IDisposable
	{
		public string Name { get; set; }
		public TmxObjectType ObjectType;
		public string Type;
		public float Width;
		public float Height;
        public float X;
        public float Y;
        public TmxLayerTile Tile;
		public bool Visible;
		public TmxText Text;

		public Vector2[] Points;
		public Dictionary<string, string> Properties;

		void IDisposable.Dispose() { }
	}
}

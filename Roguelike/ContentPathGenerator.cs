

namespace Nez
{
	/// <summary>
	/// class that contains the names of all of the files processed by the Pipeline Tool
	/// </summary>
	/// <remarks>
	/// Nez includes a T4 template that will auto-generate the content of this file.
	/// See: https://github.com/prime31/Nez/blob/master/FAQs/ContentManagement.md#auto-generating-content-paths"
	/// </remarks>
	class ContentPath
	{
		public static class Nez
		{
			public const string Directory = @"Content\nez";
		
			public static class Effects
			{
				public const string Directory = @"Content\nez\effects";
			
				public static class Transitions
				{
					public const string Directory = @"Content\nez\effects\transitions";
				
				}

				public const string ColorTint = @"Content\nez\effects\ColorTint.fx";
				public const string Outline = @"Content\nez\effects\Outline.fx";
			}

		}

		public static class Tiled
		{
			public const string Directory = @"Content\Tiled";
		
			public static class Boss
			{
				public const string Directory = @"Content\Tiled\Boss";
			
				public const string Rules = @"Content\Tiled\Boss\rules.txt";
				public const string Tall_room = @"Content\Tiled\Boss\tall_room.tmx";
			}

			public static class Fight
			{
				public const string Directory = @"Content\Tiled\Fight";
			
				public const string Example_map_platforms = @"Content\Tiled\Fight\example_map_platforms.tmx";
				public const string Rules = @"Content\Tiled\Fight\rules.txt";
				public const string Second_example_map = @"Content\Tiled\Fight\second_example_map.tmx";
			}

			public static class Shop
			{
				public const string Directory = @"Content\Tiled\Shop";
			
				public const string Penebolas = @"Content\Tiled\Shop\penebolas.tmx";
				public const string Rules = @"Content\Tiled\Shop\rules.txt";
			}

			public static class Start
			{
				public const string Directory = @"Content\Tiled\Start";
			
				public const string Example_map = @"Content\Tiled\Start\example_map.tmx";
				public const string Rules = @"Content\Tiled\Start\rules.txt";
			}

			public static class Templates
			{
				public const string Directory = @"Content\Tiled\Templates";
			
				public const string ExampleEnemy = @"Content\Tiled\Templates\Example Enemy.tx";
			}

			public static class TiledRules
			{
				public const string Directory = @"Content\Tiled\TiledRules";
			
				public const string AutomappingIcons = @"Content\Tiled\TiledRules\AutomappingIcons.tsx";
				public const string AutomappingTiles = @"Content\Tiled\TiledRules\AutomappingTiles.png";
				public const string Orange_autotiles = @"Content\Tiled\TiledRules\orange_autotiles.tmx";
				public const string OrangeBricks = @"Content\Tiled\TiledRules\OrangeBricks.png";
				public const string TilesetOrange = @"Content\Tiled\TiledRules\TilesetOrange.tsx";
			}

			public const string Rules = @"Content\Tiled\rules.txt";
			public const string TilesetOrange = @"Content\Tiled\TilesetOrange.tsx";
		}

		public const string ContentManager = @"Content\ContentManager.mgcb";
		public const string Exampleball = @"Content\example-ball.png";
		public const string ExampleSword = @"Content\ExampleSword.png";
		public const string MM35_gb_Megaman = @"Content\MM3-5_gb_Megaman.png";

	}
}


using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quarantine
{
	public static class SpriteManager
	{
		public static uint[] LoadPalette(string path)
		{
			var paletteData = File.ReadAllBytes(path);
			var palette = new uint[256];

			for (int i = 1; i < 256; i++) //first entry is transparent
			{
				int r = paletteData[i * 3 + 0] * 4;
				int g = paletteData[i * 3 + 1] * 4;
				int b = paletteData[i * 3 + 2] * 4;
				palette[i] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
			}

			return palette.ToArray();
		}

		public static Sprite[] LoadSprites(string filePath)
		{
			List<Sprite> sprites = new List<Sprite>();
			var data = File.ReadAllBytes(filePath);
			int entriesCount = data[0];
			int offset = 0;

			for (int i = 0; i < entriesCount; i++)
			{
				int width = data[i * 2 + 1];
				int height = data[i * 2 + 2];
				var sp = new Sprite(width, height, entriesCount * 2 + 1 + offset, data);
				sprites.Add(sp);
				offset += width * height;
			}

			return sprites.ToArray();
		}
	}
}

using System.IO;

namespace Quarantine
{
	public static class SpriteManager
	{
		public static void LoadPalette(string path, uint[] palette)
		{
			var paletteData = File.ReadAllBytes(path);

			for (int i = 1; i < 256; i++) //first entry is transparent
			{
				int r = paletteData[i * 3 + 0] * 4;
				int g = paletteData[i * 3 + 1] * 4;
				int b = paletteData[i * 3 + 2] * 4;
				palette[i] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
			}
		}

		public static Sprite[] LoadSprites(string filePath)
		{
			var data = File.ReadAllBytes(filePath);
			int entriesCount = data[0];
			int offset = 0;

			var sprites = new Sprite[entriesCount];
			for (int i = 0; i < entriesCount; i++)
			{
				int width = data[i * 2 + 1];
				int height = data[i * 2 + 2];
				var sp = new Sprite(width, height, entriesCount * 2 + 1 + offset, data);
				sprites[i] = sp;
				offset += width * height;
			}

			return sprites;
		}
	}
}

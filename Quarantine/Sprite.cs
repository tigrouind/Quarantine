using System;
using System.Drawing;

namespace Quarantine
{
	public class Sprite(int width, int height, int offset, byte[] data)
	{
		public readonly int Width = width, Height = height;

		public void Render(int destx, int width, uint[] pixels, uint[] palette, int[] bitmapIndex, int outfit)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					int src = x / 4 + y / 4 * Width + (x % 4 * Height + y % 4) * Width / 4;
					int color = data[offset + src];
					if (color != 0)
					{
						int index = destx + x + y * width;
						pixels[index] = palette[color];
						bitmapIndex[index] = outfit + 1;
					}
				}
			}
		}
	}
}

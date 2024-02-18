using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Quarantine
{
	public class Sprite(int width, int height, int offset, byte[] data)
	{
		public readonly int Width = width, Height = height;

		public void Render(int destx, int width, uint[] pixels, uint[] palette, int[] bitmapIndex, int outfit, bool selected)
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
						pixels[index] = selected ? BlendColors(palette[color], 0x00008080) : palette[color];
						bitmapIndex[index] = outfit + 1;
					}
				}
			}
		}

		static uint BlendColors(uint colorA, uint colorB)
		{
			var (rA, gA, bA) = GetRGB(colorA);
			var (rB, gB, bB) = GetRGB(colorB);

			uint rBlend = ((rA + rB) / 2) & 0xFF;
			uint gBlend = ((gA + gB) / 2) & 0xFF;
			uint bBlend = ((bA + bB) / 2) & 0xFF;

			return 0xFF000000 | (rBlend << 16) | (gBlend << 8) | bBlend;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static (uint r, uint g, uint b) GetRGB(uint color)
			{
				uint r = (color >> 16) & 0xFF;
				uint g = (color >> 8) & 0xFF;
				uint b = color & 0xFF;

				return (r, g, b);
			}
		}
	}
}

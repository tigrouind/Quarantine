namespace Quarantine
{
	public class Sprite
	{
		public readonly int Width, Height;
		readonly int Offset;
		readonly byte[] Data;

		public Sprite(int width, int height, int offset, byte[] data)
		{
			Width = width;
			Height = height;
			Offset = offset;
			Data = data;
		}

		public void Render(int destx, int width, uint[] pixels, uint[] palette)
		{
			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					int src = x / 4 + y / 4 * Width + (x % 4 * Height + y % 4) * Width / 4;
					int color = Data[Offset + src];
					if (color != 0)
					{
						pixels[destx + x + y * width] = palette[color];
					}
				}
			}
		}
	}
}

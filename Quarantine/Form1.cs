using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Quarantine
{
	public partial class Form1 : Form
	{
		DirectBitmap bitmap = new DirectBitmap(1, 1);
		readonly Stack<(int, int[])> undo = new Stack<(int, int[])>();
		readonly Stack<(int, int[])> redo = new Stack<(int, int[])>();
		readonly Dictionary<int, Sprite[]> spriteCache = new Dictionary<int, Sprite[]>();

		readonly Random rand = new Random();
		const string RootFolder = "data";
		uint[] palette;
		int[] outfits;
		int body;
		int width, height;
		int zoom = 1;

		public Form1()
		{
			MouseWheel += Form1_MouseWheel;
			InitializeComponent();
		}

		void Form1_Load(object sender, EventArgs e)
		{
			palette = SpriteManager.LoadPalette(Path.Combine(RootFolder, "KEMOMAP1.ZZZ"));
			RandomOutfit();
			Form1_Resize(this, EventArgs.Empty);
		}

		void Form1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.DrawImage(bitmap.Bitmap, 0, 0, bitmap.Width * zoom, bitmap.Height * zoom);
		}

		void DrawSprites()
		{
			var entry = new int[] { 0, 1, 3, 2 }[body];

			var sprites = new List<Sprite>();
			foreach (var outfit in outfits)
			{
				if (!spriteCache.TryGetValue(outfit, out Sprite[] spr))
				{
					spr = SpriteManager.LoadSprites(Path.Combine(RootFolder, $"BODA{outfit}.SPR"));
					spriteCache.Add(outfit, spr);
				}

				sprites.Add(spr[entry]);
			}

			width = sprites.Max(x => x.Width);
			height = sprites.Max(x => x.Height);
			if (bitmap.Width < width || bitmap.Height < height)
			{
				bitmap?.Dispose();
				bitmap = new DirectBitmap(Math.Max(width, bitmap.Width), Math.Max(height, bitmap.Height));
			}

			Array.Clear(bitmap.Bits, 0, bitmap.Bits.Length);
			foreach (var spr in sprites)
			{
				spr.Render(bitmap.Width, bitmap.Bits, palette);
			}

			SetZoom();
			Invalidate();
		}

		int[] GetRandomOutfits()
		{
			int head = Head(), hair = Hair(), top = -1, pants = -1, coat = -1, shoes = Shoes();

			if (ModifierKeys == Keys.Shift)
			{
				switch (rand.Next(4))
				{
					case 0:
						pants = Pants();
						break;

					case 1:
						top = Top();
						break;

					case 2:
						top = TopLeather();
						pants = Pants();
						break;

					case 3:
						top = TopLeather();
						break;
				}
			}
			else
			{
				top = Top();
				pants = Pants();
				coat = Coat();
			}

			int Head() => rand.Next(2, 18);
			int Hair() => rand.Next(18, 63);
			int Top() => rand.Next(63, 77);
			int TopLeather() => rand.Next(74, 77);
			int Pants() => rand.Next(77, 83);
			int Coat() => rand.Next(83, 96);
			int Shoes() => rand.Next(96, 103);

			return (new int[] { 1, head, hair, top, pants, coat, shoes })
				.Where(x => x >= 0)
				.ToArray();
		}

		void RandomOutfit()
		{
			body = rand.Next(4);
			outfits = GetRandomOutfits();
			DrawSprites();
		}

		bool SetZoom()
		{
			int newZoom = Math.Max(1, Math.Min(ClientSize.Width / width, ClientSize.Height / height));
			if (zoom != newZoom)
			{
				zoom = newZoom;
				return true;
			}

			return false;
		}

		void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			if (MouseButtons == MouseButtons.Left)
			{
				//skip to the end
				while (redo.Count > 0)
				{
					undo.Push((body, outfits));
					(body, outfits) = redo.Pop();
				}

				undo.Push((body, outfits));
				RandomOutfit();
			}
		}

		void Form1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (MouseButtons == MouseButtons.Right || (ModifierKeys & Keys.Shift) != 0)
			{
				Body();
			}
			else
			{
				History();
			}

			void Body()
			{
				if (e.Delta > 0)
				{
					if (body < 3)
					{
						body++;
						DrawSprites();
					}
				}
				else if (e.Delta < 0)
				{
					if (body > 0)
					{
						body--;
						DrawSprites();
					}
				}
			}

			void History()
			{
				if (e.Delta > 0)
				{
					Redo();
				}
				else if (e.Delta < 0)
				{
					Undo();
				}

				void Redo()
				{
					if (redo.Count > 0)
					{
						undo.Push((body, outfits));
						(body, outfits) = redo.Pop();
						DrawSprites();
					}
				}

				void Undo()
				{
					if (undo.Count > 0)
					{
						redo.Push((body, outfits));
						(body, outfits) = undo.Pop();
						DrawSprites();
					}
				}
			}
		}

		void Form1_Resize(object sender, EventArgs e)
		{
			if (SetZoom())
			{
				Invalidate();
			}
		}
	}
}

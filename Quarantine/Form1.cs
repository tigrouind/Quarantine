using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Quarantine
{
	public partial class Form1 : Form
	{
		DirectBitmap bitmap = new DirectBitmap(1, 1);
		int zoom = 1;

		public Form1()
		{
			InitializeComponent();
		}

		void Form1_Load(object sender, EventArgs e)
		{
			Outfits.LoadFolder("DATA");
			Outfits.RandomOutfit(false);
			DrawSprites();
		}

		void Form1_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
			e.Graphics.DrawImage(bitmap.Bitmap, 0, 0, bitmap.Width * zoom, bitmap.Height * zoom);
		}

		void DrawSprites()
		{
			ResizeBitmap();
			Array.Clear(bitmap.Bits, 0, bitmap.Bits.Length);
			Outfits.DrawSprites(bitmap);
			SetZoom();
			Invalidate();
		}

		void ResizeBitmap()
		{
			int width = Outfits.MaxWidth;
			int height = Outfits.MaxHeight;
			if (bitmap.Width < width || bitmap.Height < height)
			{
				bitmap?.Dispose();
				bitmap = new DirectBitmap(Math.Max(width, bitmap.Width), Math.Max(height, bitmap.Height));
			}
		}

		bool SetZoom()
		{
			int newZoom = Math.Max(1, Math.Min(ClientSize.Width / bitmap.Width, ClientSize.Height / bitmap.Height));
			if (zoom != newZoom)
			{
				zoom = newZoom;
				return true;
			}

			return false;
		}

		void NewOutfit(bool shift)
		{
			Outfits.PushState();
			Outfits.RandomOutfit(shift);
			DrawSprites();
		}

		void Undo()
		{
			if (Outfits.Undo())
			{
				DrawSprites();
			}
		}

		void Redo()
		{
			if (Outfits.Redo())
			{
				DrawSprites();
			}
		}

		void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			if (MouseButtons == MouseButtons.Left)
			{
				NewOutfit((ModifierKeys & Keys.Shift) != 0);
			}
		}

		void Form1_MouseUp(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.XButton1:
					Undo();
					break;

				case MouseButtons.XButton2:
					Redo();
					break;
			}
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Space:
					NewOutfit((e.Modifiers & Keys.Shift) != 0);
					break;

				case Keys.Left:
				case Keys.Up:
					Undo();
					break;

				case Keys.Right:
				case Keys.Down:
					Redo();
					break;
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

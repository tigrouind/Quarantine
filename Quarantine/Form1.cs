using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Quarantine
{
	public partial class Form1 : Form
	{
		DirectBitmap bitmap = new(1, 1);
		int[] bitmapIndex = new int[1];
		int lastSelectedOutfit;
		Point lastPosition;
		int zoom = 1;

		public Form1()
		{
			InitializeComponent();
			MouseWheel += Form1_MouseWheel;
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
			Array.Clear(bitmapIndex, 0, bitmapIndex.Length);
			Outfits.DrawSprites(bitmap, bitmapIndex, lastSelectedOutfit);
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
				bitmapIndex = new int[bitmap.Width * bitmap.Height];
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
			else if (MouseButtons == MouseButtons.Right && (ModifierKeys & Keys.Shift) != 0)
			{
				if (lastSelectedOutfit > 0)
				{
					Outfits.SetOutfit(lastSelectedOutfit, -1);
					DrawSprites();
				}
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

				case Keys.PageUp:
				case Keys.Left:
				case Keys.Up:
					Undo();
					break;

				case Keys.PageDown:
				case Keys.Right:
				case Keys.Down:
					Redo();
					break;

				case Keys.D1:
				case Keys.D2:
				case Keys.D3:
				case Keys.D4:
				case Keys.D5:
				case Keys.D6:
					int offset = (e.Modifiers & Keys.Shift) != 0 ? -1 : 1;
					Outfits.ChangeOutfit((int)(e.KeyValue - Keys.D1) + 1, offset);
					DrawSprites();
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

		void Form1_MouseWheel(object sender, MouseEventArgs e)
		{
			if (lastSelectedOutfit > 0)
			{
				Outfits.ChangeOutfit(lastSelectedOutfit, Math.Sign(e.Delta));
				DrawSprites();
			}
		}

		private void Form1_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = new Point(e.X / zoom, e.Y / zoom);
			if (pos.X < bitmap.Width && pos.Y < bitmap.Height && lastPosition != pos)
			{
				lastPosition = pos;
				var outfit = bitmapIndex[pos.X + pos.Y * bitmap.Width] - 1;
				if (outfit != lastSelectedOutfit && outfit != 0)
				{
					lastSelectedOutfit = outfit;
					DrawSprites();
				}
			}
		}
	}
}

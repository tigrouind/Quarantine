using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Quarantine
{
	public static class Outfits
	{
		readonly static Stack<int[]> undo = new();
		readonly static Stack<int[]> redo = new();
		readonly static Sprite[][] sprites = new Sprite[102][];
		readonly static int[] entryOrder = [0, 1, 3, 2];

		readonly static Random rand = new();
		readonly static uint[] palette = new uint[256];
		readonly static int[] outfits = new int[7];
		readonly static int[] maxWidths = new int[4];

		public static int MaxWidth => maxWidths.Sum();
		public static int MaxHeight { private set; get; }

		public static void ChangeOutfit(int index, int offset)
		{
			var outfitranges = new int[,]
			{
				{ -1, -1 },
				{ 2, 18 }, //head
				{ 18, 63 }, //hair
				{ 63, 77 }, //top
				{ 77, 83 }, //pants
				{ 83, 96 }, //coat
				{ 96, 103 }, //shoes
			};

			int start = outfitranges[index, 0];
			int end = outfitranges[index, 1];
			int length = end - start;
			outfits[index] = (outfits[index] - start + length + offset) % length + start;
		}

		public static void SetOutfit(int index, int value)
		{
			outfits[index] = value;
		}

		public static void LoadFolder(string rootFolder)
		{
			SpriteManager.LoadPalette(Path.Combine(rootFolder, "KEMOMAP1.ZZZ"), palette);
			LoadAllSprites();

			void LoadAllSprites()
			{
				for (int outfit = 0; outfit < sprites.GetLength(0); outfit++)
				{
					var spr = SpriteManager.LoadSprites(Path.Combine(rootFolder, $"BODA{outfit + 1}.SPR"));
					sprites[outfit] = spr;

					for (int body = 0; body < 4; body++)
					{
						var entry = entryOrder[body];
						maxWidths[body] = Math.Max(maxWidths[body], spr[entry].Width);
						MaxHeight = Math.Max(MaxHeight, spr[entry].Height);
					}
				}
			}
		}

		public static void RandomOutfit(bool shift)
		{
			int head = Head(), hair = Hair(), top = -1, pants = -1, coat = -1, shoes = Shoes();

			if (shift)
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

			Array.Copy(new int[] { 1, head, hair, top, pants, coat, shoes }, outfits, outfits.Length);
		}

		public static void DrawSprites(DirectBitmap bitmap, int[] bitmapIndex, int selectedOutfit)
		{
			int destx = 0;

			for (int body = 0; body < 4; body++)
			{
				DrawSprite(entryOrder[body]);
				destx += maxWidths[body];
			}

			void DrawSprite(int body)
			{
				foreach (var (index, outfit) in outfits
					.Select((x, i) => (Index: i, Outfit:x))
					.Where(x => x.Outfit >= 0))
				{
					sprites[outfit - 1][body].Render(destx, bitmap.Width, bitmap.Bits, palette, bitmapIndex, index, index == selectedOutfit && selectedOutfit != 0);
				}
			}
		}

		public static void PushState()
		{
			//skip to the end
			while (redo.Count > 0)
			{
				undo.Push([.. outfits]);
				Array.Copy(redo.Pop(), outfits, outfits.Length);
			}

			undo.Push([.. outfits]);
		}

		public static bool Redo()
		{
			if (redo.Count > 0)
			{
				undo.Push([.. outfits]);
				Array.Copy(redo.Pop(), outfits, outfits.Length);
				return true;
			}

			return false;
		}

		public static bool Undo()
		{
			if (undo.Count > 0)
			{
				redo.Push([.. outfits]);
				Array.Copy(undo.Pop(), outfits, outfits.Length);
				return true;
			}

			return false;
		}
	}
}

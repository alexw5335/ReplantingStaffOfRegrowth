using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System;

namespace ReplantingStaffOfRegrowth {


	public class ReplantingStaffOfRegrowth : Mod { }



	class Class1 : GlobalTile {


		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
			var player = Main.player[Main.myPlayer];
			var targetTile = Framing.GetTileSafely(i, j);
			var baseTile = Framing.GetTileSafely(i, j + 1);

			if (Main.netMode == NetmodeID.Server) return;
			if (type != TileID.BloomingHerbs && type != TileID.MatureHerbs) return;
			if (player.HeldItem.type != ItemID.StaffofRegrowth) return;
			if (baseTile.TileType != TileID.ClayPot && baseTile.TileType != TileID.PlanterBox) return;

			//Taken from WorldGen::KillTile_DropItems
			WorldGen.KillTile_GetItemDrops(
				i,
				j,
				targetTile,
				out var item1,
				out var count1,
				out var item2,
				out var count2
			);

			// Reduce seed drop count by one to account for replanting.
			if (item2 == ItemID.BlinkrootSeeds ||
				item2 == ItemID.DaybloomSeeds ||
				item2 == ItemID.MoonglowSeeds ||
				item2 == ItemID.WaterleafSeeds ||
				item2 == ItemID.FireblossomSeeds ||
				item2 == ItemID.ShiverthornSeeds ||
				item2 == ItemID.DeathweedSeeds
			) count2--;

			// Primary drop
			if (item1 > 0 && count1 > 0) {
				var num3 = Item.NewItem(null, i * 16, j * 16, 16, 16, item1, count1, false, -1, false, false);
				Main.item[num3].TryCombiningIntoNearbyItems(num3);
				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num3, 1f);
			}

			// Secondary drop
			if (item2 > 0 && count2 > 0) {
				var num4 = Item.NewItem(null, i * 16, j * 16, 16, 16, item2, count2, false, -1, false, false);
				Main.item[num4].TryCombiningIntoNearbyItems(num4);
				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num4, 1f);
			}

			// Taken from BotanyPlus. Replants the seed.
			fail = true;
			targetTile.TileType = 82;
			WorldGen.SquareTileFrame(i, j);
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, i, j, 1);
		}


	}


}
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using log4net;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Localization;

namespace ColoredDamageTypes
{
	class ColoredDamageTypes : Mod
	{
        public static Mod ThoriumMod;
		public static Mod EnigmaMod;
		public static Mod RedemptionMod;
		public static Mod CalamityMod;
		public static Mod DbzMod;
		public static Mod EsperClassMod;
		public static Mod BattleRodsMod;
		public static Mod ClickerMod;
		public static Mod ExampleMod;
		//public static Mod OrchidMod;

		//public static Mod TremorMod;
		public static bool ChangeTooltipColor = true;
		public static bool ChangeDamageColor = true;
		public static ColoredDamageTypes instance;
		public static string GithubUserName { get { return "PvtFudgepants"; } }
		public static string GithubProjectName { get { return "tModLoader---Colored-Damage-Types"; } }

		public static Item[] ProjectileWeaponSpawns = new Item[1001];

		public ColoredDamageTypes()
		{
			instance = this;
		}

		public override void PostSetupContent()
		{
			ModLoader.TryGetMod("ThoriumMod", out ThoriumMod);
            ModLoader.TryGetMod("Laugicality", out EnigmaMod);
			ModLoader.TryGetMod("Redemption", out RedemptionMod);
			ModLoader.TryGetMod("CalamityMod", out CalamityMod);
			ModLoader.TryGetMod("DBZMOD", out DbzMod);
			ModLoader.TryGetMod("EsperClass", out EsperClassMod);
			ModLoader.TryGetMod("UnuBattleRods", out BattleRodsMod);
			ModLoader.TryGetMod("ClickerClass", out ClickerMod);
			//OrchidMod = ModLoader.GetMod("OrchidMod");
			//TremorMod = ModLoader.GetMod("Tremor");
		}

		public static void Log(object message, params object[] formatData)
		{
			instance.Logger.InfoFormat("[ColoredDamageTypes] " + string.Format(message.ToString(), formatData), "ColoredDamageTypes");
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			//Main.NewText("Got packet");
			if ( Main.netMode == NetmodeID.Server ) {
				ModPacket packet = instance.GetPacket();
				packet.Write(reader.ReadByte());
				packet.Write(reader.ReadInt32());
				packet.Write(reader.ReadSingle());
				packet.Write(reader.ReadByte());
				packet.Send(-1, whoAmI);
				packet.Close();
			}
			if (Main.netMode == NetmodeID.MultiplayerClient ) {
				Netcode.recentcolor_in = (DamageTypes.Types)reader.ReadByte();
				Netcode.recentdmg_in = reader.ReadInt32();
				Netcode.recentkb_in = reader.ReadSingle();
				Netcode.recentcrit_in = reader.ReadByte();
			}
		}
	}
}

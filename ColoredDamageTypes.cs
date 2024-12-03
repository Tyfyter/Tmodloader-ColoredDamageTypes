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
using Terraria.ModLoader.UI;
using Terraria.Localization;
using System.Linq;
using System.Windows.Forms;

namespace ColoredDamageTypes
{
	class ColoredDamageTypes : Mod
	{
		//public static int condense_count = 0;
		//public static int condense_total = 0;

		public static Dictionary<string, Dictionary<string, int[]>> condense_totals = new Dictionary<string, Dictionary<string, int[]>>();
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
				Netcode.recentcolor_in = reader.ReadByte();
				Netcode.recentdmg_in = reader.ReadInt32();
				Netcode.recentkb_in = reader.ReadSingle();
				Netcode.recentcrit_in = reader.ReadByte();
			}
		}
		public override object Call(params object[] args)
		{
            //coloreddamagetypes.Call(ModContent.GetInstance<ExampleDamageClass>(), new Color(255, 0, 0), new Color(0, 255, 0), new Color(0, 0, 255));
            if (args[0].Equals("AddDamageType"))
            {
				if (args[1] is DamageClass dc)
				{
					if (!DamageTypes.DamageClasses.Contains(dc))
					{
						if (args[2] is Color ttcolor && args[3] is Color dmgcolor && args[4] is Color critdmgcolor)
						{
							string dcname = dc.ToString();
							zCrossModConfig.DamageType dt = new zCrossModConfig.DamageType(dcname, ttcolor, dmgcolor, critdmgcolor);
							zCrossModConfig.CrossModDamageConfig_Orig.Add(dcname, dt);

							DamageTypes.DamageClasses.Add(dc);
							ColoredDamageTypes.Log("Added Cross-mod type: " + dcname);
						}
						else if (args[2] is (int r1, int g1, int b1) && args[3] is (int r2, int g2, int b2) && args[4] is (int r3, int g3, int b3))
						{
							string dcname = dc.ToString();
							zCrossModConfig.DamageType dt = new zCrossModConfig.DamageType(dcname, new Color(r1, g1, b1), new Color(r2, g2, b2), new Color(r3, g3, b3));
							zCrossModConfig.CrossModDamageConfig_Orig.Add(dcname, dt);

							DamageTypes.DamageClasses.Add(dc);
							ColoredDamageTypes.Log("Added Cross-mod type: " + dcname);
						}
					}
                    else
					{
						ColoredDamageTypes.Log("Cross-mod type already exists, changing default colors provided by mod. " + dc.ToString());

						if (args[2] is Color ttcolor && args[3] is Color dmgcolor && args[4] is Color critdmgcolor)
						{
							string dcname = dc.ToString();
							zCrossModConfig.CrossModDamageConfig_Orig[dcname].TooltipColor = ttcolor;
							zCrossModConfig.CrossModDamageConfig_Orig[dcname].DamageColor = dmgcolor;
							zCrossModConfig.CrossModDamageConfig_Orig[dcname].CritDamageColor = critdmgcolor;

						}
						else if (args[2] is (int r1, int g1, int b1) && args[3] is (int r2, int g2, int b2) && args[4] is (int r3, int g3, int b3))
						{
							string dcname = dc.ToString();
							zCrossModConfig.CrossModDamageConfig_Orig[dcname].TooltipColor = new Color(r1, g1, b1);
							zCrossModConfig.CrossModDamageConfig_Orig[dcname].DamageColor = new Color(r2, g2, b2);
							zCrossModConfig.CrossModDamageConfig_Orig[dcname].CritDamageColor = new Color(r3, g3, b3);

						}
					}
				}
			}
			

			return this;
		}
        public override void PostSetupContent()
        {
			LoadModdedDamageTypes();
			Config.crossModInstance.CrossModDamageConfig = new Dictionary<string, zCrossModConfig.DamageType>(zCrossModConfig.CrossModDamageConfig_Orig);
			

		}

		public static void LoadModdedDamageTypes()
		{
			List<DamageClass> DefaultTypes = new List<DamageClass>();
			DefaultTypes.Add(DamageClass.Default);
			DefaultTypes.Add(DamageClass.Melee);
			DefaultTypes.Add(DamageClass.Magic);
			DefaultTypes.Add(DamageClass.Ranged);
			DefaultTypes.Add(DamageClass.Generic);
			DefaultTypes.Add(DamageClass.MagicSummonHybrid);
			DefaultTypes.Add(DamageClass.MeleeNoSpeed);
			DefaultTypes.Add(DamageClass.Summon);
			DefaultTypes.Add(DamageClass.SummonMeleeSpeed);
			DefaultTypes.Add(DamageClass.Throwing);
			for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
				Item itemToCheck = new Item();
				itemToCheck.SetDefaults(i, true);

				if (!DefaultTypes.Contains(itemToCheck.DamageType) && !DamageTypes.DamageClasses.Contains(itemToCheck.DamageType))
                {
					ColoredDamageTypes.Log("Found modded damage type! " + itemToCheck.DamageType.FullName.ToString());

					string dcname = itemToCheck.DamageType.FullName.ToString();
					zCrossModConfig.DamageType dt = new zCrossModConfig.DamageType(dcname, new Color(255, 255, 255), new Color(255, 160, 80), new Color(255, 100, 30));
					zCrossModConfig.CrossModDamageConfig_Orig.Add(dcname, dt);

					DamageTypes.DamageClasses.Add(itemToCheck.DamageType);
				}
            }
		}


	}
}

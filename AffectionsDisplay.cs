using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Terraria.ModLoader;

namespace AffectionsDisplay;

public class AffectionsDisplay : Mod
{
	internal static ModKeybind toggleDisplay;
	public static AffectionsDisplay Instance { get; private set; }
	public static AffectionsDisplayConfig Config => ModContent.GetInstance<AffectionsDisplayConfig>();

	public override void Load()
	{
		toggleDisplay = KeybindLoader.RegisterKeybind(this, "Toggle preferences data display", "LeftControl");
		Instance = this;
	}

}

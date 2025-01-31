using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Terraria.ModLoader;

namespace PreferencesDisplay;

public class PreferencesDisplay : Mod
{
	internal static ModKeybind toggleDisplay;
	public static PreferencesDisplay Instance { get; private set; }
	public static PreferencesDisplayConfig Config => ModContent.GetInstance<PreferencesDisplayConfig>();

	public override void Load()
	{
		toggleDisplay = KeybindLoader.RegisterKeybind(this, "Toggle preferences data display", "LeftControl");
		Instance = this;
	}

}

using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PreferencesDisplay;

public class PreferencesDisplayConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[ColorHSLSlider, ColorNoAlpha] public Color textColor;
	public static Color TextColor => ModContent.GetInstance<PreferencesDisplayConfig>().textColor;

	[Range(0.1f, 3)] public float textScale = 1;
	public static float TextScale => ModContent.GetInstance<PreferencesDisplayConfig>().textScale;
}
